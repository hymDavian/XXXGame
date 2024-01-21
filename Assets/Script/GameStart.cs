using Assets.Script.Module.GameMod;
using GameFramework.Module;
using GameFramework.utils;
using UnityEngine;

class GameStart : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

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

        ModuleManager.Register<GameModModule>();//ע����Ϸģʽģ��



        ModuleManager.GetModule<GameModModule>().SwitchGameMod(EGameModType.Hall);//��ʼ���������

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
}
