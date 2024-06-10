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
    /// ����״̬��
    /// </summary>
    public class FSM<T> : IFSM
    {
        /// <summary>
        /// Ψһid
        /// </summary>
        public ulong UniqueId { get; private set; }

        /// <summary>
        /// ״̬��������״̬
        /// </summary>
        private Dictionary<System.Type, FSMState<T>> m_AllStates = new Dictionary<System.Type, FSMState<T>>();

        /// <summary>
        /// ��ǰ״̬
        /// </summary>
        private FSMState<T> m_CurrentState = null;

        /// <summary>
        /// ״̬��������
        /// </summary>
        public T Owner { get; private set; }

        /// <summary>
        /// �Ƿ������л�״̬
        /// </summary>
        private bool m_IsChangeState = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="states">����������״̬��</param>
        /// <param name="owner">״̬��������</param>
        internal FSM(FSMState<T>[] states, T owner)
        {
            if (null == states || states.Length == 0)
            {
                CommonLog.LogError("״̬���������һϵ��״̬");
            }

            this.Owner = owner;
            this.UniqueId = UniqueIDUtility.GenerateUniqueID();

            for (int i = 0; i < states.Length; i++)
            {
                var type = states[i].GetType();
                if (m_AllStates.ContainsKey(type))
                {
                    CommonLog.LogError("�ظ���״̬�������״̬");
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
        /// ��ʼ״̬��
        /// </summary>
        public async UniTask StartFSM<K>() where K : FSMState<T>
        {
            if (null != m_CurrentState)
            {
                CommonLog.LogError($"״̬���Ѿ���ʼ����,�����ظ���ʼ����״̬��");
                return;
            }

            var type = typeof(K);
            if (m_AllStates.ContainsKey(type))
            {
                //��ʼ��
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
        /// �л�״̬
        /// </summary>
        /// <typeparam name="K"></typeparam>
        public async UniTask ChangeState<K>() where K : FSMState<T>
        {
            if (null == m_CurrentState)
            {
                CommonLog.LogError($"״̬��û�г�ʼ��, �޷��л�״̬");
                return;
            }

            if (m_IsChangeState)
            {
                CommonLog.LogError("״̬���л�״̬�ݹ�,�벻Ҫ��OnLeave�������л�״̬");
                return;
            }

            var type = typeof(K);
            if (!m_AllStates.ContainsKey(type))
            {
                CommonLog.LogError($"��ͼ�л��������ڵ�״̬: {type}");
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
