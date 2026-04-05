using dc.haxe.ds;
using dc.hl.types;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using CoreLibrary.Core.Utilities;
using Hashlink.Virtuals;
using CoreLibrary.Extensions.Customs;
using System.Collections.Concurrent;

namespace CoreLibrary.Core.Extensions
{

    public static class EnumerableExtensions
    {

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            ValidationHelper.NotNull(source, nameof(source));
            return comparer == null ? new HashSet<T>(source) : new HashSet<T>(source, comparer);
        }


        private static readonly ConditionalWeakTable<ArrayObj, ConcurrentDictionary<string, dynamic>> ParallelCacheTable =new();

        public static ConcurrentDictionary<string, dynamic> GetCachedDictionaryParallel(this ArrayObj arrayObj)
        {
            ValidationHelper.NotNull(arrayObj, nameof(arrayObj));

            return ParallelCacheTable.GetValue(arrayObj, BuildIdDictionaryParallel);
        }

        public static dynamic GetByIdParallel(this ArrayObj arrayObj, string id)
        {
            ValidationHelper.NotNull(arrayObj, nameof(arrayObj));
            ValidationHelper.NotNull(id, nameof(id));

            var dict = arrayObj.GetCachedDictionaryParallel();
            dict.TryGetValue(id, out var result);
            return result!;
        }

        private static ConcurrentDictionary<string, dynamic> BuildIdDictionaryParallel(ArrayObj arrayObj)
        {
            var dict = new ConcurrentDictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);

            Parallel.For(0, arrayObj.length, i =>
            {
                var item = arrayObj.getDyn(i);
                if (item != null && item?.id != null)
                {
                    string id = item!.id.ToString();
                    dict.TryAdd(id, item);
                }
            });
            return dict;
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
            return arrayObj.getDyn(index);
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
            var keys = intMap.keys();
            while (keys.hasNext())
            {
                var key = keys.next();
                var value = intMap.get(key);
                dictionary[key] = valueConverter(value);
            }
            return dictionary;
        }
        

        public static IntMapLiveView<T> AsLiveView<T>(this IntMap map, Func<dynamic, T> converter)
        {
            return new IntMapLiveView<T>(map, converter);
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