using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Script.Module.Map
{
    /// <summary>
    /// 动态地图对象
    /// </summary>
    public class MapScene
    {
        private static int _idseed = 0;
        private static NavMeshBuildSettings _settings;
        static MapScene()
        {
            _settings = NavMesh.GetSettingsByID(0);//所有的寻路烘焙都以首个代理数据大小为准
        }

        /// <summary>
        /// 地图名称
        /// </summary>
        public readonly string name;
        /// <summary>
        /// 地图唯一实例ID
        /// </summary>
        public readonly int id;
        /// <summary>
        /// 当前创建地图状态
        /// </summary>
        public bool buildFlag { get { return this._buildFlag; } }

        private readonly NavMeshData _navMeshData = null;//地图寻路数据
        private readonly NavMeshDataInstance _dataIns;
        private readonly Transform _rootTra = null;
        private Bounds _bounds = new Bounds();
        private bool _buildFlag = false;
        private Dictionary<int, NavMeshObstacle> _obsDic = new Dictionary<int, NavMeshObstacle>();
        private readonly NavMeshPath _path;

        private NavMeshTriangulation? _navMeshTriData;

        public MapScene(string name, Transform mapObjRootTra)
        {

            this.id = ++_idseed;
            this.name = name;
            if (mapObjRootTra == null) { return; }
            _path = new NavMeshPath();
            this._rootTra = mapObjRootTra;
            _navMeshData = new NavMeshData();
            _dataIns = NavMesh.AddNavMeshData(_navMeshData, mapObjRootTra.position, mapObjRootTra.rotation);
            _BuildNavmesh();
        }

        //刷新地图寻路数据
        private void _RefreshAllBounds()
        {
            _bounds.SetMinMax(Vector3.one, Vector3.zero);//重置
            _bounds.center = _rootTra.position;
            var rootRender = _rootTra.GetComponent<Renderer>();
            if (rootRender != null)
            {
                _bounds.Encapsulate(rootRender.bounds);
            }

            var childRenderers = _rootTra.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < childRenderers.Length; i++)
            {
                _bounds.Encapsulate(childRenderers[i].bounds);
            }
        }
        private void _BuildNavmesh()
        {
            _buildFlag = false;
            if (_navMeshData == null)
            {
                _buildFlag = true;
                return;
            }
            _RefreshAllBounds();
            List<NavMeshBuildSource> colllist = new List<NavMeshBuildSource>();
            List<NavMeshBuildMarkup> temp = new List<NavMeshBuildMarkup>();
            NavMeshBuilder.CollectSources(_bounds, LayerMask.NameToLayer(EMapLayer.MapGround), NavMeshCollectGeometry.RenderMeshes, 0, temp, colllist);
            NavMeshBuilder.UpdateNavMeshDataAsync(_navMeshData, _settings, colllist, _bounds).completed += (op) =>
            {
                _navMeshTriData = NavMesh.CalculateTriangulation();
                _buildFlag = true;
            };
        }

        /// <summary>
        /// 添加新物体作为地图固定物体到场上,并重新计算寻路区域
        /// </summary>
        /// <param name="obj"></param>
        public void AddToMapMesh(Transform obj)
        {
            if (obj == null || _rootTra == null) { return; }
            obj.SetParent(_rootTra);
            _BuildNavmesh();
        }


        /// <summary>
        /// 更新阻挡物体到地图 (不会跟随移动)
        /// </summary>
        /// <param name="obj"></param>
        public void SetObstacle(Transform obj)
        {
            int id = obj.GetInstanceID();
            NavMeshObstacle obs;
            if (!this._obsDic.ContainsKey(id))
            {
                obs = new GameObject("TempObs").AddComponent<NavMeshObstacle>();
                obs.transform.rotation = obj.rotation;
                obs.transform.position = obj.position;
                var bounds = obj.GetDeepBounds();
                obs.shape = NavMeshObstacleShape.Box;
                obs.size = bounds.size;
                obs.carving = true;
                obs.carveOnlyStationary = true;
                obs.carvingMoveThreshold = 0.1f;
                obs.carvingTimeToStationary = 1f;
                this._obsDic.Add(id, obs);
            }
            else
            {
                obs = this._obsDic[id];
                obs.transform.rotation = obj.rotation;
                obs.transform.position = obj.position;
                obs.size = obj.GetDeepBounds().size;
            }
        }

        /// <summary>
        /// 移除属于指定物体的阻挡物逻辑
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObstacle(Transform obj)
        {
            int id = obj.GetInstanceID();
            if (_obsDic.ContainsKey(id))
            {
                var obs = _obsDic[id];
                _obsDic.Remove(id);
                GameObject.Destroy(obs.gameObject);
            }
        }

        /// <summary>
        /// 获取两点间的路径角点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public Vector3[] GetPath(Vector3 p1, Vector3 p2, int mask = -1)
        {


            var cal = NavMesh.CalculatePath(p1, p2, mask, _path);
            if (!cal)
            {
                bool blocked = NavMesh.Raycast(p1, p2, out var hit, mask);
                if (blocked)//中间有不可通过的阻挡
                {
                    p2 = hit.position;//阻挡的最终点
                }
                cal = NavMesh.CalculatePath(p1, p2, mask, _path);//再次尝试查找路线
                if (!cal)//如果还是没有路
                {
                    return new Vector3[0];
                }
            }

            if (_path.status != NavMeshPathStatus.PathComplete) { return new Vector3[0]; }
            _path.ClearCorners();
            return _path.corners;
        }


        /// <summary>
        /// 获取导航网格上完全随机的一个点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRandomPoint()
        {
            if (_navMeshTriData == null) { return Vector3.zero; }
            var tri = _navMeshTriData.Value;
            int t = Random.Range(0, tri.indices.Length - 3);
            Vector3 point = Vector3.Lerp(tri.vertices[tri.indices[t]], tri.vertices[tri.indices[t + 1]], Random.value);
            point = Vector3.Lerp(point, tri.vertices[tri.indices[t + 2]], Random.value);

            return point;
        }

        /// <summary>
        /// 获取以指定中心点，指定半径范围内的随机点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Vector3 GetRandomPointByCenter(Vector3 center, float range)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 rd = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(rd, out hit, 1, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 转化指定点为导航点
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector3 ConverPointToMap(Vector3 pos)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos, out hit, 100, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return pos;
        }



        public void OnClose()
        {
            NavMesh.RemoveNavMeshData(_dataIns);
        }
    }
}
