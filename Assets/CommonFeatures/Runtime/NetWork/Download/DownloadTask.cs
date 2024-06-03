using CommonFeatures.Log;
using CommonFeatures.Pool;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Threading;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// ����������
    /// </summary>
    public class DownloadTask : IProgress<float>, IReference
    {
        internal DownloadModel DownloadModel { get; private set; }

        /// <summary>
        /// ÿ���������ݱ��س���
        /// <para>8 * 1024 * 512 = 4194304 ����,��512KB</para>
        /// </summary>
        private const ulong EACH_DOWNLOAD_BYTE_LENGTH = 4194304;

        /// <summary>
        /// �жϺ������������
        /// </summary>
        private const int TIMEOUT_REPEATE_COUNT = 3;

        /// <summary>
        /// ��ʼ��һ������
        /// </summary>
        /// <param name="url">��Դ��λ��ַ</param>
        /// <param name="saveDirectoryPath">�����ַ</param>
        /// <param name="timeout">��ʱʱ��</param>
        /// <returns>����Ψһid</returns>
        internal async UniTask<DownloadModel> InitDownload(string url, string saveDirectoryPath, int timeout)
        {
            if (!Directory.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
            }

            var fileName = url.Substring(url.LastIndexOf('/') + 1);
            //û��������ɵ��ļ���.download��Ϊ��ʱ��׺
            saveDirectoryPath = Path.Combine(saveDirectoryPath, $"{fileName}.download");

            if (!File.Exists(saveDirectoryPath))
            {
                File.WriteAllText(saveDirectoryPath, string.Empty);//�������ļ�
            }
            this.DownloadModel.downloadUrl = url;
            this.DownloadModel.savePath = saveDirectoryPath;
            this.DownloadModel.timeout = timeout;

            await RequestDownloadInfo();

            return this.DownloadModel;
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <returns></returns>
        internal async UniTask<DownloadModel> StartDonwload()
        {
            this.DownloadModel.downloadState = EDownloadState.Downloading;

            var (result, downloadModel) = await BreakpointResume().SuppressCancellationThrow();

            ReferencePool.Back(this);

            return downloadModel;
        }

        /// <summary>
        /// �ж�����
        /// </summary>
        /// <param name="model"></param>
        internal void AbortDownload()
        {
            if (null != this.DownloadModel.webRequest && !this.DownloadModel.webRequest.isDone)
            {
                this.DownloadModel.webRequest.Abort();
                this.DownloadModel.webRequest = null;
            }
        }

        /// <summary>
        /// �������������Ϣ
        /// </summary>
        /// <returns></returns>
        private async UniTask<DownloadModel> RequestDownloadInfo()
        {
            //UnityWebRequest �����ÿɴ��� HTTP HEAD ����� UnityWebRequest��
            var request = await CommonFeaturesManager.Http.Head(this.DownloadModel.downloadUrl, null);
            //�ɹ�����ͷ�ļ�
            if (request.result == UnityWebRequest.Result.Success)
            {
                //��ȡ�ļ��ܴ�С
                this.DownloadModel.downloadTotalLength.Value = ulong.Parse(request.GetResponseHeader("Content-Length"));
                request.Dispose();
                this.DownloadModel.downloadRepeatedCounter.Value = 0;
            }
            //����ͷ�ļ�ʧ��
            else if (!string.IsNullOrEmpty(request.error))
            {
                CommonLog.NetError($"�ӵ�ַ {this.DownloadModel.downloadUrl} ��ȡ������Դ�ļ�ͷʧ��, ʧ��ԭ��: \n{request.error}");
                request.Dispose();
                throw new OperationCanceledException();

            }
            //δ֪δ�ɹ��쳣
            else
            {
                CommonLog.NetError($"�ӵ�ַ {this.DownloadModel.downloadUrl} ��ȡ������Դ�ļ�ͷʧ��, ʧ��ԭ��δ֪, �������: \n{request.result}");
                request.Dispose();
                throw new OperationCanceledException();
            }

            return this.DownloadModel;
        }

        /// <summary>
        /// �ֶΣ��ϵ������ļ�
        /// </summary>
        /// <returns></returns>
        private async UniTask<DownloadModel> BreakpointResume()
        {
            if (this.DownloadModel.downloadTotalLength.Value > 0)
            {
                //�������ļ�
                var newSavePath = this.DownloadModel.savePath.Substring(0, this.DownloadModel.savePath.LastIndexOf('.'));
                //����Ƿ������ļ�
                if (File.Exists(newSavePath))
                {
                    var fileInfo = new FileInfo(newSavePath);
                    if (this.DownloadModel.downloadTotalLength == (ulong)fileInfo.Length)
                    {
                        this.DownloadModel.downloadedLength.Value = this.DownloadModel.downloadTotalLength.Value;
                        //�Ƴ���ʱ�����ļ�
                        if (File.Exists(this.DownloadModel.savePath))
                        {
                            File.Delete(this.DownloadModel.savePath);
                        }
                        return this.DownloadModel;
                    }
                    File.Delete(newSavePath);
                }

                await StartWebrequestGet(false);

                File.Move(this.DownloadModel.savePath, newSavePath);
            }

            this.DownloadModel.downloadState = EDownloadState.Done;
            return this.DownloadModel;
        }

        /// <summary>
        /// ��ʼһ��unitywebrequest��get��������
        /// </summary>
        /// <param name="isContinue"></param>
        /// <returns></returns>
        private async UniTask<DownloadModel> StartWebrequestGet(bool isContinue)
        {
            if (!isContinue)
            {
                //�����ļ�
                FileInfo file = new FileInfo(this.DownloadModel.savePath);
                //��ǰ���ص��ļ�����
                this.DownloadModel.downloadedLength.Value = (ulong)file.Length;
                this.DownloadModel.curDownloadedCompletedLength.Value = 0;
            }
            if (this.DownloadModel.downloadedLength.Value >= this.DownloadModel.downloadTotalLength.Value)
            {
                return this.DownloadModel;
            }

            this.DownloadModel.webRequest = UnityWebRequest.Get(this.DownloadModel.downloadUrl);
            this.DownloadModel.webRequest.timeout = this.DownloadModel.timeout + 1;
            this.DownloadModel.webRequest.downloadHandler = new DownloadHandlerFile(this.DownloadModel.savePath, true);
            //�����������ݴӵ�fileLength�������ֽ�
            this.DownloadModel.webRequest.SetRequestHeader("Range", $"bytes={this.DownloadModel.downloadedLength.Value}-");
            this.DownloadModel.cancellationTokenSource?.Dispose();
            this.DownloadModel.cancellationTokenSource = new CancellationTokenSource();
            this.DownloadModel.cancellationTokenSource.CancelAfterSlim(this.DownloadModel.timeout * 1000);
            try
            {
                var (isCancel, _) = await this.DownloadModel.webRequest.SendWebRequest()
                    .ToUniTask(progress: this, cancellationToken: this.DownloadModel.cancellationTokenSource.Token)
                    .SuppressCancellationThrow();
            }
            catch
            {
                this.DownloadModel.downloadedLength.Value += this.DownloadModel.webRequest?.downloadedBytes ?? 0;
                if (!string.IsNullOrEmpty(this.DownloadModel.webRequest?.error))
                {
                    CommonLog.NetError($"�ӵ�ַ {this.DownloadModel.downloadUrl} ������Դʧ��, ʧ��ԭ��: \n{this.DownloadModel.webRequest.error}");
                }
                else
                {
                    CommonLog.NetError($"�ӵ�ַ {this.DownloadModel.downloadUrl} ������Դʧ��, ʧ��ԭ��δ֪, �������: \n{this.DownloadModel.webRequest?.result}");
                }

                this.DownloadModel.downloadRepeatedCounter.Value++;
                if (this.DownloadModel.downloadRepeatedCounter.Value >= TIMEOUT_REPEATE_COUNT || !UnityEngine.Application.isPlaying)
                {
                    AbortDownload();
                    throw new OperationCanceledException();
                }
                else
                {
                    AbortDownload();
                    await StartWebrequestGet(true);
                }
            }

            //�ɹ�����һ���ļ�
            if (this.DownloadModel.webRequest.downloadedBytes > 0)
            {
                this.DownloadModel.downloadedLength.Value += this.DownloadModel.webRequest.downloadedBytes;
                this.DownloadModel.downloadRepeatedCounter.Value = 0;
                if (this.DownloadModel.webRequest.result != UnityWebRequest.Result.Success)
                {
                    AbortDownload();
                    await StartWebrequestGet(true);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.DownloadModel.webRequest.error))
                {
                    CommonLog.NetError($"�ӵ�ַ {this.DownloadModel.downloadUrl} ������Դʧ��, ʧ��ԭ��: \n{this.DownloadModel.webRequest.error}");
                }
                else
                {
                    CommonLog.NetError($"�ӵ�ַ {this.DownloadModel.downloadUrl} ������Դʧ��, ʧ��ԭ��δ֪, �������: \n{this.DownloadModel.webRequest.result}");
                }

                this.DownloadModel.downloadRepeatedCounter.Value++;
                if (this.DownloadModel.downloadRepeatedCounter.Value >= TIMEOUT_REPEATE_COUNT)
                {
                    AbortDownload();
                    throw new OperationCanceledException();
                }
                else
                {
                    AbortDownload();
                    await StartWebrequestGet(true);
                }
            }

            return this.DownloadModel;
        }

        public void Report(float value)
        {
            if (!UnityEngine.Application.isPlaying)
            {
                AbortDownload();
                return;
            }
            this.DownloadModel.curDownloadedCompletedLength.Value = this.DownloadModel.webRequest.downloadedBytes;
            //����һ�����ֽڹر����ڵ�Э�̣������µ�Э�̣�����Դ�ֶ�����
            if (this.DownloadModel.curDownloadedCompletedLength.Value >= EACH_DOWNLOAD_BYTE_LENGTH
                || this.DownloadModel.curDownloadedProgressLength >= this.DownloadModel.downloadTotalLength.Value)
            {
                //ֹͣ��ǰ����
                this.DownloadModel.cancellationTokenSource.Cancel();
            }
        }

        public void Reset()
        {
            if (null == DownloadModel)
            {
                this.DownloadModel = new DownloadModel();
                this.DownloadModel.task = this;
                this.DownloadModel.downloadTotalLength = new AsyncReactiveProperty<ulong>(0);
                this.DownloadModel.downloadedLength = new AsyncReactiveProperty<ulong>(0);
                this.DownloadModel.curDownloadedCompletedLength = new AsyncReactiveProperty<ulong>(0);
                this.DownloadModel.downloadRepeatedCounter = new AsyncReactiveProperty<int>(0);
            }
        }
    }
}
