using CommonFeatures.Pool;
using Cysharp.Threading.Tasks;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// ͨ�ù���-����
    /// </summary>
    public class CommonFeature_Download : CommonFeature
    {
        /// <summary>
        /// Ĭ�ϵ������س�ʱʱ��
        /// </summary>
        private const int DEFAULT_TIMEOUT = 5;

        public override void Init()
        {
        }

        /// <summary>
        /// ��ʼһ������
        /// </summary>
        /// <param name="url">��Դ��λ��ַ</param>
        /// <param name="saveDirectoryPath">�����ַ</param>
        /// <param name="timeout">�������س�ʱʱ��</param>
        /// <param name="forget">�Ƿ񲻵ȴ�����ȫ�����</param>
        /// <returns>����Ψһid</returns>
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
        /// ��ʼ������
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
        /// ��ʼ����
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
        /// �ж�һ������
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
