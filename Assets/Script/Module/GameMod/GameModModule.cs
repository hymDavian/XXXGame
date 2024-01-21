using Assets.Script.Events;
using Assets.Script.Module.GameMod.ModLogic;
using GameFramework.Module;
using System;
using System.Collections.Generic;
using Debug = GameFramework.utils.GDebug;

namespace Assets.Script.Module.GameMod
{
    public class GameModModule : ModuleBase, IGameModEvent
    {

        public event Action<EGameModType> OnGameStart;
        public event Action<EGameModType> OnGameStop;
        public override bool canUpdate { get { return true; } }
        /// <summary>
        /// 当前模式
        /// </summary>
        public EGameModType currentMod { get { return this._cueMod; } }

        private EGameModType _cueMod = EGameModType.None;//当前模式
        private readonly Dictionary<EGameModType, ModLogicBase> _moddic = new Dictionary<EGameModType, ModLogicBase>();


        public override bool Initialize()
        {
            GameEvents.GameModEvent = this;
            _moddic.Add(EGameModType.RougeLike, new GameMod_RougeLike(this, EGameModType.RougeLike));
            _moddic.Add(EGameModType.Hall, new GameMod_Hall(this, EGameModType.Hall));
            Debug.LogInfo_Date($"{nameof(GameModModule)} init.");
            return true;
        }


        public override void Update()
        {
            if (_cueMod != EGameModType.Hall)
            {
                _moddic[_cueMod].Update();
                if (_moddic[_cueMod].isFinish)
                {
                    SwitchGameMod(EGameModType.Hall);
                }
            }
        }

        public override void Shutdown()
        {
            this.EndGameMod();
        }

        /// <summary>
        /// 切换位指定模式
        /// </summary>
        /// <param name="mod"></param>
        public void SwitchGameMod(EGameModType mod)
        {
            if (_cueMod == mod) //处于特殊模式
            {
                Debug.LogWarning_Date($"已经在{Enum.GetName(typeof(EGameModType), _cueMod)}模式了!");
                return;
            }
            EndGameMod();
            _cueMod = mod;
            _moddic[_cueMod].Init();
            OnGameStart?.Invoke(mod);
        }
        /// <summary>
        /// 结束当前模式
        /// </summary>
        private void EndGameMod()
        {
            if (_moddic.ContainsKey(_cueMod))
            {
                _moddic[_cueMod].OnSettlement();
            }
            OnGameStop?.Invoke(_cueMod);
        }
    }

}
