using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ���ݻ���
    /// </summary>
    public abstract class DataRow : IDataRow
    {
        public abstract int ID { get; }

        public abstract void FromBinary(BinaryReader br);
    }
}
