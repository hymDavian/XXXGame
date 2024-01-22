using System;
using Assets.Script.Module.Actor;
using UnityEngine;

namespace Assets.Script.Events
{
    public interface IActorEvents
    {
        /// <summary>
        /// 当任意单位被销毁
        /// </summary>
        event Action<ActorBase> OnActorDestory;
        /// <summary>
        /// 当任意单位被生成
        /// </summary>
        event Action<ActorBase> OnActorSpawn;
        /// <summary>
        /// 单位的某个属性发生变更
        /// </summary>
        event Action<int, EActorAttr, float> OnActorAttrChange;
        /// <summary>
        /// 单位移动到新的位置了
        /// </summary>
        event Action<int, Vector3> OnActorMove;
    }
}
