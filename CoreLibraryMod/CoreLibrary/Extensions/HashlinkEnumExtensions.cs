using Hashlink.Marshaling;
using Hashlink.Proxy;
using Hashlink.Proxy.Values;
using Hashlink.Reflection;
using Hashlink.Reflection.Types;
using HaxeProxy.Runtime;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CoreLibrary.Extensions
{
    public static class HashlinkEnumExtensions
    {
        private static readonly ConstructorInfo baseCtor = typeof(HaxeProxyBase)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .First();

        private static readonly ConcurrentDictionary<Type, HashlinkEnumType> typeCache = new();

        private static HashlinkEnumType GetEnumType( Type parentType )
        {
            return typeCache.GetOrAdd(parentType, _ =>
            {
                var nested = parentType.GetNestedTypes()
                    .First(t => t.IsAssignableTo(typeof(HaxeProxyBase)));
                var dummy = (HaxeProxyBase)Activator.CreateInstance(nested)!;
                return (HashlinkEnumType)dummy.HashlinkObj.Type;
            });
        }

        private static nint? cachedSingletonPtr;
        private static HashlinkEnumType? cachedEnumType;

        public static T GetSingleton<T>() where T : HaxeEnum
        {
            var csType = typeof(T);
            var parentType = csType.IsNested ? csType.DeclaringType! : csType;
            var enumType = GetEnumType(parentType);

            nint singletonPtr;
            if (cachedEnumType == enumType && cachedSingletonPtr.HasValue)
            {
                singletonPtr = cachedSingletonPtr.Value;
            }
            else
            {
                singletonPtr = FindGlobalPtr(enumType);
                cachedEnumType = enumType;
                cachedSingletonPtr = singletonPtr;
            }

            var hlEnum = new HashlinkEnum(HashlinkObjPtr.Get(singletonPtr));
            var proxy = (T)RuntimeHelpers.GetUninitializedObject(csType);
            baseCtor.Invoke(proxy, [hlEnum]);
            return proxy;
        }

        private static nint FindGlobalPtr( HashlinkEnumType enumType )
        {
            foreach (var g in HashlinkMarshal.Module.Globals)
            {
                if (g.Type == enumType && g.Value is HashlinkEnum hlEnum)
                    return hlEnum.HashlinkPointer;
            }
            throw new InvalidOperationException($"No global found for {enumType.Name}");
        }
    }
}
