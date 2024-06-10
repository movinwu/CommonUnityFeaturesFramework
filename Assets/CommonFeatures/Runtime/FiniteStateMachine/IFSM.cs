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
        /// 销毁
        /// </summary>
        void OnDestroy();
    }
}
