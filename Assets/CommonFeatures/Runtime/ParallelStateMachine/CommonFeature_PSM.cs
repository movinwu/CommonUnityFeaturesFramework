using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// 通用功能,并行状态机
    /// </summary>
    public class CommonFeature_PSM : CommonFeature
    {
        /// <summary>
        /// 所有状态机
        /// </summary>
        private Dictionary<ulong, IPSM> m_AllPSM = new Dictionary<ulong, IPSM>();

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="states"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public PSM<T> CreatePSM<T>(PSMState<T>[] states, T owner)
        {
            var psm = new PSM<T>(states, owner);
            m_AllPSM.Add(psm.UniqueId, psm);
            return psm;
        }

        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="psm"></param>
        public void DestroyPSM<T>(PSM<T> psm)
        {
            if (!m_AllPSM.ContainsKey(psm.UniqueId))
            {
                CommonLog.LogError("状态机不存在");
                return;
            }

            m_AllPSM.Remove(psm.UniqueId);
            psm.OnDestroy();
        }

        public override void Release()
        {
            base.Release();

            foreach (var psm in m_AllPSM.Values)
            {
                psm.OnDestroy();
            }
            m_AllPSM.Clear();
        }
    }
}

