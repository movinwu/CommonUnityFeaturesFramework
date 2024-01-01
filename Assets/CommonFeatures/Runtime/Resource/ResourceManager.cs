using CommonFeatures.Config;
using CommonFeatures.Log;
using CommonFeatures.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ������
    /// </summary>
    public class ResourceManager : SingletonBase<ResourceManager>
    {
        private ResourceManager() { Init(); }

        /// <summary>
        /// ��Դ��������
        /// </summary>
        private EResourceLoadType m_ResourceLoadType;

        /// <summary>
        /// ��Դ���ظ�����
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
                    CommonLog.LogError("��Դ��������ش���,��������ļ�Resource��resource_load_type����");
                    break;
            }
        }

        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="onLoadStart">��Դ���ؿ�ʼ�ص�</param>
        /// <param name="onLoading">��Դ�����лص�</param>
        /// <param name="onLoadEnd">��Դ���ؽ����ص�</param>
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
