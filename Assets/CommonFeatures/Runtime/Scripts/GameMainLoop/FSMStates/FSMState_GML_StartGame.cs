using CommonFeatures.FSM;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ��״̬-��ʼ��Ϸ
    /// </summary>
    public class FSMState_GML_StartGame : FSMState<CommonFeature_GML>
    {
        public override async UniTask OnEnter()
        {
            await base.OnEnter();

            //��ʼ������
            CommonLog.Log("��ʼ��Ϸ");
        }
    }
}
