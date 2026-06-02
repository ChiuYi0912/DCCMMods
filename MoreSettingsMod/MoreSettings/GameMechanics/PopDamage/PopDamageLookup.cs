using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using dc;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    internal interface IPopDamageLookup
    {
        IPopDamage GetHandler(Entity entity);
    }

    internal class StaticPopDamageLookup : IPopDamageLookup
    {
        public IPopDamage GetHandler(Entity entity) => EntityPopDamage.handler;
    }


    internal class ThreadSafePopDamageLookup : IPopDamageLookup
    {
        private readonly ConcurrentDictionary<Entity, IPopDamage> _handlerCache = new();

        public IPopDamage ForcedHandler = null!;

        public IPopDamage GetHandler(Entity entity)
        {
            if (ForcedHandler != null)
                return ForcedHandler;

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