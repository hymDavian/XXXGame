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



        public override async void Init()
        {
            if (firstInit)
            {
                firstInit = false;
                //todo 具体游戏模式逻辑
                var cfg = new MapMeshCreator.BuildMapConfig();
                cfg.up = 10f; cfg.down = 0f;
                cfg.width = 300;cfg.length = 300;
                cfg.segmentX = 150;cfg.segmentY = 150;
                await ModuleManager.GetModule<MapModule>().BuildMapByTexture("Hall", Resources.Load<Texture2D>("Hall"),new Vector3(0,0,0),cfg, this.MapCreateTask);
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
