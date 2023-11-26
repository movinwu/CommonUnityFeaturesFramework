using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源加载辅助类:从本地加载AB资源
    /// </summary>
    public class ResourceHelper_LocalAB : ResourceHelperBase
    {
        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            this.m_OnLoadEnd?.Invoke();
        }
    }
}
