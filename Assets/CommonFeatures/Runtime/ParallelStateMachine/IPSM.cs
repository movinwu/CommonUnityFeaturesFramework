using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// ����״̬���ӿ�
    /// </summary>
    internal interface IPSM
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
