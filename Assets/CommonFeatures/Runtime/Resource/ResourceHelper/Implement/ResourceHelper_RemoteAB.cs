using CommonFeatures.Config;
using CommonFeatures.NetWork;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// ���ذ汾�ļ���ַ
        /// </summary>
        private string m_LocalVersionFilePath;

        /// <summary>
        /// Զ��ab�ļ���ַ
        /// </summary>
        private string m_RemoteABFilePath;

        /// <summary>
        /// ����AB�ļ���ַ
        /// </summary>
        private string m_LocalABFilePath;

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
            this.m_OnLoading?.Invoke("У�鱾���ļ�", 0f, 1f);

            var versionPath = CFM.Config.GetStringConfig("Resource", "RemoteAB", "remote_version_path");
            CFM.Http.Get(versionPath, null, 
                completeCallback: request =>
                {
                    m_RemoteVersionInfo = JsonMapper.ToObject<ResourceVersionInfo>(request.downloadHandler.text);
                    AnalysisLocalVersionFile();
                }, 
                errorCallback: request =>
                {
                    this.m_OnLoadError?.Invoke(new System.Exception(request.error));
                });
        }

        /// <summary>
        /// �������ذ汾�ļ�
        /// </summary>
        private void AnalysisLocalVersionFile()
        {
            m_LocalVersionFilePath = Path.Combine(Application.persistentDataPath, CFM.Config.GetStringConfig("Resource", "RemoteAB", "local_version_path"));
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
        /// �ȶ԰汾�ļ�
        /// </summary>
        private void CompareVersionFile()
        {
            if (null == m_LocalVersionInfo)
            {
                DownloadFileList();
                return;
            }

            //�汾��ͬ
            if (m_LocalVersionInfo.FullVersion.Equals(m_RemoteVersionInfo.FullVersion))
            {
                this.m_OnLoading?.Invoke("У�鱾���ļ�", 1f, 1f);
                this.m_OnLoadEnd?.Invoke();
            }
            //�汾��ͬ
            else
            {
                m_RemoteABFilePath = Path.Combine(Application.persistentDataPath, CFM.Config.GetStringConfig("Resource", "RemoteAB", "remote_AB_directory_path"));
                CFM.Http.Get(m_RemoteABFilePath, null,
                    completeCallback: webrequest =>
                    {

                    },
                    errorCallback: webrequest =>
                    {

                    });
            }
        }

        /// <summary>
        /// �����ļ��б�
        /// </summary>
        private void DownloadFileList()
        {

        }

        private void DownloadRemoteFiles()
        {

        }
    }
}
