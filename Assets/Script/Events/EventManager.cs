namespace Assets.Script.Events
{
    public sealed class GameEvents
    {
        /// <summary>
        /// 全局游戏事件
        /// </summary>
        public static IGameStartGlobalEvent GameGlobalEvent { get; set; }
        /// <summary>
        /// 游戏模式事件
        /// </summary>
        public static IGameModEvent GameModEvent { get; set; }

        /// <summary>
        /// 地图事件
        /// </summary>
        public static IMapEvent MapEvent { get; set; }

        /// <summary>
        /// 单位事件
        /// </summary>
        public static IActorEvents ActorEvent { get; set; }



    }
}
