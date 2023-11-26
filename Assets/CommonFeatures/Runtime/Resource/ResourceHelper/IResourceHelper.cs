using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ������ӿ�
    /// </summary>
    public interface IResourceHelper
    {
        /// <summary>
        /// ������Դ
        /// </summary>
        void Load(System.Action onLoadStart, System.Action<float, float> onLoading, System.Action onLoadEnd);
    }
}
