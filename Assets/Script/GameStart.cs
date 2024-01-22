using Assets.Script.Events;
using Assets.Script.Module.GameMod;
using Assets.Script.Utils;
using GameFramework.Module;
using GameFramework.utils;
using System;
using UnityEngine;
using UnityEngine.AI;

class GameStart : MonoBehaviour, IGameStartGlobalEvent
{
    public event Action OnDrawGizmosCall;


    // Start is called before the first frame update
    void Start()
    {
        GameEvents.GameGlobalEvent = this;
        //NavMesh mesh;
        //UnityEngine.AI.NavMeshAgent ag;
        //NavMeshPath path;
        //ag.CalculatePath(Vector3.zero, path);
        //Vector3[] vs;
        //path.GetCornersNonAlloc(vs);//����·��
        //ag.GetNearestPointOnMesh;
        //ag.IsPositionBlocked;
        //NavMeshBuilder b;b.


        //ag.SetDestination();//�ƶ���Ŀ���
        //ag.Stop();
        //ag.Resume();//�ָ��ƶ�
        //ag.Warp();//˲��
        //ag.path.corners;//�ƶ�·��
        //ag.destination = Vector3.positiveInfinity;//����Ŀ���
        //ag.Move(ag.desiredVelocity * Time.deltaTime);



        GDebug.debugAction = (string msg, int lv) =>
        {
            GDebug.ELogLevel glv = (GDebug.ELogLevel)lv;
            switch (glv)
            {
                case GDebug.ELogLevel.Warning: Debug.LogWarning(msg); break;
                case GDebug.ELogLevel.Error: Debug.LogError(msg); break;
                default:
                case GDebug.ELogLevel.Info: Debug.Log(msg); break;
            }
        };
        

        ModuleManager.Register<GameModModule>();



        ModuleManager.GetModule<GameModModule>().SwitchGameMod(EGameModType.Hall);


    }

    // Update is called once per frame
    void Update()
    {
        TimeUtil.Update();
    }

    private void OnDestroy()
    {
        ModuleManager.ShutDown();
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        OnDrawGizmosCall?.Invoke();
    }
#endif
}
