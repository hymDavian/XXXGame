

using System;
using Assets.Script.Module.Map;
using UnityEngine;

namespace Assets.Script.Events
{
    public interface IMapEvent
    {
        /// <summary>
        /// 地图构建中的进度
        /// </summary>
        event Action<float> OnMapBuildingProgress;

        /// <summary>
        /// 当新地图构建完成
        /// </summary>
        event Action<MapScene> OnMapBuildCompeleted;

        /// <summary>
        /// 地图更新调用
        /// </summary>
        event Action<MapScene> OnMapUpdate;

        /// <summary>
        /// 当地图被关闭以后
        /// </summary>
        event Action<int> OnMapClosed;
    }
}