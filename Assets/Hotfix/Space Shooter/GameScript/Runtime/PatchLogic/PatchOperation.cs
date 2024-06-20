using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonFeatures.Machine;
using CommonFeatures.Event;
using YooAsset;
using CommonFeatures;

public class PatchOperation : GameAsyncOperation
{
    private enum ESteps
    {
        None,
        Update,
        Done,
    }

    private readonly StateMachine _machine;
    private ESteps _steps = ESteps.None;

    public PatchOperation(string packageName, string buildPipeline, EPlayMode playMode)
    {
        // 注册监听事件
        CommonFeaturesManager.Event.AddListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
        CommonFeaturesManager.Event.AddListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
        CommonFeaturesManager.Event.AddListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
        CommonFeaturesManager.Event.AddListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
        CommonFeaturesManager.Event.AddListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);

        // 创建状态机
        _machine = new StateMachine(this);
        _machine.AddNode<FsmInitializePackage>();
        _machine.AddNode<FsmUpdatePackageVersion>();
        _machine.AddNode<FsmUpdatePackageManifest>();
        _machine.AddNode<FsmCreatePackageDownloader>();
        _machine.AddNode<FsmDownloadPackageFiles>();
        _machine.AddNode<FsmDownloadPackageOver>();
        _machine.AddNode<FsmClearPackageCache>();
        _machine.AddNode<FsmUpdaterDone>();

        _machine.SetBlackboardValue("PackageName", packageName);
        _machine.SetBlackboardValue("PlayMode", playMode);
        _machine.SetBlackboardValue("BuildPipeline", buildPipeline);
    }
    protected override void OnStart()
    {
        _steps = ESteps.Update;
        _machine.Run<FsmInitializePackage>();
    }
    protected override void OnUpdate()
    {
        if (_steps == ESteps.None || _steps == ESteps.Done)
            return;

        if(_steps == ESteps.Update)
        {
            _machine.Update();
            if(_machine.CurrentNode == typeof(FsmUpdaterDone).FullName)
            {
                CommonFeaturesManager.Event.RemoveListener<UserEventDefine.UserTryInitialize>(OnHandleEventMessage);
                CommonFeaturesManager.Event.RemoveListener<UserEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
                CommonFeaturesManager.Event.RemoveListener<UserEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
                CommonFeaturesManager.Event.RemoveListener<UserEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
                CommonFeaturesManager.Event.RemoveListener<UserEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);
                Status = EOperationStatus.Succeed;
                _steps = ESteps.Done;
            }
        }
    }
    protected override void OnAbort()
    {
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is UserEventDefine.UserTryInitialize)
        {
            _machine.ChangeState<FsmInitializePackage>();
        }
        else if (message is UserEventDefine.UserBeginDownloadWebFiles)
        {
            _machine.ChangeState<FsmDownloadPackageFiles>();
        }
        else if (message is UserEventDefine.UserTryUpdatePackageVersion)
        {
            _machine.ChangeState<FsmUpdatePackageVersion>();
        }
        else if (message is UserEventDefine.UserTryUpdatePatchManifest)
        {
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
        else if (message is UserEventDefine.UserTryDownloadWebFiles)
        {
            _machine.ChangeState<FsmCreatePackageDownloader>();
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }
}