using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Utility
{
    /// <summary>
    /// Ψһid����
    /// <para>ʵ�ּ�Ψһid����</para>
    /// </summary>
    public static class UniqueIDUtility
    {
        private static ulong _uniqueID = 0;

        /// <summary>
        /// ����Ψһid
        /// </summary>
        /// <returns></returns>
        public static ulong GenerateUniqueID()
        {
            return ++_uniqueID;
        }
    }
}
