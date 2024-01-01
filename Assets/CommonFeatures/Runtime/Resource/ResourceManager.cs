using CommonFeatures.Config;
using CommonFeatures.Log;
using CommonFeatures.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourceManager : SingletonBase<ResourceManager>
    {
        private ResourceManager() { Init(); }

        /// <summary>
        /// 资源加载类型
        /// </summary>
        private EResourceLoadType m_ResourceLoadType;

        /// <summary>
        /// 资源加载辅助类
        /// </summary>
        private IResourceHelper m_Helper;

        private void Init()
        {
            m_ResourceLoadType = (EResourceLoadType)CommonConfig.GetLongConfig("Resource", "resource_load_type");
            switch (m_ResourceLoadType)
            {
                case EResourceLoadType.RemoteAB:
                    m_Helper = new ResourceHelper_RemoteAB();
                    break;
                case EResourceLoadType.LocalAB:
                    m_Helper = new ResourceHelper_LocalAB();
                    break;
                case EResourceLoadType.Editor:
                    m_Helper = new ResourceHelper_Editor();
                    break;
                default:
                    CommonLog.LogError("资源辅助类加载错误,检查配置文件Resource中resource_load_type配置");
                    break;
            }
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="onLoadStart">资源加载开始回调</param>
        /// <param name="onLoading">资源加载中回调</param>
        /// <param name="onLoadEnd">资源加载结束回调</param>
        public void LoadResource(System.Action onLoadStart, System.Action<string, float, float> onLoading, System.Action onLoadEnd, System.Action<System.Exception> onLoadError)
        {
            try
            {
                m_Helper.Load(onLoadStart, onLoading, onLoadEnd, onLoadError);
            }
            catch (System.Exception ex)
            {
                CommonLog.ResourceException(ex);
            }
        }
    }
}
