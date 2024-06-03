using CommonFeatures.Config;
using CommonFeatures.NetWork;
using Cysharp.Threading.Tasks;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// 本地版本文件地址
        /// </summary>
        private string m_LocalVersionFilePath;

        /// <summary>
        /// 远端ab文件地址
        /// </summary>
        private string m_RemoteABFilePath;

        /// <summary>
        /// 本地AB文件地址
        /// </summary>
        private string m_LocalABFilePath;

        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            LoadVersionFile();
        }

        /// <summary>
        /// 加载版本文件
        /// </summary>
        private async void LoadVersionFile()
        {
            this.m_OnLoading?.Invoke("校验本地文件", 0f, 1f);

            var versionPath = CommonConfig.GetStringConfig("Resource", "RemoteAB", "remote_version_path");
            var result = await CommonFeaturesManager.Http.Get(versionPath, null);
            if (result.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                m_RemoteVersionInfo = JsonMapper.ToObject<ResourceVersionInfo>(result.downloadHandler.text);
                AnalysisLocalVersionFile();
            }
            else
            {
                this.m_OnLoadError?.Invoke(new System.Exception(result.error));
            }
        }

        /// <summary>
        /// 分析本地版本文件
        /// </summary>
        private void AnalysisLocalVersionFile()
        {
            m_LocalVersionFilePath = Path.Combine(Application.persistentDataPath, CommonConfig.GetStringConfig("Resource", "RemoteAB", "local_version_path"));
            if (!File.Exists(m_LocalVersionFilePath))
            {
                m_LocalVersionInfo = null;
                CompareVersionFile();
                return;
            }

            var text = File.ReadAllText(m_LocalVersionFilePath);
            m_LocalVersionInfo = JsonMapper.ToObject<ResourceVersionInfo>(text);
            CompareVersionFile();
        }

        /// <summary>
        /// 比对版本文件
        /// </summary>
        private async UniTaskVoid CompareVersionFile()
        {
            if (null == m_LocalVersionInfo)
            {
                DownloadFileList();
                return;
            }

            //版本相同
            if (m_LocalVersionInfo.FullVersion.Equals(m_RemoteVersionInfo.FullVersion))
            {
                this.m_OnLoading?.Invoke("校验本地文件", 1f, 1f);
                this.m_OnLoadEnd?.Invoke();
            }
            //版本不同
            else
            {
                m_RemoteABFilePath = Path.Combine(Application.persistentDataPath, CommonConfig.GetStringConfig("Resource", "RemoteAB", "remote_AB_directory_path"));
                await CommonFeaturesManager.Http.Get(m_RemoteABFilePath, null);
            }
        }

        /// <summary>
        /// 下载文件列表
        /// </summary>
        private void DownloadFileList()
        {

        }

        private void DownloadRemoteFiles()
        {

        }
    }
}
