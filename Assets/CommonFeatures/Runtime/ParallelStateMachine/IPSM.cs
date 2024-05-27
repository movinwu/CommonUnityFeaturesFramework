using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// 并行状态机接口
    /// </summary>
    internal interface IPSM
    {
        /// <summary>
        /// 每帧轮询
        /// </summary>
        void OnTick();

        /// <summary>
        /// 销毁
        /// </summary>
        void OnDestroy();
    }
}
