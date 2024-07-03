using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.PSM
{
    public enum EPSMState : byte
    {
        /// <summary>
        /// ׼������
        /// </summary>
        PrepareToRun,

        /// <summary>
        /// ������
        /// </summary>
        Running,

        /// <summary>
        /// ��������
        /// </summary>
        Ran,
    }
}
