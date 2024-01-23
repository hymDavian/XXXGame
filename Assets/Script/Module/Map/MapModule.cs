

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Script.Events;
using Assets.Script.Module.Map;
using GameFramework.Module;
using UnityEngine;

namespace Assets.Script.Module.Map
{
    public class MapModule : ModuleBase, IMapEvent
    {
        public event Action<MapScene> OnMapBuildCompeleted;
        public event Action<MapScene> OnMapUpdate;
        public event Action<int> OnMapClosed;
        public event Action<float> OnMapBuildingProgress;

        public override bool canUpdate { get { return true; } }

        private readonly Dictionary<int, MapScene> _maps = new Dictionary<int, MapScene>();


        public override bool Initialize()
        {
            GameEvents.MapEvent = this;

            return true;
        }

        public override void Update()
        {
            foreach (var map in _maps.Values)
            {
                OnMapUpdate?.Invoke(map);
            }
        }

        public override void Shutdown()
        {
            try
            {
            foreach (var map in _maps.Values)
            {
                CloseMap(map.id);
            }

            }
            catch (Exception e)
            {
                ;
            }
        }

        /// <summary>
        /// 创建地图
        /// </summary>
        /// <param name="name">地图名称</param>
        /// <param name="maproot">地图游戏物体根节点</param>
        /// <param name="createTask">地图创建额外逻辑,实际创建完毕会根据此函数返回1作为创建完成</param>
        /// <returns></returns>
        public async Task<MapScene> BuildMap(string name, Transform maproot, Func<MapScene, float> createTask = null)
        {
            MapScene map = new MapScene(name, maproot);
            while (!map.buildFlag)
            {
                await Task.Run(() => { Debug.Log("wait map..."); });
            }

            float pro = 0;
            if (createTask != null)
            {
                while (pro < 1)
                {
                    await Task.Run(() => { pro = createTask.Invoke(map); });
                    OnMapBuildingProgress?.Invoke(pro);
                    Debug.Log("Map build progress:" + pro);
                }
            }
            else
            {
                OnMapBuildingProgress?.Invoke(1);
            }

            _maps[map.id] = map;
            OnMapBuildCompeleted?.Invoke(map);
            return map;

        }

        /// <summary>
        /// 根据指定纹理图案生成地形
        /// </summary>
        /// <param name="name">地图名称</param>
        /// <param name="texture">纹理资源</param>
        /// <param name="position">生成位置</param>
        /// <returns></returns>
        public async Task<MapScene> BuildMapByTexture(string name, Texture2D texture,Vector3 position, MapMeshCreator.BuildMapConfig buildcfg, Func<MapScene, float> createTask = null)
        {
            Transform mapTra = await MapMeshCreator.CreateMapObject(texture, buildcfg);
            mapTra.position = position;
            MapScene map = await this.BuildMap(name, mapTra,createTask);
            return map;
        }

        /// <summary>
        /// 关闭地图
        /// </summary>
        /// <param name="mapid"></param>
        public void CloseMap(int mapid)
        {
            if (!_maps.ContainsKey(mapid)) { return; }
            _maps[mapid].OnClose();
            _maps.Remove(mapid);
            OnMapClosed?.Invoke(mapid);
        }

        /// <summary>
        /// 获取地图
        /// </summary>
        /// <param name="mapid"></param>
        /// <returns></returns>
        public MapScene GetMap(int mapid)
        {
            if (!_maps.ContainsKey(mapid)) { return null; }
            return _maps[mapid];
        }

    }
}