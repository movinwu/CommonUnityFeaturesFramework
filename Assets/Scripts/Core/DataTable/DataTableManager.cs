using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ���ݱ������
    /// </summary>
    public class DataTableManager : SingletonBase<DataTableManager>
    {
        private DataTableManager() { }

        /// <summary>
        /// byte�ļ�·��
        /// </summary>
        private const string BinaryPath = "Assets/Hotfix/Resources/Data/Binary";

        /// <summary>
        /// json�ļ�·��
        /// </summary>
        private const string JsonPath = "Assets/Hotfix/Resources/Data/Json";

        private EDataReadType m_DataReadType = EDataReadType.Binary;

        /// <summary>
        /// �������ݱ�
        /// </summary>
        private Dictionary<System.Type, IDataTable> m_AllDataTable;

        /// <summary>
        /// ��ȡ���ݱ�����
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
        /// ��ȡ�������ݱ�����
        /// </summary>
        /// <param name="id"></param>
        public void GetDataRow(int id)
        {

        }

        /// <summary>
        /// ��ȡ�������ݱ�����
        /// </summary>
        public void GetAllDataRow()
        {

        }

        /// <summary>
        /// ���ݱ��ȡ����
        /// </summary>
        private enum EDataReadType : byte
        {
            Binary,

            Json,
        }

    }
}
