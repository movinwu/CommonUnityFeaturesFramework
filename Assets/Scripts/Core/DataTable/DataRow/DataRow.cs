using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// 数据基类
    /// </summary>
    public abstract class DataRow : IDataRow
    {
        public abstract int ID { get; }

        public abstract void FromBinary(BinaryReader br);
    }
}
