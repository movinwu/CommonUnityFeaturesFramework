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
    /// 下载任务类
    /// </summary>
    public class DownloadTask : IProgress<float>, IReference
    {
        internal DownloadModel DownloadModel { get; private set; }

        /// <summary>
        /// 每次下载数据比特长度
        /// <para>8 * 1024 * 512 = 4194304 比特,即512KB</para>
        /// </summary>
        private const ulong EACH_DOWNLOAD_BYTE_LENGTH = 4194304;

        /// <summary>
        /// 中断后重新请求次数
        /// </summary>
        private const int TIMEOUT_REPEATE_COUNT = 3;

        /// <summary>
        /// 初始化一个下载
        /// </summary>
        /// <param name="url">资源定位地址</param>
        /// <param name="saveDirectoryPath">保存地址</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>下载唯一id</returns>
        internal async UniTask<DownloadModel> InitDownload(string url, string saveDirectoryPath, int timeout)
        {
            if (!Directory.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
            }

            var fileName = url.Substring(url.LastIndexOf('/') + 1);
            //没有下载完成的文件以.download作为临时后缀
            saveDirectoryPath = Path.Combine(saveDirectoryPath, $"{fileName}.download");

            if (!File.Exists(saveDirectoryPath))
            {
                File.WriteAllText(saveDirectoryPath, string.Empty);//创建空文件
            }
            this.DownloadModel.downloadUrl = url;
            this.DownloadModel.savePath = saveDirectoryPath;
            this.DownloadModel.timeout = timeout;

            await RequestDownloadInfo();

            return this.DownloadModel;
        }

        /// <summary>
        /// 开始下载
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
        /// 中断下载
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
        /// 请求下载相关信息
        /// </summary>
        /// <returns></returns>
        private async UniTask<DownloadModel> RequestDownloadInfo()
        {
            //UnityWebRequest 经配置可传输 HTTP HEAD 请求的 UnityWebRequest。
            var request = await CommonFeaturesManager.Http.Head(this.DownloadModel.downloadUrl, null);
            //成功加载头文件
            if (request.result == UnityWebRequest.Result.Success)
            {
                //获取文件总大小
                this.DownloadModel.downloadTotalLength.Value = ulong.Parse(request.GetResponseHeader("Content-Length"));
                request.Dispose();
                this.DownloadModel.downloadRepeatedCounter.Value = 0;
            }
            //加载头文件失败
            else if (!string.IsNullOrEmpty(request.error))
            {
                CommonLog.NetError($"从地址 {this.DownloadModel.downloadUrl} 获取下载资源文件头失败, 失败原因: \n{request.error}");
                request.Dispose();
                throw new OperationCanceledException();

            }
            //未知未成功异常
            else
            {
                CommonLog.NetError($"从地址 {this.DownloadModel.downloadUrl} 获取下载资源文件头失败, 失败原因未知, 结果类型: \n{request.result}");
                request.Dispose();
                throw new OperationCanceledException();
            }

            return this.DownloadModel;
        }

        /// <summary>
        /// 分段，断点下载文件
        /// </summary>
        /// <returns></returns>
        private async UniTask<DownloadModel> BreakpointResume()
        {
            if (this.DownloadModel.downloadTotalLength.Value > 0)
            {
                //重命名文件
                var newSavePath = this.DownloadModel.savePath.Substring(0, this.DownloadModel.savePath.LastIndexOf('.'));
                //检查是否已有文件
                if (File.Exists(newSavePath))
                {
                    var fileInfo = new FileInfo(newSavePath);
                    if (this.DownloadModel.downloadTotalLength == (ulong)fileInfo.Length)
                    {
                        this.DownloadModel.downloadedLength.Value = this.DownloadModel.downloadTotalLength.Value;
                        //移除临时下载文件
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
        /// 开始一个unitywebrequest的get函数下载
        /// </summary>
        /// <param name="isContinue"></param>
        /// <returns></returns>
        private async UniTask<DownloadModel> StartWebrequestGet(bool isContinue)
        {
            if (!isContinue)
            {
                //创建文件
                FileInfo file = new FileInfo(this.DownloadModel.savePath);
                //当前下载的文件长度
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
            //请求网络数据从第fileLength到最后的字节
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
                    CommonLog.NetError($"从地址 {this.DownloadModel.downloadUrl} 下载资源失败, 失败原因: \n{this.DownloadModel.webRequest.error}");
                }
                else
                {
                    CommonLog.NetError($"从地址 {this.DownloadModel.downloadUrl} 下载资源失败, 失败原因未知, 结果类型: \n{this.DownloadModel.webRequest?.result}");
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

            //成功下载一次文件
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
                    CommonLog.NetError($"从地址 {this.DownloadModel.downloadUrl} 下载资源失败, 失败原因: \n{this.DownloadModel.webRequest.error}");
                }
                else
                {
                    CommonLog.NetError($"从地址 {this.DownloadModel.downloadUrl} 下载资源失败, 失败原因未知, 结果类型: \n{this.DownloadModel.webRequest.result}");
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
            //超过一定的字节关闭现在的协程，开启新的协程，将资源分段下载
            if (this.DownloadModel.curDownloadedCompletedLength.Value >= EACH_DOWNLOAD_BYTE_LENGTH
                || this.DownloadModel.curDownloadedProgressLength >= this.DownloadModel.downloadTotalLength.Value)
            {
                //停止当前下载
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
