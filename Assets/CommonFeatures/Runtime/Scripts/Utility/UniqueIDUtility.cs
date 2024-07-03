using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Utility
{
    /// <summary>
    /// 唯一id工具
    /// <para>实现简单唯一id生成</para>
    /// </summary>
    public static class UniqueIDUtility
    {
        private static ulong _uniqueID = 0;

        /// <summary>
        /// 生成唯一id
        /// </summary>
        /// <returns></returns>
        public static ulong GenerateUniqueID()
        {
            return ++_uniqueID;
        }
    }
}
