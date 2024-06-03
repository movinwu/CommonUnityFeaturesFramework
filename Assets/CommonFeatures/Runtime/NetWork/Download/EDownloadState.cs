using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 下载状态枚举
    /// </summary>
    public enum EDownloadState : byte
    {
        /// <summary>
        /// 下载结束
        /// </summary>
        Done,

        /// <summary>
        /// 下载中
        /// </summary>
        Downloading,
    }
}
