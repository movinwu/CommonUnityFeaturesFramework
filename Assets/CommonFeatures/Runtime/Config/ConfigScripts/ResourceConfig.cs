using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源配置
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Resource", fileName = "ResourceConfig")]
    public class ResourceConfig : ScriptableObject
    {
        /// <summary>
        /// 资源加载类型
        /// </summary>
        public EResourceLoadType ResourceLoadType = EResourceLoadType.Editor;
    }
}
