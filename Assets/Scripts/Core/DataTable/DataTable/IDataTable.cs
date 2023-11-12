using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ���ݱ�
    /// </summary>
    public interface IDataTable
    {
        /// <summary>
        /// ��byte��ȡ
        /// </summary>
        /// <param name="reader"></param>
        void FromBinary(BinaryReader reader);

        /// <summary>
        /// ��json��ȡ
        /// </summary>
        /// <param name="json"></param>
        void FromJson(string json);
    }
}
