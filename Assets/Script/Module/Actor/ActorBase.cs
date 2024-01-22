
using System.Collections.Generic;
using Assets.Script.Module.Map;
using GameFramework;
using GameFramework.Module;
using UnityEngine;
using UnityEngine.AI;
using Vector = UnityEngine.Vector3;

namespace Assets.Script.Module.Actor
{
    public abstract class ActorBase : GBase
    {
        /// <summary>
        /// 游戏模型对象
        /// </summary>
        public abstract GameObject gameObject { get; }
        /// <summary>
        /// 模型资源路径
        /// </summary>
        public string skinAsset
        {
            get { return _skinAsset; }
            set
            {
                _skinAsset = value;
                _SetSkin(value);
            }
        }
        /// <summary>
        /// 当前所在坐标
        /// </summary>
        public Vector position
        {
            get { return _pos; }
            set
            {
                _pos = value;
                _SetPosition(value);
            }
        }
        /// <summary>
        /// 当前旋转
        /// </summary>
        public Quaternion rotation
        {
            get { return _rot; }
            set
            {
                _rot = value;
                _SetRotation(value);
            }
        }

        /// <summary>
        /// 当前所在地图id
        /// </summary>
        public int map { get { return _map; } }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get { return _name; } }
        /// <summary>
        /// 作为目标类型
        /// </summary>
        public ActorTypeMask actorType { get { return _actorType; } }
        /// <summary>
        /// 属性列表
        /// </summary>
        public readonly Dictionary<EActorAttr, ActorAttribute> attrs = new Dictionary<EActorAttr, ActorAttribute>();
        /// <summary>
        /// 一定会有的血量属性
        /// </summary>
        public ActorAttribute hp
        {
            get
            {
                if (!attrs.ContainsKey(EActorAttr.Hp))
                {
                    attrs[EActorAttr.Hp] = new ActorAttribute(this.id, EActorAttr.Hp, 100);
                }
                return attrs[EActorAttr.Hp];
            }
        }

        private string _name;
        protected readonly ActorTypeMask _actorType = new ActorTypeMask(0);
        private string _skinAsset;
        private int _map;
        private Vector _pos;
        private Quaternion _rot;



        protected abstract NavMeshAgent _agent { get; }


        public ActorBase() : base()
        {
            attrs[EActorAttr.Hp] = new ActorAttribute(this.id, EActorAttr.Hp, 100);//默认任何单位都有100hp
        }
        public void ResetData(string name, int map, Vector pos)
        {
            _name = name;
            _map = map;
            skinAsset = "";
            ChangeMap(map, pos);
            Initialize();
        }


        /// <summary>
        /// 通过设置看向点，修改朝向
        /// </summary>
        /// <param name="position"></param>
        public void LookAt(Vector position)
        {
            if (gameObject != null)
            {
                gameObject.transform.LookAt(position);
            }
        }

        /// <summary>
        /// 根据资源路径设置自身显示对象
        /// </summary>
        protected abstract void _SetSkin(string asset);

        private void _SetPosition(Vector pos)
        {
            if (gameObject != null)
            {
                gameObject.transform.position = pos;
            }
            ModuleManager.GetModule<ActorModule>().CallActorMove(id, pos);
        }
        private void _SetRotation(Quaternion rot)
        {
            if (gameObject != null)
            {
                gameObject.transform.rotation = rot;
            }
        }

        /// <summary>
        /// 切换地图
        /// </summary>
        /// <param name="mapid"></param>
        /// <param name="pos"></param>
        public void ChangeMap(int mapid, Vector pos)
        {
            _map = mapid;
            _pos = pos;
            var map = ModuleManager.GetModule<MapModule>().GetMap(mapid);
            if (map != null)
            {
                pos = map.ConverPointToMap(pos);

            }
            _SetPosition(pos);
        }


        /// <summary>
        /// 设置网格显隐
        /// </summary>
        /// <param name="visible"></param>
        public void SetMeshVisible(bool visible)
        {
            if (gameObject == null) { return; }
            var rootmesh = gameObject.GetComponent<Renderer>();
            if (rootmesh) { rootmesh.enabled = visible; }
            var childRenderers = gameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < childRenderers.Length; i++)
            {
                childRenderers[i].enabled = visible;
            }
        }

    }
}