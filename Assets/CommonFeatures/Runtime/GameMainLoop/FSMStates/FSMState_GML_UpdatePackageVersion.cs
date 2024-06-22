using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// 游戏主循环状态-更新包版本
    /// </summary>
    public class FSMState_GML_UpdatePackageVersion : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //初始化配置
            CommonLog.Resource("获取最新的资源版本 !");

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
