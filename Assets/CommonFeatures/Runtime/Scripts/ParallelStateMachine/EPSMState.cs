using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.PSM
{
    public enum EPSMState : byte
    {
        /// <summary>
        /// 准备运行
        /// </summary>
        PrepareToRun,

        /// <summary>
        /// 运行中
        /// </summary>
        Running,

        /// <summary>
        /// 结束运行
        /// </summary>
        Ran,
    }
}
