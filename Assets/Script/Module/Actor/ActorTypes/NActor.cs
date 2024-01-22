

using System;
using Assets.Script.Module.Map;
using GameFramework;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.Module.Actor
{
    /// <summary>
    /// 一般活动单位
    /// </summary>
    public class NActor : ActorBase
    {
        public readonly ActorAttribute baseDmg;

        public readonly ActorAttribute moveSpeed;

        protected readonly GameObject _root;
        private NavMeshAgent _agent_n = null;
        public NActor() : base()
        {
            _root = new GameObject($"NActor_[{id}]");
            _agent_n = _root.AddComponent<NavMeshAgent>();
            _agent_n.stoppingDistance = 0.5f;
            _agent_n.updatePosition = false;
            _agent_n.updateRotation = true;
            _agent.autoRepath = true;
            baseDmg = attrs[EActorAttr.Damage] = new ActorAttribute(id, EActorAttr.Damage, 10);//基础伤害值
            moveSpeed = attrs[EActorAttr.MoveSpeed] = new ActorAttribute(id, EActorAttr.MoveSpeed, 2.5f, (0.1f, 100f));
        }

        public override GameObject gameObject { get { return _root; } }

        protected override NavMeshAgent _agent { get { return _agent_n; } }

        public override bool Initialize()
        {
            hp.Reset(500, (0, 500));//设置一般单位的血量有上限值500

            return true;
        }

        public override void Shutdown()
        {
            DestoryModel();
            beforeRes = null;
        }

        public override void Update()
        {
            hp.AddBase(1.0f * Time.deltaTime);//普通单位每秒回血1点

            if (_agent.isStopped) { return; }
            if (_agent.pathPending) { return; }
            if (_agent.remainingDistance <= _agent.stoppingDistance)//到达指定距离位置了
            {
                StopMove();
            }
            else
            {
                _agent.speed = moveSpeed.value;
                _agent.acceleration = Vector3.Angle(_root.transform.forward, _agent.steeringTarget - this.position) * _agent.speed;
                position = _agent.nextPosition;
            }
        }

        /// <summary>
        /// 设置出生配置信息
        /// </summary>
        /// <param name="cfgid"></param>
        public void SetBirthConfig(int cfgid)
        {
            //todo 根据配置，修改显示模型，修改伤害，血量，目标类型。。。
        }

        private Action _stopCallback = null;
        public bool MoveTo(Vector3 pos, float mindis, Action stopCallback = null)
        {
            _agent.speed = moveSpeed.value;
            _agent.stoppingDistance = mindis.Clamp(0.1f, mindis);
            _stopCallback = stopCallback;
            if (!_agent.SetDestination(pos))
            {
                StopMove(false);
                return false;
            }

            _agent.isStopped = false;
            return true;
            // _agent.SetPath()
        }
        public void StopMove(bool callStop = true)
        {
            if (callStop && _stopCallback != null)
            {
                _stopCallback.Invoke();

            }
            _stopCallback = null;
            // _agent.ResetPath
            _agent.isStopped = true;
            _agent.ResetPath();

        }


        private GameObject _g;
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
                _g.transform.SetParent(_root.transform);
                _g.transform.localPosition = Vector3.zero;
                _g.transform.localRotation = Quaternion.identity;
                _g.transform.localScale = Vector3.one;
            }
        }
        private void DestoryModel()
        {
            if (_g != null)
            {
                _g.transform.SetParent(null);
                GameObject.Destroy(_g);
                _g = null;
            }
        }
    }
}