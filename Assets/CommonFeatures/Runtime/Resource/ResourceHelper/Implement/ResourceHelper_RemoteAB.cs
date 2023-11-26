using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源辅助类:远程加载AB资源
    /// </summary>
    public class ResourceHelper_RemoteAB : ResourceHelperBase
    {
        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            this.m_OnLoadEnd?.Invoke();
        }
    }
}
