using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.FSM
{
    /// <summary>
    /// ����״̬���ӿ�
    /// </summary>
    internal interface IFSM
    {
        /// <summary>
        /// ÿ֡��ѯ
        /// </summary>
        void OnTick();

        /// <summary>
        /// ����
        /// </summary>
        void OnDestroy();
    }
}
