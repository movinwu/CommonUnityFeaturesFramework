using CommonFeatures.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.GML
{
    /// <summary>
    /// GameMainLoop
    /// ��Ϸ��ѭ��
    /// </summary>
    public class CommonFeature_GML : CommonFeature
    {
        /// <summary>
        /// ��Ϸ��ѭ��״̬��
        /// </summary>
        private FSM<CommonFeature_GML> m_FSM;

        public void StartGame()
        {
            //��ʽ��ʼ��Ϸ
            var states = new FSMState<CommonFeature_GML>[]
            {
                new FSMState_GML_StartGame(),
            };
            m_FSM = CommonFeaturesManager.FSM.CreateFSM(states, this);
            m_FSM.StartFSM<FSMState_GML_StartGame>();
        }
    }
}
