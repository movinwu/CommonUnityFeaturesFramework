using CommonFeatures.Config;
using CommonFeatures.NetWork;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ������:Զ�̼���AB��Դ
    /// </summary>
    public class ResourceHelper_RemoteAB : ResourceHelperBase
    {
        /// <summary>
        /// Զ�˰汾��Ϣ
        /// </summary>
        private ResourceVersionInfo m_RemoteVersionInfo;

        /// <summary>
        /// ���ذ汾��Ϣ
        /// </summary>
        private ResourceVersionInfo m_LocalVersionInfo;

        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            LoadVersionFile();
        }

        /// <summary>
        /// ���ذ汾�ļ�
        /// </summary>
        private void LoadVersionFile()
        {
            this.m_OnLoading("���ذ汾��Ϣ", 0f, 1f);

            var versionPath = ConfigManager.Instance.GetStrConfig("Resource", "RemoteAB", "remote_version_path");
            HttpManager.Instance.Get(versionPath, null, 
                completeCallback: request =>
                {
                    this.m_OnLoading("���ذ汾��Ϣ", 1f, 1f);
                    m_RemoteVersionInfo = JsonMapper.ToObject<ResourceVersionInfo>(request.downloadHandler.text);
                    AnalysisVersionFile();
                }, 
                errorCallback: request =>
                {
                    this.m_OnLoadError?.Invoke(new System.Exception(request.error));
                });
        }

        /// <summary>
        /// �����汾�ļ�
        /// </summary>
        private void AnalysisVersionFile()
        {

        }
    }
}
