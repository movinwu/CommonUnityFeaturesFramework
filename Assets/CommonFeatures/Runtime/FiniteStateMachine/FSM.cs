using CommonFeatures.Log;
using CommonFeatures.Utility;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.FSM
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FSM<T> : IFSM
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public ulong UniqueId { get; private set; }

        /// <summary>
        /// 状态机的所有状态
        /// </summary>
        private Dictionary<System.Type, FSMState<T>> m_AllStates = new Dictionary<System.Type, FSMState<T>>();

        /// <summary>
        /// 当前状态
        /// </summary>
        private FSMState<T> m_CurrentState = null;

        /// <summary>
        /// 状态机持有者
        /// </summary>
        public T Owner { get; private set; }

        /// <summary>
        /// 是否正在切换状态
        /// </summary>
        private bool m_IsChangeState = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="states">包含的有限状态机</param>
        /// <param name="owner">状态机持有者</param>
        internal FSM(FSMState<T>[] states, T owner)
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
        public async UniTask StartFSM<K>() where K : FSMState<T>
        {
            if (null != m_CurrentState)
            {
                CommonLog.LogError($"状态机已经开始运行,不能重复开始运行状态机");
                return;
            }

            var type = typeof(K);
            if (m_AllStates.ContainsKey(type))
            {
                //初始化
                var array = m_AllStates.Values.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    await array[i].OnInit();
                }

                var state = m_AllStates[type];
                this.m_CurrentState = state;
                await state.OnEnter();
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="K"></typeparam>
        public async UniTask ChangeState<K>() where K : FSMState<T>
        {
            if (null == m_CurrentState)
            {
                CommonLog.LogError($"状态机没有初始化, 无法切换状态");
                return;
            }

            if (m_IsChangeState)
            {
                CommonLog.LogError("状态机切换状态递归,请不要再OnLeave函数中切换状态");
                return;
            }

            var type = typeof(K);
            if (!m_AllStates.ContainsKey(type))
            {
                CommonLog.LogError($"试图切换到不存在的状态: {type}");
                return;
            }

            var newState = m_AllStates[type];
            m_IsChangeState = true;
            await m_CurrentState.OnLeave();
            m_IsChangeState = false;
            m_CurrentState = newState;
            await newState.OnEnter();
        }

        public void OnDestroy()
        {
            foreach (var state in m_AllStates.Values)
            {
                state.OnDestroy();
            }
            m_CurrentState = null;
        }
    }
}
