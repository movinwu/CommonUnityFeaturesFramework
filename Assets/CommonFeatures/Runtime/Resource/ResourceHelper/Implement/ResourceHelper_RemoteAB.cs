using CommonFeatures.Config;
using CommonFeatures.NetWork;
using LitJson;
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
        /// <summary>
        /// 远端版本信息
        /// </summary>
        private ResourceVersionInfo m_RemoteVersionInfo;

        /// <summary>
        /// 本地版本信息
        /// </summary>
        private ResourceVersionInfo m_LocalVersionInfo;

        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            LoadVersionFile();
        }

        /// <summary>
        /// 加载版本文件
        /// </summary>
        private void LoadVersionFile()
        {
            this.m_OnLoading("加载版本信息", 0f, 1f);

            var versionPath = ConfigManager.Instance.GetStrConfig("Resource", "RemoteAB", "remote_version_path");
            HttpManager.Instance.Get(versionPath, null, 
                completeCallback: request =>
                {
                    this.m_OnLoading("加载版本信息", 1f, 1f);
                    m_RemoteVersionInfo = JsonMapper.ToObject<ResourceVersionInfo>(request.downloadHandler.text);
                    AnalysisVersionFile();
                }, 
                errorCallback: request =>
                {
                    this.m_OnLoadError?.Invoke(new System.Exception(request.error));
                });
        }

        /// <summary>
        /// 分析版本文件
        /// </summary>
        private void AnalysisVersionFile()
        {

        }
    }
}
