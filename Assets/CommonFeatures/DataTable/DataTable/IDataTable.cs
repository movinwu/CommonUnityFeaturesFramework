using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CommonFeatures.DataTable
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

        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetDataRow<T>(int id) where T : DataRow;

        /// <summary>
        /// 获取满足指定条件,按照指定顺序排列的所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        List<T> GetDataRows<T>(Predicate<T> predicate = null, Comparer<T> comparer = null) where T : DataRow;
    }
}
