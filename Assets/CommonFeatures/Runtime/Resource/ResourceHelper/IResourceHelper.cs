using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源辅助类接口
    /// </summary>
    public interface IResourceHelper
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        void Load(System.Action onLoadStart, System.Action<float, float> onLoading, System.Action onLoadEnd);
    }
}
