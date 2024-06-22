using CommonFeatures.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ���ڰ�����
    /// </summary>
    internal class GameMainLoopBlackboard : FSMBlackboard
    {
        /// <summary>
        /// ��Ϸ����ģʽ
        /// </summary>
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        /// <summary>
        /// Ĭ�Ϲ�������
        /// </summary>
        public EDefaultBuildPipeline DefaultBuildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

        /// <summary>
        /// ����
        /// </summary>
        public string PackageName = "DefaultPackage";

        /// <summary>
        /// ��Դ����
        /// </summary>
        public ResourcePackage Package;

        /// <summary>
        /// ���汾
        /// </summary>
        public string PackageVersion;

        /// <summary>
        /// ��Դ���ز���
        /// </summary>
        public ResourceDownloaderOperation Downloader;
    }
}
