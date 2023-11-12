using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// 数据表
    /// </summary>
    public interface IDataTable
    {
        /// <summary>
        /// 从byte读取
        /// </summary>
        /// <param name="reader"></param>
        void FromBinary(BinaryReader reader);

        /// <summary>
        /// 从json读取
        /// </summary>
        /// <param name="json"></param>
        void FromJson(string json);
    }
}
