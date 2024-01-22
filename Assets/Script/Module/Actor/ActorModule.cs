

using System;
using System.Collections.Generic;
using Assets.Script.Events;
using GameFramework.Module;
using UnityEngine;

namespace Assets.Script.Module.Actor
{
    internal class ActorModule : ModuleBase, IActorEvents
    {
        public override bool canUpdate { get { return true; } }
        public event Action<ActorBase> OnActorDestory;
        public event Action<ActorBase> OnActorSpawn;
        public event Action<int, EActorAttr, float> OnActorAttrChange;
        public event Action<int, Vector3> OnActorMove;

        private readonly Dictionary<int, ActorBase> _activeActors = new Dictionary<int, ActorBase>();
        private readonly Dictionary<string, Queue<ActorBase>> _actorPool = new Dictionary<string, Queue<ActorBase>>();


        public override bool Initialize()
        {
            GameEvents.ActorEvent = this;

            GameEvents.MapEvent.OnMapBuildCompeleted += map =>
            {
                SpawnActor<SceneObsActor>("测试阻挡单位", map.id, map.GetRandomPoint()).skinAsset = "cube";
            };

            return true;
        }

        public override void Update()
        {
            foreach (var actor in _activeActors.Values)
            {
                actor.Update();
            }
        }

        public override void Shutdown()
        {
            try
            {
                foreach (var actor in _activeActors.Values)
                {
                    DestoryActor(actor.id);
                }

            }
            catch (System.Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 移除单位
        /// </summary>
        /// <param name="id"></param>
        public void DestoryActor(int id)
        {
            if (_activeActors.ContainsKey(id))
            {
                var actor = _activeActors[id];
                _activeActors.Remove(id);
                actor.Shutdown();

                string typeKey = actor.GetType().FullName;
                if (_actorPool.TryGetValue(typeKey, out var pool))
                {
                    pool.Enqueue(actor);
                }
                else
                {
                    pool = new Queue<ActorBase>();
                    pool.Enqueue(actor);
                    _actorPool[typeKey] = pool;
                }
                OnActorDestory?.Invoke(actor);
            }
        }

        /// <summary>
        /// 生成单位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="typeMask"></param>
        /// <param name="pos"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public T SpawnActor<T>(string name, int map, Vector3 pos)
            where T : ActorBase, new()
        {
            ActorBase ret;
            string typeKey = typeof(T).FullName;
            if (_actorPool.TryGetValue(typeKey, out var pool) && pool.Count > 0)
            {
                ret = pool.Dequeue();
            }
            else
            {
                ret = new T();
            }
            ret.ResetData(name, map, pos);
            _activeActors[ret.id] = ret;
            OnActorSpawn?.Invoke(ret);
            return ret as T;
        }

        /// <summary>
        /// 获取还存在的单位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActorBase GetActorByID(int id)
        {
            if (_activeActors.TryGetValue(id, out var actor)) return actor;
            return null;
        }

        public void CallActorAttrChange(int id, EActorAttr ty, float val)
        {
            OnActorAttrChange?.Invoke(id, ty, val);
        }

        public void CallActorMove(int id, Vector3 pos)
        {
            OnActorMove?.Invoke(id, pos);
        }
    }
}