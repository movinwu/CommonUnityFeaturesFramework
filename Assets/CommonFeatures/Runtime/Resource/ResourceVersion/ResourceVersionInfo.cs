using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源版本信息
    /// </summary>
    public class ResourceVersionInfo
    {
        /// <summary>
        /// 游戏版本
        /// </summary>
        public string GameVersion;

        /// <summary>
        /// AB包版本
        /// </summary>
        public int ABVersion;

        /// <summary>
        /// 完整版本号
        /// </summary>
        public string FullVersion { get => $"{GameVersion}_{ABVersion}"; }

        /// <summary>
        /// AB文件数量
        /// </summary>
        public int ABFileCount;

        /// <summary>
        /// AB文件总字节数
        /// </summary>
        public ulong ABFileLength;
    }
}
