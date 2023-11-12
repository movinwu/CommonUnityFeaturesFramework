using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// 数据表管理器
    /// </summary>
    public class DataTableManager : SingletonBase<DataTableManager>
    {
        private DataTableManager() { }

        /// <summary>
        /// byte文件路径
        /// </summary>
        private const string BinaryPath = "Assets/Hotfix/Resources/Data/Binary";

        /// <summary>
        /// json文件路径
        /// </summary>
        private const string JsonPath = "Assets/Hotfix/Resources/Data/Json";

        private EDataReadType m_DataReadType = EDataReadType.Binary;

        /// <summary>
        /// 所有数据表
        /// </summary>
        private Dictionary<System.Type, IDataTable> m_AllDataTable;

        /// <summary>
        /// 读取数据表数据
        /// </summary>
        public void ReadDataTable()
        {
            if (m_DataReadType == EDataReadType.Binary)
            {
                var directory = new DirectoryInfo(BinaryPath);
                var files = directory.GetFiles();
                for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                {
                    var file = files[fileIndex];
                    if (file.Name.EndsWith(".byte"))
                    {
                        string name = file.Name.Substring(0, file.Name.LastIndexOf('.'));
                        string typeName = $"OOPS.DataTable<DT_{name}>";

                    }
                }
            }
            else if (m_DataReadType == EDataReadType.Json)
            {

            }
        }

        /// <summary>
        /// 获取单行数据表数据
        /// </summary>
        /// <param name="id"></param>
        public void GetDataRow(int id)
        {

        }

        /// <summary>
        /// 获取所有数据表数据
        /// </summary>
        public void GetAllDataRow()
        {

        }

        /// <summary>
        /// 数据表读取类型
        /// </summary>
        private enum EDataReadType : byte
        {
            Binary,

            Json,
        }

    }
}
