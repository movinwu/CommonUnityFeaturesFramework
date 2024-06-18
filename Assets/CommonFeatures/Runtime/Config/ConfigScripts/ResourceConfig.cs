using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ����
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Resource", fileName = "ResourceConfig")]
    public class ResourceConfig : ScriptableObject
    {
        /// <summary>
        /// ��Դ��������
        /// </summary>
        public EResourceLoadType ResourceLoadType = EResourceLoadType.Editor;
    }
}
