using Assets.Script.Module.GameMod;
using System;

namespace Assets.Script.Events
{
    public interface IGameModEvent
    {
        /// <summary>
        /// 当特定游戏模式开始
        /// </summary>
        event Action<EGameModType> OnGameStart;
        /// <summary>
        /// 当特定游戏模式结束
        /// </summary>
        event Action<EGameModType> OnGameStop;
    }
}
