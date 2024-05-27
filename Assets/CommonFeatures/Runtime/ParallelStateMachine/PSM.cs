using CommonFeatures.Log;
using CommonFeatures.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// 并行状态机
    /// </summary>
    public class PSM<T> : IPSM
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public ulong UniqueId { get; private set; }

        /// <summary>
        /// 状态机的所有状态
        /// </summary>
        private Dictionary<System.Type, PSMState<T>> m_AllStates = new Dictionary<System.Type, PSMState<T>>();

        /// <summary>
        /// 状态机持有者
        /// </summary>
        public T Owner { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="states">包含的有限状态机</param>
        /// <param name="owner">状态机持有者</param>
        internal PSM(PSMState<T>[] states, T owner)
        {
            if (null == states || states.Length == 0)
            {
                CommonLog.LogError("状态机必须包含一系列状态");
            }

            this.Owner = owner;
            this.UniqueId = UniqueIDUtility.GenerateUniqueID();

            for (int i = 0; i < states.Length; i++)
            {
                var type = states[i].GetType();
                if (m_AllStates.ContainsKey(type))
                {
                    CommonLog.LogError("重复向状态机中添加状态");
                    continue;
                }
                else
                {
                    m_AllStates.Add(type, states[i]);
                    states[i].Init(this);
                }
            }
        }

        /// <summary>
        /// 开始状态机
        /// </summary>
        public void StartPSM(List<PSMState<T>> startStates)
        {
            if (null == m_AllStates || m_AllStates.Count == 0)
            {
                CommonLog.LogError($"状态机没有初始化，不能开始运行");
                return;
            }
            if (m_AllStates.Values.Where(x => x.State == EPSMState.Running).Count() > 0)
            {
                CommonLog.LogError($"状态机已经开始运行,不能重复开始运行状态机");
                return;
            }
            if (null == startStates || startStates.Count == 0)
            {
                CommonLog.LogError($"状态机初始状态数量为0，不能开始运行");
                return;
            }

            //初始化
            foreach (var s in m_AllStates.Values)
            {
                s.State = EPSMState.NotRunning;
                s.OnInit();
            }

            for (int i = 0; i < startStates.Count; i++)
            {
                var startState = startStates[i];
                var startStateType = startState.GetType();
                if (m_AllStates.ContainsKey(startStateType))
                {
                    startState.StartStateRunning(null);
                }
            }
        }

        public void OnTick()
        {
            var curStates = m_AllStates.Values.Where(x => x.State == EPSMState.Running);
            foreach (var state in curStates)
            {
                state.OnTick();
            }
        }

        public void OnDestroy()
        {
            foreach (var state in m_AllStates.Values)
            {
                state.OnDestroy();
            }
            m_AllStates.Clear();
        }
    }
}
