using CommonFeatures.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// 游戏主循环黑板数据
    /// </summary>
    internal class GameMainLoopBlackboard : FSMBlackboard
    {
        /// <summary>
        /// 游戏运行模式
        /// </summary>
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        /// <summary>
        /// 默认构建管线
        /// </summary>
        public EDefaultBuildPipeline DefaultBuildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

        /// <summary>
        /// 包名
        /// </summary>
        public string PackageName = "DefaultPackage";

        /// <summary>
        /// 资源包裹
        /// </summary>
        public ResourcePackage Package;

        /// <summary>
        /// 包版本
        /// </summary>
        public string PackageVersion;

        /// <summary>
        /// 资源下载操作
        /// </summary>
        public ResourceDownloaderOperation Downloader;
    }
}
