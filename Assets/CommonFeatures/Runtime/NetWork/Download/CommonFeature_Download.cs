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
    /// ͨ�ù���-����
    /// </summary>
    public class CommonFeature_Download : CommonFeature
    {
        /// <summary>
        /// ��������
        /// </summary>
        private Dictionary<ulong, DownloadHandler> m_AllDownloads = new Dictionary<ulong, DownloadHandler>();

        /// <summary>
        /// ���б����ַ(��֤�����ַΨһ)
        /// </summary>
        private HashSet<string> m_AllDownloadsSavePath = new HashSet<string>();

        /// <summary>
        /// ÿ���������ݱ��س���
        /// </summary>
        private ulong m_EachDownloadByteLength = 0;

        /// <summary>
        /// timeout
        /// </summary>
        private float m_Timeout = 0;

        /// <summary>
        /// �жϺ������������
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
        /// ��ʼһ������
        /// </summary>
        /// <param name="url">��Դ��λ��ַ</param>
        /// <param name="saveDirectoryPath">�����ַ</param>
        /// <param name="onDownloadStart">��ʼ���ػص�</param>
        /// <param name="onDownloading">�����лص�</param>
        /// <param name="onDownloadComplete">������ɻص�</param>
        /// <returns>����Ψһid</returns>
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
            //û��������ɵ��ļ���.download��Ϊ��ʱ��׺
            saveDirectoryPath = Path.Combine(saveDirectoryPath, $"{fileName}.download");

            if (m_AllDownloadsSavePath.Contains(saveDirectoryPath))
            {
                CommonLog.NetError($"�ظ������������ݵ���ͬ�ĵ�ַ: {saveDirectoryPath}");
                return 0;
            }

            if (File.Exists(saveDirectoryPath))
            {
                File.Delete(saveDirectoryPath);
            }
            File.WriteAllText(saveDirectoryPath, string.Empty);//�������ļ�
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
        /// �ж�һ������
        /// </summary>
        /// <param name="uniqueId"></param>
        public void AbortDownload(ulong uniqueId)
        {
            if (m_AllDownloads.TryGetValue(uniqueId, out var handler))
            {
                if (null != handler.downloadCoroutine)
                {
                    AbortDownload(handler);

                    //ɾ���ļ�
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
        /// �ֶΣ��ϵ������ļ�
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        private IEnumerator BreakpointResume(DownloadHandler handler)
        {
            if (handler.downloadTotalLength == 0)
            {
                //UnityWebRequest �����ÿɴ��� HTTP HEAD ����� UnityWebRequest��
                UnityWebRequest headRequest = UnityWebRequest.Head(handler.downloadPath);
                //��ʼ��Զ�̷�����ͨ�š�
                yield return headRequest.SendWebRequest();

                if (!string.IsNullOrEmpty(headRequest.error))
                {
                    Debug.LogError("��ȡ������Դ�ļ�");
                    yield break;
                }
                //��ȡ�ļ��ܴ�С
                ulong totalLength = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
                //Debug.Log("��ȡ��С" + totalLength);
                handler.downloadTotalLength = totalLength;
                headRequest.Dispose();
            }

            handler.webRequest = UnityWebRequest.Get(handler.downloadPath);
            //append����Ϊtrue�ļ�д�뷽ʽΪ����д�룬������ԭ�ļ���
            handler.webRequest.downloadHandler = new DownloadHandlerFile(handler.savePath, true);
            //�����ļ�
            FileInfo file = new FileInfo(handler.savePath);
            //��ǰ���ص��ļ�����
            handler.preDownloadedCompletedLength = (ulong)file.Length;
            handler.curDownloadedCompletedLength = 0;
            handler.downloadedLength = handler.preDownloadedCompletedLength + handler.curDownloadedCompletedLength;

            //�����������ݴӵ�fileLength�������ֽڣ�
            handler.webRequest.SetRequestHeader("Range", "bytes=" + handler.preDownloadedCompletedLength + "-");

            if (!string.IsNullOrEmpty(handler.webRequest.error))
            {
                CommonLog.NetError("����ʧ��" + handler.webRequest.error);
                handler.onDownloadFailed?.Invoke(handler);
            }
            if (handler.curDownloadedCompletedLength < handler.downloadTotalLength)
            {
                handler.webRequest.SendWebRequest();
                while (!handler.webRequest.isDone)
                {
                    //����û�м���,��ʼ��ʱ
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
                                CommonLog.NetError($"{handler.downloadPath} ����ʧ��");
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
                        //����һ�����ֽڹر����ڵ�Э�̣������µ�Э�̣�����Դ�ֶ�����
                        if (handler.curDownloadedCompletedLength >= m_EachDownloadByteLength)
                        {
                            if (!string.IsNullOrEmpty(handler.webRequest.error))
                            {
                                CommonLog.NetError("����ʧ��" + handler.webRequest.error);
                                handler.onDownloadFailed?.Invoke(handler);
                                break;
                            }
                            //��� UnityWebRequest �ڽ����У���ֹͣ��
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

            //��ʾ����ʹ�ô� UnityWebRequest������Ӧ������ʹ�õ�������Դ��
            handler.webRequest.Dispose();

            //�������ļ�
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
