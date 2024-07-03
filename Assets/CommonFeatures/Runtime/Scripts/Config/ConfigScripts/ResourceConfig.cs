using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源配置
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Resource", fileName = "ResourceConfig")]
    public class ResourceConfig : ScriptableObject
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
    }
}
