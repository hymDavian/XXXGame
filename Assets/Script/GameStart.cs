using Assets.Script.Events;
using Assets.Script.Module.Actor;
using Assets.Script.Module.GameMod;
using Assets.Script.Module.Map;
using Assets.Script.Utils;
using GameFramework.Module;
using GameFramework.utils;
using System;
using UnityEngine;
using UnityEngine.AI;

class GameStart : MonoBehaviour, IGameStartGlobalEvent
{
    public event Action OnDrawGizmosCall;

    public Texture2D texture;

    private void Awake()
    {
        UnityEngine.Analytics.Analytics.enabled = false;
        UnityEngine.Analytics.Analytics.deviceStatsEnabled = false;
        UnityEngine.Analytics.Analytics.initializeOnStartup = false;
        UnityEngine.Analytics.Analytics.limitUserTracking = false;
        UnityEngine.Analytics.PerformanceReporting.enabled = false;

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
    }



    // Start is called before the first frame update
    void Start()
    {
        GameEvents.GameGlobalEvent = this;

        ModuleManager.Register<GameModModule>();//游戏模式
        ModuleManager.Register<MapModule>();//地图模块
        ModuleManager.Register<ActorModule>();//单位模块

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
