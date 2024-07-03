using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ��״̬-���°��汾
    /// </summary>
    public class FSMState_GML_UpdatePackageVersion : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //��ʼ������
            CommonLog.Resource("��ȡ���µ���Դ�汾 !");

            await UniTask.WaitForSeconds(0.5f);

            var blackboard = this.FSM.GetBlackboard<GameMainLoopBlackboard>();

            var operation = blackboard.Package.UpdatePackageVersionAsync();
            await UniTask.WaitUntil(() => operation.IsDone);

            if (operation.Status != EOperationStatus.Succeed)
            {
                CommonLog.ResourceError(operation.Error);
            }
            else
            {
                blackboard.PackageVersion = operation.PackageVersion;
                this.FSM.ChangeState<FSMState_GML_UpdatePackageManifest>();
            }
        }
    }
}
