


using Assets.Script.Module.Map;
using GameFramework.Module;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Module.Actor
{
    /// <summary>
    /// 场上的固定可破坏物体摆件
    /// </summary>
    public class SceneObsActor : ActorBase
    {


        protected override NavMeshAgent _agent { get { return null; } }

        public override bool Initialize()
        {
            this._actorType.Set(EActorTarget.Ground);
            return true;
        }

        public override void Shutdown()
        {
            DestoryModel();
            this.beforeRes = null;
        }

        public override void Update()
        {
            ;
        }

        private GameObject _g;
        public override GameObject gameObject { get { return _g; } }
        private string beforeRes = null;
        protected override void _SetSkin(string asset)
        {
            if (beforeRes == asset) { return; }
            beforeRes = asset;
            DestoryModel();
            var res = Resources.Load<GameObject>(asset);
            if (res != null)
            {
                _g = GameObject.Instantiate(res);
                _g.transform.position = this.position;
                _g.transform.rotation = this.rotation;
                var map = ModuleManager.GetModule<MapModule>().GetMap(this.map);
                if (map != null)
                {
                    map.SetObstacle(_g.transform);
                }
            }
        }

        private void DestoryModel()
        {
            if (_g != null)
            {
                var map = ModuleManager.GetModule<MapModule>().GetMap(this.map);
                if (map != null)
                {
                    map.RemoveObstacle(_g.transform);
                }
                GameObject.Destroy(_g);
                _g = null;
            }

        }
    }
}