using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace CommonFeatures.GML
{
    /// <summary>
    /// 游戏主循环状态-创建文件下载器
    /// </summary>
    public class FSMState_GML_CreatePackageDownloader : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //初始化配置
            CommonLog.Resource("创建补丁下载器！");

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
                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                //int totalDownloadCount = downloader.TotalDownloadCount;
                //long totalDownloadBytes = downloader.TotalDownloadBytes;

                this.FSM.ChangeState<FSMState_GML_DownloadPackageFiles>();
            }
        }
    }
}
