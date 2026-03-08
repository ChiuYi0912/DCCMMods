using dc.haxe.ds;
using dc.hl.types;
using HaxeProxy.Runtime;
using DebugMod.Core.Utilities;
using ModCore.Utilities;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DebugMod.Core.Extensions
{

    public static class EnumerableExtensions
    {

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            ValidationHelper.NotNull(source, nameof(source));
            return comparer == null ? new HashSet<T>(source) : new HashSet<T>(source, comparer);
        }



        public static IEnumerable<dynamic> AsEnumerable(this ArrayObj arrayObj)
        {
            ValidationHelper.NotNull(arrayObj, nameof(arrayObj));

            for (int i = 0; i < arrayObj.length; i++)
            {
                if (arrayObj.getDyn(i) != null)
                    yield return arrayObj.getDyn(i)!;
            }
        }
        public static async IAsyncEnumerable<dynamic> AsEnumerableAsync(this ArrayObj arrayObj, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ValidationHelper.NotNull(arrayObj, nameof(arrayObj));
            await Task.Yield();
            for (int i = 0; i < arrayObj.length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (arrayObj.array[i] != null)
                    yield return arrayObj.array[i]!;
            }
        }




        public static List<T> ToList<T>(this ArrayObj arrayObj, Func<dynamic, T> converter)
        {
            ValidationHelper.NotNull(arrayObj, nameof(arrayObj));
            ValidationHelper.NotNull(converter, nameof(converter));

            var list = new List<T>(arrayObj.length);
            for (int i = 0; i < arrayObj.length; i++)
            {
                var item = arrayObj.array[i];
                if (item != null)
                {
                    list.Add(converter(item));
                }
            }
            return list;
        }


        public static ArrayObj ToArrayObj<T>(this IEnumerable<T> source)
        {
            ValidationHelper.NotNull(source, nameof(source));

            var arrayObj = (ArrayObj)ArrayUtils.CreateDyn().array;

            if (source is ICollection<T> collection)
            {
                if (source is List<T> list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        arrayObj.push(list[i]);
                    }
                    return arrayObj;
                }

                foreach (var item in collection)
                {
                    arrayObj.push(item);
                }
                return arrayObj;
            }

            foreach (var item in source)
            {
                arrayObj.push(item);
            }
            return arrayObj;
        }



        public static dynamic? GetSafe(this ArrayObj arrayObj, int index)
        {
            if (arrayObj == null || index < 0 || index >= arrayObj.length)
            {
                ValidationHelper.NotNull(arrayObj!, $"{nameof(arrayObj)}");
                return null;
            }
            return arrayObj.array[index];
        }


        public static bool IsNullOrEmpty(this ArrayObj arrayObj)
        {
            return arrayObj == null || arrayObj.length == 0;
        }


        public static bool HasItems(this ArrayObj arrayObj)
        {
            return arrayObj != null && arrayObj.length > 0;
        }


        public static Dictionary<int, T> ToDictionary<T>(this IntMap intMap, Func<dynamic, T> valueConverter)
        {
            ValidationHelper.NotNull(intMap, nameof(intMap));
            ValidationHelper.NotNull(valueConverter, nameof(valueConverter));

            var dictionary = new Dictionary<int, T>();
            dynamic keys = intMap.keys();
            for (int i = 0; i < keys.length; i++)
            {
                var key = keys.getDyn(i);
                var value = intMap.get(key);
                if (value != null)
                {
                    dictionary[key] = valueConverter(value);
                }
            }
            return dictionary;
        }


        public static IntMap ToIntMap<T>(this Dictionary<int, T> dictionary, Func<T, dynamic> valueConverter)
        {
            ValidationHelper.NotNull(dictionary, nameof(dictionary));
            ValidationHelper.NotNull(valueConverter, nameof(valueConverter));

            var intMap = new IntMap();
            foreach (var kvp in dictionary)
            {
                intMap.set(kvp.Key, valueConverter(kvp.Value));
            }
            return intMap;
        }
    }
}