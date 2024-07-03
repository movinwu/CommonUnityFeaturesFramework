using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ��״̬-�����ļ�������
    /// </summary>
    public class FSMState_GML_DownloadPackageFiles : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //��ʼ������
            CommonLog.Resource("��ʼ���ز����ļ���");

            var blackboard = this.FSM.GetBlackboard<GameMainLoopBlackboard>();

            blackboard.Downloader.BeginDownload();
            await UniTask.WaitUntil(() => blackboard.Downloader.IsDone);

            // ������ؽ��
            if (blackboard.Downloader.Status != EOperationStatus.Succeed)
            {
                CommonLog.ResourceError("���ز����ļ�ʧ��");
                return;
            }

            this.FSM.ChangeState<FSMState_GML_StartGame>();
        }
    }
}
