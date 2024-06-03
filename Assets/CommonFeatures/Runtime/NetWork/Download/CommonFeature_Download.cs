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
