using CommonFeatures.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 下载句柄
    /// </summary>
    public class DownloadHandler : IReference
    {
        /// <summary>
        /// 下载唯一id
        /// </summary>
        public ulong uniqueId;

        /// <summary>
        /// 下载地址
        /// </summary>
        public string downloadPath;

        /// <summary>
        /// 保存地址
        /// </summary>
        public string savePath;

        /// <summary>
        /// 网络请求
        /// </summary>
        public UnityWebRequest webRequest;

        /// <summary>
        /// 下载开始回调
        /// </summary>
        public Action<DownloadHandler> onDownloadStart;

        /// <summary>
        /// 下载中回调
        /// </summary>
        public Action<DownloadHandler> onDownloading;

        /// <summary>
        /// 下载完成回调
        /// </summary>
        public Action<DownloadHandler> onDownloadComplete;

        /// <summary>
        /// 下载失败回调
        /// </summary>
        public Action<DownloadHandler> onDownloadFailed;

        /// <summary>
        /// 下载文件总长度
        /// </summary>
        public ulong downloadTotalLength;

        /// <summary>
        /// 之前request已经完成下载的长度
        /// </summary>
        public ulong preDownloadedCompletedLength;

        /// <summary>
        /// 当前request已经下载完成的长度
        /// </summary>
        public ulong curDownloadedCompletedLength;

        /// <summary>
        /// 文件已下载长度
        /// </summary>
        public ulong downloadedLength;

        /// <summary>
        /// 下载协程
        /// </summary>
        public Coroutine downloadCoroutine;

        /// <summary>
        /// 下载重置次数
        /// </summary>
        public int timeoutRepeatedTimer;

        /// <summary>
        /// 下载重置计时器
        /// </summary>
        public float timeoutTimer;

        /// <summary>
        /// 开始下载标识
        /// </summary>
        public bool startFlag;

        public void Reset()
        {
            uniqueId = 0;
            downloadPath = string.Empty;
            savePath = string.Empty;
            webRequest = null;
            onDownloadStart = null;
            onDownloading = null;
            onDownloadComplete = null;
            downloadTotalLength = 0;
            downloadedLength = 0;
            preDownloadedCompletedLength = 0;
            curDownloadedCompletedLength = 0;
            downloadCoroutine = null;
            timeoutRepeatedTimer = 0;
            timeoutTimer = 0;
            startFlag = false;
        }
    }
}
