using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// 引用缓存池对象接口
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// 重置函数
        /// </summary>
        void Reset();
    }
}
