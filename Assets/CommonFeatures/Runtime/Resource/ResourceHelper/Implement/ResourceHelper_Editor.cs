using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源加载辅助类:本地资源
    /// </summary>
    public class ResourceHelper_Editor : ResourceHelperBase
    {
        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            this.m_OnLoadEnd?.Invoke();
        }
    }
}
