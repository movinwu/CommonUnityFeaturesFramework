using CommonFeatures.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.GML
{
    /// <summary>
    /// GameMainLoop
    /// 游戏主循环
    /// </summary>
    public class CommonFeature_GML : CommonFeature
    {
        /// <summary>
        /// 游戏主循环状态机
        /// </summary>
        private FSM<CommonFeature_GML> m_FSM;

        public void StartGame()
        {
            //正式开始游戏
            var states = new FSMState<CommonFeature_GML>[]
            {
                new FSMState_GML_StartGame(),
            };
            m_FSM = CommonFeaturesManager.FSM.CreateFSM(states, this);
            m_FSM.StartFSM<FSMState_GML_StartGame>();
        }
    }
}
