using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ��״̬-�����ļ�������
    /// </summary>
    public class FSMState_GML_CreatePackageDownloader : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //��ʼ������
            CommonLog.Resource("����������������");

            await UniTask.WaitForSeconds(0.5f);

            var blackboard = this.FSM.GetBlackboard<GameMainLoopBlackboard>();

            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = blackboard.Package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            blackboard.Downloader = downloader;

            if (downloader.TotalDownloadCount == 0)
            {
                CommonLog.Resource("Not found any download files !");
                this.FSM.ChangeState<FSMState_GML_StartGame>();
            }
            else
            {
                // �����¸����ļ��󣬹�������ϵͳ
                // ע�⣺��������Ҫ������ǰ�����̿ռ䲻��
                //int totalDownloadCount = downloader.TotalDownloadCount;
                //long totalDownloadBytes = downloader.TotalDownloadBytes;

                this.FSM.ChangeState<FSMState_GML_DownloadPackageFiles>();
            }
        }
    }
}
