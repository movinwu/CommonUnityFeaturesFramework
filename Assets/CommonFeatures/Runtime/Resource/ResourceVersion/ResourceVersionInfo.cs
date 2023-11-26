using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ�汾��Ϣ
    /// </summary>
    public class ResourceVersionInfo
    {
        /// <summary>
        /// ��Ϸ�汾
        /// </summary>
        public string GameVersion;

        /// <summary>
        /// AB���汾
        /// </summary>
        public int ABVersion;

        /// <summary>
        /// �����汾��
        /// </summary>
        public string FullVersion { get => $"{GameVersion}_{ABVersion}"; }

        /// <summary>
        /// AB�ļ�����
        /// </summary>
        public int ABFileCount;

        /// <summary>
        /// AB�ļ����ֽ���
        /// </summary>
        public ulong ABFileLength;
    }
}
