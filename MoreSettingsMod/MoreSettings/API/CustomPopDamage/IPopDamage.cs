using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.tool.atk;
using ModCore.Storage;
using MoreSettings.GameMechanics.CustomPopDamage;

namespace MoreSettings.API
{
    public abstract class IPopDamage
    {
        public string Id { get; }

        /// <summary>
        /// 速度倍率，数值越大伤害数字消失越慢。
        /// </summary>
        public double SpeedMultiplier
        {
            get => EntityPopDamage.popconfig.DATA[Id].SpeedMultiplier;
            set
            {
                var DATA = EntityPopDamage.popconfig.DATA;
                if (!DATA.ContainsKey(Id)) return;
                DATA[Id].SpeedMultiplier = value;
                EntityPopDamage.Config.Save();
            }
        }

        public PopDamageData damageData
        {
            get => EntityPopDamage.popconfig.DATA[Id];
        }

        /// <summary>
        /// 实例化属性
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <param name="Speed">逐渐消失的速度</param>
        protected IPopDamage(string id, double Speed)
        {
            Id = id;
            if (EntityPopDamage.popconfig.DATA.ContainsKey(id))
                return;

            var data = new PopDamageData();
            data.SpeedMultiplier = Speed;
            EntityPopDamage.popconfig.DATA.Add(id, data);
        }


        /// <summary>
        /// 如没特殊条件
        /// 可在CanHandle方法判断,当前玩家是否在设置选择该伤害显示
        /// </summary>
        /// <returns>bool</returns>
        public bool Current() => EntityPopDamage.ForcedHandler == this;


        /// <summary>
        /// 设置中的描述
        /// </summary>
        public abstract string OptionsTitle { get; }


        /// <summary>
        /// 设置中的子描述
        /// </summary>
        public abstract string SubStr { get; }


        /// <summary>
        /// 优先级，数值越小越先匹配。
        /// </summary>
        public abstract int Priority { get; }


        /// <summary>
        /// 判断当前攻击是否由此负责显示
        /// <param name="a">攻击数据</param>
        /// <param name="entity">被击中的实体</param>
        /// <returns>bool</returns>
        public abstract bool CanHandle(AttackData a, Entity entity);


        /// <summary>
        /// 用于实例化并显示伤害数字
        /// </summary>
        /// <param name="a"></param>
        /// <param name="entity"></param>
        public abstract void CreatePopDamage(AttackData a, Entity entity);
    }


    public class PopDamageData()
    {
        public double SpeedMultiplier { get; set; } = 0.5;

        /// <summary>
        /// 不加入设置界面
        /// </summary>
        public bool unique = false;
    }
}