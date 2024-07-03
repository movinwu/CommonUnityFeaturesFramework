using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// ͨ�ù���,����״̬��
    /// </summary>
    public class CommonFeature_PSM : CommonFeature
    {
        /// <summary>
        /// ����״̬��
        /// </summary>
        private Dictionary<ulong, IPSM> m_AllPSM = new Dictionary<ulong, IPSM>();

        /// <summary>
        /// ����״̬��
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
        /// ����״̬��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="psm"></param>
        public void DestroyPSM<T>(PSM<T> psm)
        {
            if (!m_AllPSM.ContainsKey(psm.UniqueId))
            {
                CommonLog.LogError("״̬��������");
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

