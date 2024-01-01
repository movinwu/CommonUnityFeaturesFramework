using CommonFeatures.FSM;
using CommonFeatures.Log;

namespace CommonFeatures.GML
{
    /// <summary>
    /// 游戏主循环状态-开始游戏
    /// </summary>
    public class FSMState_GML_StartGame : FSMState<CommonFeature_GML>
    {
        public override void OnEnter()
        {
            base.OnEnter();

            //初始化配置
            CommonLog.Log("开始游戏");
        }
    }
}
