using CommonFeatures.Pool;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 下载数据类
    /// </summary>
    public class DownloadModel
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string downloadUrl;

        /// <summary>
        /// 保存地址
        /// </summary>
        public string savePath;

        /// <summary>
        /// 网络请求
        /// </summary>
        public UnityWebRequest webRequest;

        /// <summary>
        /// 下载文件总长度
        /// </summary>
        public AsyncReactiveProperty<ulong> downloadTotalLength;

        /// <summary>
        /// 已经完成下载的长度
        /// </summary>
        public AsyncReactiveProperty<ulong> downloadedLength;

        /// <summary>
        /// 当前request下载完成的长度
        /// </summary>
        public AsyncReactiveProperty<ulong> curDownloadedCompletedLength;

        /// <summary>
        /// 文件已下载长度进度
        /// </summary>
        public ulong curDownloadedProgressLength { get => downloadedLength + curDownloadedCompletedLength; }

        /// <summary>
        /// 下载重置次数
        /// </summary>
        public AsyncReactiveProperty<int> downloadRepeatedCounter;

        /// <summary>
        /// 下载重置计时器
        /// </summary>
        public int timeout;

        /// <summary>
        /// 取消token
        /// </summary>
        public CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// 下载状态
        /// </summary>
        public EDownloadState downloadState;

        /// <summary>
        /// 绑定task
        /// </summary>
        public DownloadTask task;
    }
}
