using Debug = GameFramework.utils.GDebug;

namespace Assets.Script.Module.GameMod.ModLogic
{
    internal class GameMod_RougeLike : ModLogicBase
    {
        public override bool isFinish { get { return false; } }

        public GameMod_RougeLike(GameModModule module, EGameModType modType) : base(module, modType)
        {
            //todo 具体游戏模式逻辑
        }

        public override void Init()
        {
            Debug.LogInfo("rouge init !");
        }

        public override void OnSettlement()
        {
            Debug.LogInfo($"{nameof(GameMod_RougeLike)} settlement!");
        }

        public override void Update()
        {
            ;
        }

    }
}
