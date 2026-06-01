using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.tool.atk;
using ModCore.Storage;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    public abstract class IPopDamage
    {
        public string Id { get; }

        protected IPopDamage(string id)
        {
            Id = id;
        }

        /// <summary>
        /// 设置中的描述
        /// </summary>
        public abstract string OptionsTitle { get; }

        /// <summary>
        /// 设置中的子描述
        /// </summary>
        public abstract string SubStr { get; }

        /// <summary>
        /// 配置项，决定是否启用此伤害显示处理器
        /// </summary>
        public virtual PopConfig popconfig { get; } = EntityPopDamage.popconfig;

        /// <summary>
        /// 优先级，数值越小越先匹配。
        /// </summary>
        public abstract int Priority { get; }

        /// <summary>
        /// 速度倍率，数值越大伤害数字消失越慢。
        /// </summary>
        public abstract double SpeedMultiplier { get; set; }

        /// <summary>
        /// 判断当前攻击是否由此负责显示
        /// </summary>
        public abstract bool CanHandle(AttackData a, Entity entity);

        /// <summary>
        /// 创建并显示伤害数字
        /// </summary>
        public abstract void CreatePopDamage(AttackData a, Entity entity);
    }
}