using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CommonFeatures.DataTable
{
    /// <summary>
    /// 单行数据
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// 每行数据的id
        /// </summary>
        int ID { get; }

        /// <summary>
        /// 从byte读取数据
        /// </summary>
        /// <param name="br"></param>
        void FromBinary(BinaryReader br);
    }
}
