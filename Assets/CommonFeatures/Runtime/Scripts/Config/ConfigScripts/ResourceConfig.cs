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
        [Header("游戏运行模式")]
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        [Header("默认构建管线")]
        public EDefaultBuildPipeline DefaultBuildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

        [Header("包名")]
        public string PackageName = "DefaultPackage";
    }
}
