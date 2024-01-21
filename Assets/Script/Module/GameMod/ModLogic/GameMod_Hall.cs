using Debug = GameFramework.utils.GDebug;

namespace Assets.Script.Module.GameMod.ModLogic
{
    internal class GameMod_Hall : ModLogicBase
    {
        public override bool isFinish { get { return false; } }

        public GameMod_Hall(GameModModule module, EGameModType modType) : base(module, modType)
        {
            //todo 具体游戏模式逻辑
        }

        public override void Init()
        {
            Debug.LogInfo("hall mod !");
        }

        public override void OnSettlement()
        {
            ;
        }

        public override void Update()
        {
            ;//大厅模式不会驱动update
        }
    }
}
