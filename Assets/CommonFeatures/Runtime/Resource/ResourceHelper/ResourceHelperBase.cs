using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源加载基类
    /// </summary>
    public abstract class ResourceHelperBase : IResourceHelper
    {
        protected System.Action m_OnLoadStart = null;
        protected System.Action<float, float> m_OnLoading = null;
        protected System.Action m_OnLoadEnd = null;

        public void Load(System.Action onLoadStart, System.Action<float, float> onLoading, System.Action onLoadEnd)
        {
            this.m_OnLoadStart = onLoadStart;
            this.m_OnLoading = onLoading;
            this.m_OnLoadEnd = onLoadEnd;
            Load();
        }

        protected abstract void Load();
    }
}
