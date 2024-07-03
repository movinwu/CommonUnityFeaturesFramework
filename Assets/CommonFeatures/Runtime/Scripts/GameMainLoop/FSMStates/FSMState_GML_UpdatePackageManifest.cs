using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ��״̬-���°��嵥
    /// </summary>
    public class FSMState_GML_UpdatePackageManifest : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //��ʼ������
            CommonLog.Resource("������Դ�嵥��"); 
            
            await UniTask.WaitForSeconds(0.5f);

            var blackboard = this.FSM.GetBlackboard<GameMainLoopBlackboard>();

            bool savePackageVersion = true;
            var operation = blackboard.Package.UpdatePackageManifestAsync(blackboard.PackageVersion, savePackageVersion);

            await UniTask.WaitUntil(() => operation.IsDone);

            if (operation.Status != EOperationStatus.Succeed)
            {
                CommonLog.ResourceError(operation.Error);
            }
            else
            {
                this.FSM.ChangeState<FSMState_GML_CreatePackageDownloader>();
            }
        }
    }
}
