using GameFramework.utils;

namespace Assets.Script.Module.Actor
{
    /// <summary>
    /// 单位目标类型枚举
    /// </summary>
    public enum EActorTarget
    {
        /// <summary>
        /// 地面的
        /// </summary>
        Ground = 1 << 0,
        /// <summary>
        /// 血肉生命
        /// </summary>
        PhysicalLife = 1 << 1,
        /// <summary>
        /// 机械单位
        /// </summary>
        Mechanical = 1 << 2,
        /// <summary>
        /// 飞行的
        /// </summary>
        Flying = 1 << 3,
        /// <summary>
        /// 悬浮的
        /// </summary>
        Floating = 1 << 4,
        /// <summary>
        /// 能量生命
        /// </summary>
        EnergyLife = 1 << 5,
        /// <summary>
        /// 神圣的
        /// </summary>
        Sacred = 1 << 6,
        /// <summary>
        /// 混沌的
        /// </summary>
        Chaos = 1 << 7,
        /// <summary>
        /// 亡灵
        /// </summary>
        Undead = 1 << 8,
        /// <summary>
        /// 守卫
        /// </summary>
        Guard = 1 << 9,
        /// <summary>
        /// 无敌的
        /// </summary>
        Invincible = 1 << 10,
    }

    //定义攻击规则：攻击目标类型范围必须囊括目标所有所属范围
    /// <summary>
    /// 二进制操作对象
    /// </summary>
    public class ActorTypeMask
    {
        private readonly BitMask mask = new BitMask(0);

        public ActorTypeMask(EActorTarget value)
        {
            mask.Set((int)value);
        }

        public void Set(EActorTarget value)
        {
            mask.Set((int)value);
        }

        public void AddType(EActorTarget type)
        {
            mask.Open((int)type);
        }

        public void RemoveType(EActorTarget type)
        {
            mask.Close((int)type);
        }

        public bool CheckAll(EActorTarget value)
        {
            return mask.TargetIsOpenAll((int)value);
        }

        public bool CheckAny(EActorTarget value)
        {
            return mask.TargetIsOpenAny((int)value);
        }
    }


}
