using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CommonFeatures.DataTable
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

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetDataRow<T>(int id) where T : DataRow;

        /// <summary>
        /// ��ȡ����ָ������,����ָ��˳�����е���������
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        List<T> GetDataRows<T>(Predicate<T> predicate = null, Comparer<T> comparer = null) where T : DataRow;
    }
}
