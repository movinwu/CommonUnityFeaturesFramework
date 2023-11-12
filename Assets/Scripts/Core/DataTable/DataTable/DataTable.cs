using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ���ݱ����
    /// </summary>
    public class DataTable<T> : IDataTable where T : IDataRow, new()
    {
        /// <summary>
        /// ��������
        /// </summary>
        private Dictionary<int, T> m_AllDataDic;

        /// <summary>
        /// ��������
        /// </summary>
        private T[] m_AllDataArray;

        public void FromBinary(BinaryReader reader)
        {
            m_AllDataDic = new Dictionary<int, T>();
            int count = reader.ReadInt32();
            m_AllDataArray = new T[count];
            for (int i = 0; i < count; i++)
            {
                var dataRow = new T();
                dataRow.FromBinary(reader);
                m_AllDataDic.Add(dataRow.ID, dataRow);
                m_AllDataArray[i] = dataRow;
            }
        }

        public void FromJson(string json)
        {
            m_AllDataArray = JsonMapper.ToObject<T[]>(json);
            m_AllDataDic = new Dictionary<int, T>();
            for (int i = 0; i < m_AllDataArray.Length; i++)
            {
                m_AllDataDic.Add(m_AllDataArray[i].ID, m_AllDataArray[i]);
            }
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetDataRow(int id)
        {
            if (null == m_AllDataDic)
            {
                Logger.ModelError($"��ȡ��� {typeof(T)} ʱ���ֱ��û�г�ʼ��");
                return default(T);
            }

            if (m_AllDataDic.TryGetValue(id, out var t))
            {
                return t;
            }
            return default(T);
        }

        /// <summary>
        /// ��ȡ����ָ������,����ָ��˳�����е���������
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<T> GetDataRows(Predicate<T> predicate = null, Comparer<T> comparer = null)
        {
            if (null == m_AllDataArray)
            {
                Logger.ModelError($"��ȡ��� {typeof(T)} ʱ���ֱ��û�г�ʼ��");
                return new List<T>(0);
            }

            if (null == predicate && null == comparer)
            {
                return m_AllDataArray.ToList();
            }
            else if (null == predicate)
            {
                var result = m_AllDataArray.ToList();
                result.Sort(comparer);
                return result;
            }
            else if (null == comparer)
            {
                return m_AllDataArray.Where(x => predicate(x)).ToList();
            }
            else
            {
                var result = m_AllDataArray.Where(x => predicate(x)).ToList();
                result.Sort(comparer);
                return result;
            }
        }
    }
}
