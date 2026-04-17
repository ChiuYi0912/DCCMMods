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
        private static readonly List<IPopDamageHandler> Handlers = new List<IPopDamageHandler>();

        /// <summary>
        /// 注册自定义处理器。
        /// </summary>
        public static void Register(IPopDamageHandler handler)
        {
            if (!Handlers.Contains(handler))
            {
                Handlers.Add(handler);
                Handlers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
        }


        public static void Unregister(IPopDamageHandler handler)
        {
            Handlers.Remove(handler);
        }


        public static IPopDamageHandler GetHandler(AttackData a, Entity entity)
        {
            return Handlers.FirstOrDefault(h => h.CanHandle(a, entity))!;
        }
    }
}