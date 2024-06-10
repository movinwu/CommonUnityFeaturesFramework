using CommonFeatures.Log;
using CommonFeatures.Utility;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// ����״̬��
    /// <para>��֧��ѭ������,ÿһ��������ɺ���Ҫ��������״̬</para>
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
                }
            }
        }

        /// <summary>
        /// ��������״̬
        /// <para>״̬����֧�ֳɻ�����,����һ�����к������������״̬,���ܼ�����һ������</para>
        /// </summary>
        public async UniTask ResetAllPSMState()
        {
            var array = this.m_AllStates.Values.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                await array[i].Init(this);
            }
        }

        /// <summary>
        /// ����һ��״̬����(����״̬��һ������ʱʹ��)
        /// </summary>
        /// <typeparam name="K"></typeparam>
        public async UniTask EnterState<K>() where K : PSMState<T>
        {
            var type = typeof(K);
            if (m_AllStates.ContainsKey(type))
            {
                await m_AllStates[type].Enter();
            }
        }

        /// <summary>
        /// ��ȡһ��״̬
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K GetState<K>() where K : PSMState<T>
        {
            var type = typeof(K);
            if (m_AllStates.ContainsKey(type))
            {
                return m_AllStates[type] as K;
            }

            CommonLog.LogError($"��ȡ״̬ʧ��,���Ի�ȡ�����ڵ�״̬: {type}");
            return null;
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
