using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.tool.atk;

namespace CoreLibrary.Utilities.CustomPopDamage
{
    public class PopDamageHandlerRegistry
    {
        private static readonly List<IPopDamage> Handlers = new List<IPopDamage>();

        /// <summary>
        /// 注册自定义处理器。
        /// </summary>
        public static void Register(IPopDamage handler)
        {
            if (!Handlers.Contains(handler))
            {
                Handlers.Add(handler);
                Handlers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
        }


        public static void Unregister(IPopDamage handler)
        {
            Handlers.Remove(handler);
        }


        public static IPopDamage GetHandler(AttackData a, Entity entity)
        {
            return Handlers.FirstOrDefault(h => h.CanHandle(a, entity))!;
        }
    }
}