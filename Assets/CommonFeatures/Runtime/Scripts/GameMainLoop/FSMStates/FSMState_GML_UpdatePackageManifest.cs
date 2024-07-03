using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// 游戏主循环状态-更新包清单
    /// </summary>
    public class FSMState_GML_UpdatePackageManifest : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //初始化配置
            CommonLog.Resource("更新资源清单！"); 
            
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
