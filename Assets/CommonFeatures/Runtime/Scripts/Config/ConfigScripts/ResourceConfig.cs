using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ����
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Resource", fileName = "ResourceConfig")]
    public class ResourceConfig : ScriptableObject
    {
        [Header("��Ϸ����ģʽ")]
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        [Header("Ĭ�Ϲ�������")]
        public EDefaultBuildPipeline DefaultBuildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

        [Header("����")]
        public string PackageName = "DefaultPackage";
    }
}
