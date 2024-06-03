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
    /// ����������
    /// </summary>
    public class DownloadModel
    {
        /// <summary>
        /// ���ص�ַ
        /// </summary>
        public string downloadUrl;

        /// <summary>
        /// �����ַ
        /// </summary>
        public string savePath;

        /// <summary>
        /// ��������
        /// </summary>
        public UnityWebRequest webRequest;

        /// <summary>
        /// �����ļ��ܳ���
        /// </summary>
        public AsyncReactiveProperty<ulong> downloadTotalLength;

        /// <summary>
        /// �Ѿ�������صĳ���
        /// </summary>
        public AsyncReactiveProperty<ulong> downloadedLength;

        /// <summary>
        /// ��ǰrequest������ɵĳ���
        /// </summary>
        public AsyncReactiveProperty<ulong> curDownloadedCompletedLength;

        /// <summary>
        /// �ļ������س��Ƚ���
        /// </summary>
        public ulong curDownloadedProgressLength { get => downloadedLength + curDownloadedCompletedLength; }

        /// <summary>
        /// �������ô���
        /// </summary>
        public AsyncReactiveProperty<int> downloadRepeatedCounter;

        /// <summary>
        /// �������ü�ʱ��
        /// </summary>
        public int timeout;

        /// <summary>
        /// ȡ��token
        /// </summary>
        public CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// ����״̬
        /// </summary>
        public EDownloadState downloadState;

        /// <summary>
        /// ��task
        /// </summary>
        public DownloadTask task;
    }
}
