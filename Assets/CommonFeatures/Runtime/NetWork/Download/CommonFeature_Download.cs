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
            var downloadModel = await InitDownload(url, saveDirectoryPath, timeout);

            if (forget)
            {
                StartDownload(downloadModel).Forget();
            }
            else
            {

                await StartDownload(downloadModel);
            }

            return downloadModel;
        }

        /// <summary>
        /// 初始化下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="saveDirectoryPath"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async UniTask<DownloadModel> InitDownload(string url, string saveDirectoryPath, int timeout = DEFAULT_TIMEOUT)
        {
            var downloadTask = ReferencePool.Acquire<DownloadTask>();

            return await downloadTask.InitDownload(url, saveDirectoryPath, timeout);
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="downloadModel"></param>
        /// <param name="forget"></param>
        /// <returns></returns>
        public async UniTask<DownloadModel> StartDownload(DownloadModel downloadModel)
        {
            if (null == downloadModel)
            {
                return downloadModel;
            }

            return await downloadModel.task
                .StartDonwload();
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
