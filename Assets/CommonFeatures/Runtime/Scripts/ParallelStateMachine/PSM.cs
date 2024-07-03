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
    /// 并行状态机
    /// <para>不支持循环运行,每一轮运行完成后需要重置所有状态</para>
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
                }
            }
        }

        /// <summary>
        /// 重置所有状态
        /// <para>状态机不支持成环运行,经过一轮运行后必须重置所有状态,才能继续下一轮运行</para>
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
        /// 进入一个状态运行(启动状态机一轮运行时使用)
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
        /// 获取一个状态
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

            CommonLog.LogError($"获取状态失败,尝试获取不存在的状态: {type}");
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
