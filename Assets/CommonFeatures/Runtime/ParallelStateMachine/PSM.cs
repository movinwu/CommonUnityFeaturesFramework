using CommonFeatures.Log;
using CommonFeatures.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// ����״̬��
    /// </summary>
    public class PSM<T> : IPSM
    {
        /// <summary>
        /// Ψһid
        /// </summary>
        public ulong UniqueId { get; private set; }

        /// <summary>
        /// ״̬��������״̬
        /// </summary>
        private Dictionary<System.Type, PSMState<T>> m_AllStates = new Dictionary<System.Type, PSMState<T>>();

        /// <summary>
        /// ״̬��������
        /// </summary>
        public T Owner { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="states">����������״̬��</param>
        /// <param name="owner">״̬��������</param>
        internal PSM(PSMState<T>[] states, T owner)
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
        public void StartPSM(List<PSMState<T>> startStates)
        {
            if (null == m_AllStates || m_AllStates.Count == 0)
            {
                CommonLog.LogError($"״̬��û�г�ʼ�������ܿ�ʼ����");
                return;
            }
            if (m_AllStates.Values.Where(x => x.State == EPSMState.Running).Count() > 0)
            {
                CommonLog.LogError($"״̬���Ѿ���ʼ����,�����ظ���ʼ����״̬��");
                return;
            }
            if (null == startStates || startStates.Count == 0)
            {
                CommonLog.LogError($"״̬����ʼ״̬����Ϊ0�����ܿ�ʼ����");
                return;
            }

            //��ʼ��
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
