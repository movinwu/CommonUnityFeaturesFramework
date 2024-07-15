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
    /// ��Ϸ��ѭ��
    /// </summary>
    public class CommonFeature_GML : CommonFeature
    {
        /// <summary>
        /// ��Ϸ��ѭ��״̬��
        /// </summary>
        private FSM<CommonFeature_GML> m_FSM;

        public async UniTask StartGame()
        {
            //��ʽ��ʼ��Ϸ
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

            //��ʼ������
            var blackboard = new GameMainLoopBlackboard();
            var config = CFM.Config.GetConfig<ResourceConfig>();
            blackboard.DefaultBuildPipeline = config.DefaultBuildPipeline;
            blackboard.PackageName = config.PackageName;
            blackboard.PlayMode = config.PlayMode;
            m_FSM.BlackBoard = blackboard;

            //��ʾ���ɽ���
            await CFM.UI.GetLayerContainer<UILayerContainer_Base>().ShowUI(UI.EBaseLayerUIType.Splash);

            //��ʾ����������
            await CFM.UI.GetLayerContainer<UILayerContainer_Base>().ShowUI(UI.EBaseLayerUIType.Progress);

            //��ʼ��ʼ����
            await m_FSM.StartFSM<FSMState_GML_InitializePackage>();
        }
    }
}
