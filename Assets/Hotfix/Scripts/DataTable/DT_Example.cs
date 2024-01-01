//------------------------------------------------------------
// oops-framework-cocos2unity
// Copyright © 2024 movinwu. All rights reserved.
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2024/1/1 16:31:00
//------------------------------------------------------------

using CommonFeatures.DataTable;
using CommonFeatures.Log;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HotfixScripts
{
    /// <summary>
    /// 数据表基类
    /// </summary>
    public class DT_Example : IDataTable
    {
        /// <summary>
        /// 所有数据
        /// </summary>
        private Dictionary<int, DR_Example> m_AllDataDic;

        /// <summary>
        /// 所有数据
        /// </summary>
        private DR_Example[] m_AllDataArray;

        public void FromBinary(BinaryReader reader)
        {
            m_AllDataDic = new Dictionary<int, DR_Example>();
            int count = reader.ReadInt32();
            m_AllDataArray = new DR_Example[count];
            for (int i = 0; i < count; i++)
            {
                var dataRow = new DR_Example();
                dataRow.FromBinary(reader);
                m_AllDataDic.Add(dataRow.ID, dataRow);
                m_AllDataArray[i] = dataRow;
            }
        }

        public void FromJson(string json)
        {
            m_AllDataArray = JsonMapper.ToObject<DR_Example[]>(json);
            m_AllDataDic = new Dictionary<int, DR_Example>();
            for (int i = 0; i < m_AllDataArray.Length; i++)
            {
                m_AllDataDic.Add(m_AllDataArray[i].ID, m_AllDataArray[i]);
            }
        }

        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetDataRow<T>(int id) where T : DataRow
        {
            if (null == m_AllDataDic)
            {
                CommonLog.ConfigError($"读取表格 {typeof(T)} 时发现表格没有初始化");
                return default(T);
            }

            if (m_AllDataDic.TryGetValue(id, out var t))
            {
                return t as T;
            }
            return default(T);
        }

        /// <summary>
        /// 获取满足指定条件,按照指定顺序排列的所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<T> GetDataRows<T>(Predicate<T> predicate = null, Comparer<T> comparer = null) where T : DataRow
        {
            if (null == m_AllDataArray)
            {
                CommonLog.ConfigError($"读取表格 {typeof(T)} 时发现表格没有初始化");
                return new List<T>(0);
            }

            if (null == predicate && null == comparer)
            {
                return m_AllDataArray.Select(x => x as T).ToList();
            }
            else if (null == predicate)
            {
                var result = m_AllDataArray.Select(x => x as T).ToList();
                result.Sort(comparer);
                return result;
            }
            else if (null == comparer)
            {
                return m_AllDataArray.Select(x => x as T).Where(x => predicate(x)).ToList();
            }
            else
            {
                var result = m_AllDataArray.Select(x => x as T).Where(x => predicate(x)).ToList();
                result.Sort(comparer);
                return result;
            }
        }
    }
}
