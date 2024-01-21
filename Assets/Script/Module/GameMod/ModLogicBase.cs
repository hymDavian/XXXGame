namespace Assets.Script.Module.GameMod
{
    public abstract class ModLogicBase
    {
        /// <summary>
        /// 是否应该结束了
        /// </summary>
        public abstract bool isFinish { get; }

        protected readonly GameModModule module;
        protected readonly EGameModType modType;
        public ModLogicBase(GameModModule module, EGameModType modType)
        {
            this.module = module;
            this.modType = modType;
        }
        /// <summary>
        /// 每次进入此模式后的初始化
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 更新驱动
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// 结算并结束
        /// </summary>
        public abstract void OnSettlement();
    }
}
