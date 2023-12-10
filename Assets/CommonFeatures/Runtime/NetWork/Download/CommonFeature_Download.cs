using CommonFeatures.Config;
using CommonFeatures.Log;
using CommonFeatures.Pool;
using CommonFeatures.Singleton;
using CommonFeatures.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通用功能-下载
    /// </summary>
    public class CommonFeature_Download : CommonFeature
    {
        /// <summary>
        /// 所有下载
        /// </summary>
        private Dictionary<ulong, DownloadHandler> m_AllDownloads = new Dictionary<ulong, DownloadHandler>();

        /// <summary>
        /// 所有保存地址(保证保存地址唯一)
        /// </summary>
        private HashSet<string> m_AllDownloadsSavePath = new HashSet<string>();

        /// <summary>
        /// 每次下载数据比特长度
        /// </summary>
        private ulong m_EachDownloadByteLength = 0;

        /// <summary>
        /// timeout
        /// </summary>
        private float m_Timeout = 0;

        /// <summary>
        /// 中断后重新请求次数
        /// </summary>
        private int m_TimeoutRepeatedTime = 0;

        public override void Init()
        {
            m_EachDownloadByteLength = (ulong)CFM.Config.GetLongConfig("Download", "each_bytes_length");

            m_Timeout = (float)CFM.Config.GetDoubleConfig("Download", "timeout");
            m_Timeout = Mathf.Max(0, m_Timeout);

            m_TimeoutRepeatedTime = (int)CFM.Config.GetLongConfig("Download", "repeated_time");
            m_TimeoutRepeatedTime = Mathf.Max(0, m_TimeoutRepeatedTime);
        }

        /// <summary>
        /// 开始一个下载
        /// </summary>
        /// <param name="url">资源定位地址</param>
        /// <param name="saveDirectoryPath">保存地址</param>
        /// <param name="onDownloadStart">开始下载回调</param>
        /// <param name="onDownloading">下载中回调</param>
        /// <param name="onDownloadComplete">下载完成回调</param>
        /// <returns>下载唯一id</returns>
        public ulong AddDownload(string url, string saveDirectoryPath,
            System.Action<DownloadHandler> onDownloadStart = null,
            System.Action<DownloadHandler> onDownloading = null,
            System.Action<DownloadHandler> onDownloadComplete = null,
            System.Action<DownloadHandler> onDownloadFailed = null)
        {
            if (!Directory.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
            }

            var fileName = url.Substring(url.LastIndexOf('/') + 1);
            //没有下载完成的文件以.download作为临时后缀
            saveDirectoryPath = Path.Combine(saveDirectoryPath, $"{fileName}.download");

            if (m_AllDownloadsSavePath.Contains(saveDirectoryPath))
            {
                CommonLog.NetError($"重复保存下载内容到相同的地址: {saveDirectoryPath}");
                return 0;
            }

            if (File.Exists(saveDirectoryPath))
            {
                File.Delete(saveDirectoryPath);
            }
            File.WriteAllText(saveDirectoryPath, string.Empty);//创建空文件
            var handler = ReferencePool.Acquire<DownloadHandler>();
            handler.downloadPath = url;
            handler.savePath = saveDirectoryPath;
            handler.onDownloadStart = onDownloadStart;
            handler.onDownloading = onDownloading;
            handler.onDownloadComplete = onDownloadComplete;
            handler.onDownloadFailed = onDownloadFailed;

            handler.uniqueId = UniqueIDUtility.GenerateUniqueID();
            m_AllDownloads.Add(handler.uniqueId, handler);
            m_AllDownloadsSavePath.Add(saveDirectoryPath);

            handler.onDownloadStart?.Invoke(handler);

            handler.downloadCoroutine = StartCoroutine(BreakpointResume(handler));

            return handler.uniqueId;
        }

        /// <summary>
        /// 中断一个下载
        /// </summary>
        /// <param name="uniqueId"></param>
        public void AbortDownload(ulong uniqueId)
        {
            if (m_AllDownloads.TryGetValue(uniqueId, out var handler))
            {
                if (null != handler.downloadCoroutine)
                {
                    AbortDownload(handler);

                    //删除文件
                    if (File.Exists(handler.savePath))
                    {
                        File.Delete(handler.savePath);
                    }
                }

                m_AllDownloads.Remove(uniqueId);
                m_AllDownloadsSavePath.Remove(handler.savePath);
            }
        }

        private void AbortDownload(DownloadHandler handler)
        {
            StopCoroutine(handler.downloadCoroutine);
            handler.downloadCoroutine = null;

            if (null != handler.webRequest && !handler.webRequest.isDone)
            {
                handler.webRequest.Abort();
                handler.webRequest = null;
            }
        }

        /// <summary>
        /// 分段，断点下载文件
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        private IEnumerator BreakpointResume(DownloadHandler handler)
        {
            if (handler.downloadTotalLength == 0)
            {
                //UnityWebRequest 经配置可传输 HTTP HEAD 请求的 UnityWebRequest。
                UnityWebRequest headRequest = UnityWebRequest.Head(handler.downloadPath);
                //开始与远程服务器通信。
                yield return headRequest.SendWebRequest();

                if (!string.IsNullOrEmpty(headRequest.error))
                {
                    Debug.LogError("获取不到资源文件");
                    yield break;
                }
                //获取文件总大小
                ulong totalLength = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
                //Debug.Log("获取大小" + totalLength);
                handler.downloadTotalLength = totalLength;
                headRequest.Dispose();
            }

            handler.webRequest = UnityWebRequest.Get(handler.downloadPath);
            //append设置为true文件写入方式为接续写入，不覆盖原文件。
            handler.webRequest.downloadHandler = new DownloadHandlerFile(handler.savePath, true);
            //创建文件
            FileInfo file = new FileInfo(handler.savePath);
            //当前下载的文件长度
            handler.preDownloadedCompletedLength = (ulong)file.Length;
            handler.curDownloadedCompletedLength = 0;
            handler.downloadedLength = handler.preDownloadedCompletedLength + handler.curDownloadedCompletedLength;

            //请求网络数据从第fileLength到最后的字节；
            handler.webRequest.SetRequestHeader("Range", "bytes=" + handler.preDownloadedCompletedLength + "-");

            if (!string.IsNullOrEmpty(handler.webRequest.error))
            {
                CommonLog.NetError("下载失败" + handler.webRequest.error);
                handler.onDownloadFailed?.Invoke(handler);
            }
            if (handler.curDownloadedCompletedLength < handler.downloadTotalLength)
            {
                handler.webRequest.SendWebRequest();
                while (!handler.webRequest.isDone)
                {
                    //下载没有继续,开始计时
                    if (handler.startFlag && handler.curDownloadedCompletedLength == handler.webRequest.downloadedBytes)
                    {
                        handler.timeoutTimer += Time.deltaTime;
                        if (handler.timeoutTimer >= m_Timeout)
                        {
                            handler.timeoutTimer -= m_Timeout;
                            handler.timeoutRepeatedTimer++;
                            if (handler.timeoutRepeatedTimer >= m_TimeoutRepeatedTime)
                            {
                                handler.webRequest.Abort();
                                CommonLog.NetError($"{handler.downloadPath} 下载失败");
                                handler.onDownloadFailed?.Invoke(handler);
                                break;
                            }
                            else
                            {
                                AbortDownload(handler);
                                handler.timeoutTimer = 0;
                                handler.startFlag = false;
                                handler.downloadCoroutine = StartCoroutine(BreakpointResume(handler));
                                yield break;
                            }
                        }
                    }
                    else
                    {
                        if (handler.startFlag)
                        {
                            handler.timeoutRepeatedTimer = 0;
                        }
                        else
                        {
                            handler.startFlag = true;
                        }
                        handler.timeoutTimer = 0;
                        handler.curDownloadedCompletedLength = handler.webRequest.downloadedBytes;
                        handler.downloadedLength = handler.curDownloadedCompletedLength + handler.preDownloadedCompletedLength;
                        handler.onDownloading?.Invoke(handler);
                        //超过一定的字节关闭现在的协程，开启新的协程，将资源分段下载
                        if (handler.curDownloadedCompletedLength >= m_EachDownloadByteLength)
                        {
                            if (!string.IsNullOrEmpty(handler.webRequest.error))
                            {
                                CommonLog.NetError("下载失败" + handler.webRequest.error);
                                handler.onDownloadFailed?.Invoke(handler);
                                break;
                            }
                            //如果 UnityWebRequest 在进行中，就停止。
                            handler.webRequest.Abort();
                            handler.downloadCoroutine = StartCoroutine(BreakpointResume(handler));
                            yield break;
                        }
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
            if (string.IsNullOrEmpty(handler.webRequest.error))
            {
                handler.downloadedLength = handler.webRequest.downloadedBytes;

                handler.onDownloadComplete?.Invoke(handler);
            }

            //表示不再使用此 UnityWebRequest，并且应清理它使用的所有资源。
            handler.webRequest.Dispose();

            //重命名文件
            var newSavePath = handler.savePath.Substring(0, handler.savePath.LastIndexOf('.'));
            if (File.Exists(newSavePath))
            {
                File.Delete(newSavePath);
            }
            File.Move(handler.savePath, newSavePath);

            m_AllDownloads.Remove(handler.uniqueId);
            m_AllDownloadsSavePath.Remove(handler.savePath);
            ReferencePool.Back(handler);
        }
    }
}
