using System;
using dc.libs;

namespace DebugMod.Core.Utilities
{
    public static class MathHelper
    {
       
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static class Random
        {

            public static int Range(int minInclusive, int maxExclusive, Rand rng)
            {
                ValidationHelper.NotNull(rng, nameof(rng));
                ValidationHelper.That(minInclusive < maxExclusive, "min必须小于max", nameof(minInclusive));

                int range = maxExclusive - minInclusive;
                if (range <= 0) return minInclusive;

                double seedResult = rng.seed * 16807.0 % 2147483647.0;
                rng.seed = seedResult;
                int randomValue = (int)seedResult & 1073741823;
                randomValue %= range;

                return minInclusive + randomValue;
            }

           
            public static double Range(double minInclusive, double maxInclusive, Rand rng)
            {
                ValidationHelper.NotNull(rng, nameof(rng));
                ValidationHelper.That(minInclusive <= maxInclusive, "min必须小于等于max", nameof(minInclusive));

                double seedResult = rng.seed * 16807.0 % 2147483647.0;
                rng.seed = seedResult;
                double randomValue = ((int)seedResult & 1073741823) / 1073741823.0;

                return minInclusive + (maxInclusive - minInclusive) * randomValue;
            }


            public static bool Bool(Rand rng)
            {
                ValidationHelper.NotNull(rng, nameof(rng));
                return Range(0, 2, rng) == 0;
            }

           
            public static bool Bool(Rand rng, double probability)
            {
                ValidationHelper.NotNull(rng, nameof(rng));
                ValidationHelper.InRange(probability, 0.0, 1.0, nameof(probability));

                return Range(0.0, 1.0, rng) < probability;
            }

         
            public static T Choice<T>(T[] array, Rand rng)
            {
                ValidationHelper.NotNull(array, nameof(array));
                ValidationHelper.NotEmpty(array, nameof(array));
                ValidationHelper.NotNull(rng, nameof(rng));

                int index = Range(0, array.Length, rng);
                return array[index];
            }


            public static T Choice<T>(System.Collections.Generic.List<T> list, Rand rng)
            {
                ValidationHelper.NotNull(list, nameof(list));
                ValidationHelper.NotEmpty(list, nameof(list));
                ValidationHelper.NotNull(rng, nameof(rng));

                int index = Range(0, list.Count, rng);
                return list[index];
            }

            public static void Shuffle<T>(T[] array, Rand rng)
            {
                ValidationHelper.NotNull(array, nameof(array));
                ValidationHelper.NotNull(rng, nameof(rng));

                for (int i = array.Length - 1; i > 0; i--)
                {
                    int j = Range(0, i + 1, rng);
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }


            public static void Shuffle<T>(System.Collections.Generic.List<T> list, Rand rng)
            {
                ValidationHelper.NotNull(list, nameof(list));
                ValidationHelper.NotNull(rng, nameof(rng));

                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = Range(0, i + 1, rng);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
        }     
    }
}