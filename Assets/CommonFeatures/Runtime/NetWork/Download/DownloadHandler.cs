using CommonFeatures.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// ���ؾ��
    /// </summary>
    public class DownloadHandler : IReference
    {
        /// <summary>
        /// ����Ψһid
        /// </summary>
        public ulong uniqueId;

        /// <summary>
        /// ���ص�ַ
        /// </summary>
        public string downloadPath;

        /// <summary>
        /// �����ַ
        /// </summary>
        public string savePath;

        /// <summary>
        /// ��������
        /// </summary>
        public UnityWebRequest webRequest;

        /// <summary>
        /// ���ؿ�ʼ�ص�
        /// </summary>
        public Action<DownloadHandler> onDownloadStart;

        /// <summary>
        /// �����лص�
        /// </summary>
        public Action<DownloadHandler> onDownloading;

        /// <summary>
        /// ������ɻص�
        /// </summary>
        public Action<DownloadHandler> onDownloadComplete;

        /// <summary>
        /// ����ʧ�ܻص�
        /// </summary>
        public Action<DownloadHandler> onDownloadFailed;

        /// <summary>
        /// �����ļ��ܳ���
        /// </summary>
        public ulong downloadTotalLength;

        /// <summary>
        /// ֮ǰrequest�Ѿ�������صĳ���
        /// </summary>
        public ulong preDownloadedCompletedLength;

        /// <summary>
        /// ��ǰrequest�Ѿ�������ɵĳ���
        /// </summary>
        public ulong curDownloadedCompletedLength;

        /// <summary>
        /// �ļ������س���
        /// </summary>
        public ulong downloadedLength;

        /// <summary>
        /// ����Э��
        /// </summary>
        public Coroutine downloadCoroutine;

        /// <summary>
        /// �������ô���
        /// </summary>
        public int timeoutRepeatedTimer;

        /// <summary>
        /// �������ü�ʱ��
        /// </summary>
        public float timeoutTimer;

        /// <summary>
        /// ��ʼ���ر�ʶ
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
