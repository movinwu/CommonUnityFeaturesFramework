using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.FSM
{
    /// <summary>
    /// 有限状态机接口
    /// </summary>
    internal interface IFSM
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
