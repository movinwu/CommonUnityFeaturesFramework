using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonFeatures.Event;
using YooAsset;
using CommonFeatures;

public class GameManager
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameManager();
            return _instance;
        }
    }

    /// <summary>
    /// 协程启动器
    /// </summary>
    public MonoBehaviour Behaviour;


    private GameManager()
    {
        // 注册监听事件
        CommonFeaturesManager.Event.AddListener<SceneEventDefine.ChangeToHomeScene>(OnHandleEventMessage);
        CommonFeaturesManager.Event.AddListener<SceneEventDefine.ChangeToBattleScene>(OnHandleEventMessage);
    }

    /// <summary>
    /// 开启一个协程
    /// </summary>
    public void StartCoroutine(IEnumerator enumerator)
    {
        Behaviour.StartCoroutine(enumerator);
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is SceneEventDefine.ChangeToHomeScene)
        {
            YooAssets.LoadSceneAsync("scene_home");
        }
        else if (message is SceneEventDefine.ChangeToBattleScene)
        {
            YooAssets.LoadSceneAsync("scene_battle");
        }
    }
}