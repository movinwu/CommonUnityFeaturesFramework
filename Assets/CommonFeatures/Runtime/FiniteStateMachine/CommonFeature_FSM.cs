using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.FSM
{
    /// <summary>
    /// 通用功能-有限状态机
    /// </summary>
    public class CommonFeature_FSM : CommonFeature
    {
        /// <summary>
        /// 所有状态机
        /// </summary>
        private Dictionary<ulong, IFSM> m_AllFSM = new Dictionary<ulong, IFSM>();

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="states"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public FSM<T> CreateFSM<T>(FSMState<T>[] states, T owner)
        {
            var fsm = new FSM<T>(states, owner);
            m_AllFSM.Add(fsm.UniqueId, fsm);
            return fsm;
        }

        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fsm"></param>
        public void DestroyFSM<T>(FSM<T> fsm)
        {
            if (!m_AllFSM.ContainsKey(fsm.UniqueId))
            {
                CommonLog.LogError("状态机不存在");
                return;
            }

            m_AllFSM.Remove(fsm.UniqueId);
            fsm.OnDestroy();
        }

        public override void Release()
        {
            base.Release();

            foreach(var fsm in m_AllFSM.Values)
            {
                fsm.OnDestroy();
            }
            m_AllFSM.Clear();
        }
    }
}
