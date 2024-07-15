using CommonFeatures.FSM;
using CommonFeatures.Resource;
using CommonFeatures.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.GML
{
    /// <summary>
    /// GameMainLoop
    /// 游戏主循环
    /// </summary>
    public class CommonFeature_GML : CommonFeature
    {
        /// <summary>
        /// 游戏主循环状态机
        /// </summary>
        private FSM<CommonFeature_GML> m_FSM;

        public async UniTask StartGame()
        {
            //正式开始游戏
            var states = new FSMState<CommonFeature_GML>[]
            {
                new FSMState_GML_InitializePackage(),
                new FSMState_GML_StartGame(),
                new FSMState_GML_CreatePackageDownloader(),
                new FSMState_GML_DownloadPackageFiles(),
                new FSMState_GML_UpdatePackageManifest(),
                new FSMState_GML_UpdatePackageVersion(),
            };
            m_FSM = CFM.FSM.CreateFSM(states, this);

            //初始化数据
            var blackboard = new GameMainLoopBlackboard();
            var config = CFM.Config.GetConfig<ResourceConfig>();
            blackboard.DefaultBuildPipeline = config.DefaultBuildPipeline;
            blackboard.PackageName = config.PackageName;
            blackboard.PlayMode = config.PlayMode;
            m_FSM.BlackBoard = blackboard;

            //显示过渡界面
            await CFM.UI.GetLayerContainer<UILayerContainer_Base>().ShowUI(UI.EBaseLayerUIType.Splash);

            //显示进度条界面
            await CFM.UI.GetLayerContainer<UILayerContainer_Base>().ShowUI(UI.EBaseLayerUIType.Progress);

            //开始初始化包
            await m_FSM.StartFSM<FSMState_GML_InitializePackage>();
        }
    }
}
