using GameFramework.Module;
using UnityEngine;
using Debug = GameFramework.utils.GDebug;
using Assets.Script.Module.Map;
using System;
using System.Collections.Generic;

namespace Assets.Script.Module.GameMod.ModLogic
{
    internal class GameMod_Hall : ModLogicBase
    {
        public override bool isFinish { get { return false; } }
        private bool firstInit = true;
        private int maptaskCount = 0;
        public GameMod_Hall(GameModModule module, EGameModType modType) : base(module, modType)
        {
            maptaskCount = buildMapTasks.Count;
        }



        public override void Init()
        {
            if (firstInit)
            {
                firstInit = false;
                var mapModel = GameObject.Instantiate(Resources.Load<Transform>("Hall"));
                mapModel.position = Vector3.zero;
                //todo 具体游戏模式逻辑
                ModuleManager.GetModule<MapModule>().BuildMap("Hall", mapModel, this.MapCreateTask);
            }
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

        private float MapCreateTask(MapScene map)
        {
            if (buildMapTasks.Count <= 0) { return 1; }
            buildMapTasks.Dequeue().Invoke(map);
            return 1 - (float)buildMapTasks.Count / maptaskCount;
        }

        private readonly Queue<Action<MapScene>> buildMapTasks = new Queue<Action<MapScene>>(new Action<MapScene>[]{
            // map=>{
            //     Debug.LogInfo("a");
            //     Thread.Sleep(1000);
            // }
        });
    }
}
