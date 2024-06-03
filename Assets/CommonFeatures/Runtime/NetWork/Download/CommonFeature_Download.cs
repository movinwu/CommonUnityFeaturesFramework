using CommonFeatures.Pool;
using Cysharp.Threading.Tasks;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通用功能-下载
    /// </summary>
    public class CommonFeature_Download : CommonFeature
    {
        /// <summary>
        /// 默认单个下载超时时间
        /// </summary>
        private const int DEFAULT_TIMEOUT = 5;

        public override void Init()
        {
        }

        /// <summary>
        /// 开始一个下载
        /// </summary>
        /// <param name="url">资源定位地址</param>
        /// <param name="saveDirectoryPath">保存地址</param>
        /// <param name="timeout">单个下载超时时间</param>
        /// <param name="forget">是否不等待下完全部完成</param>
        /// <returns>下载唯一id</returns>
        public async UniTask<DownloadModel> AddDownload(string url, string saveDirectoryPath, int timeout = DEFAULT_TIMEOUT, bool forget = true)
        {
            var downloadTask = ReferencePool.Acquire<DownloadTask>();

            await downloadTask.InitDownload(url, saveDirectoryPath, timeout);

            if (forget)
            {
                downloadTask
                    .StartDonwload()
                    .Forget();
            }
            else
            {
                return await downloadTask
                    .StartDonwload();
            }

            return downloadTask.DownloadModel;
        }

        /// <summary>
        /// 中断一个下载
        /// </summary>
        /// <param name="downloadModel"></param>
        public void AbortDownload(DownloadModel downloadModel)
        {
            if (null != downloadModel.task && downloadModel.downloadState == EDownloadState.Downloading)
            {
                downloadModel.task.AbortDownload();
            }
        }
    }
}
