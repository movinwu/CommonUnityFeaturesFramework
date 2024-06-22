using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// 游戏主循环状态-创建文件下载器
    /// </summary>
    public class FSMState_GML_DownloadPackageFiles : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //初始化配置
            CommonLog.Resource("开始下载补丁文件！");

            var blackboard = this.FSM.GetBlackboard<GameMainLoopBlackboard>();

            blackboard.Downloader.BeginDownload();
            await UniTask.WaitUntil(() => blackboard.Downloader.IsDone);

            // 检测下载结果
            if (blackboard.Downloader.Status != EOperationStatus.Succeed)
            {
                CommonLog.ResourceError("下载补丁文件失败");
                return;
            }

            this.FSM.ChangeState<FSMState_GML_StartGame>();
        }
    }
}
