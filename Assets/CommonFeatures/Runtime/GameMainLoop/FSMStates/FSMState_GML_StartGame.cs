using CommonFeatures.FSM;
using CommonFeatures.Log;

namespace CommonFeatures.GML
{
    /// <summary>
    /// ��Ϸ��ѭ��״̬-��ʼ��Ϸ
    /// </summary>
    public class FSMState_GML_StartGame : FSMState<CommonFeature_GML>
    {
        public override void OnEnter()
        {
            base.OnEnter();

            //��ʼ������
            CommonLog.Log("��ʼ��Ϸ");
        }
    }
}
