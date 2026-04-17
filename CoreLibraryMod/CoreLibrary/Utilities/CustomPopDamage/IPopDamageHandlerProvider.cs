using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using CoreLibrary.Utilities.CustomPopDamage.PopDamages;
using dc;

namespace CoreLibrary.Utilities.CustomPopDamage
{
    public interface IPopDamageHandlerProvider
    {

        IPopDamageHandler GetHandler(Entity entity);
    }

    public class StaticPopDamageHandlerProvider : IPopDamageHandlerProvider
    {
        public IPopDamageHandler GetHandler(Entity entity) => EntityPopDamage.handler;
    }


    public class ThreadSafePopDamageHandlerProvider : IPopDamageHandlerProvider
    {
        private readonly ConcurrentDictionary<Entity, IPopDamageHandler> _handlerCache = new();

        public IPopDamageHandler GetHandler(Entity entity)
        {
            if (_handlerCache.TryGetValue(entity, out var handler))
            {
                return handler;
            }


            return new DefaultPopDamageHandler();
        }


        public void CacheHandler(Entity entity, IPopDamageHandler handler)
        {
            _handlerCache[entity] = handler;
        }


        public void ClearCache(Entity entity)
        {
            _handlerCache.TryRemove(entity, out _);
        }
    }
}