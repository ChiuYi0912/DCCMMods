using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using CoreLibrary.Utilities.CustomPopDamage.PopDamages;
using dc;

namespace CoreLibrary.Utilities.CustomPopDamage
{
    public interface IPopDamageHandlerProvider
    {

        IPopDamage GetHandler(Entity entity);
    }

    public class StaticPopDamageHandlerProvider : IPopDamageHandlerProvider
    {
        public IPopDamage GetHandler(Entity entity) => EntityPopDamage.handler;
    }


    public class ThreadSafePopDamageHandlerProvider : IPopDamageHandlerProvider
    {
        private readonly ConcurrentDictionary<Entity, IPopDamage> _handlerCache = new();

        public IPopDamage GetHandler(Entity entity)
        {
            if (_handlerCache.TryGetValue(entity, out var handler))
            {
                return handler;
            }


            return new DefaultPopDamageHandler();
        }


        public void CacheHandler(Entity entity, IPopDamage handler)
        {
            _handlerCache[entity] = handler;
        }


        public void ClearCache(Entity entity)
        {
            _handlerCache.TryRemove(entity, out _);
        }
    }
}