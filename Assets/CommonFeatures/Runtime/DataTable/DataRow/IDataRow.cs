using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CommonFeatures.DataTable
{
    /// <summary>
    /// ��������
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// ÿ�����ݵ�id
        /// </summary>
        int ID { get; }

        /// <summary>
        /// ��byte��ȡ����
        /// </summary>
        /// <param name="br"></param>
        void FromBinary(BinaryReader br);
    }
}
