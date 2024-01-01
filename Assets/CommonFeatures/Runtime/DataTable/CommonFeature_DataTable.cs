using CommonFeatures.Config;
using CommonFeatures.Log;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommonFeatures.DataTable
{
    /// <summary>
    /// ͨ�ù���-���ݱ�
    /// </summary>
    public class CommonFeature_DataTable : CommonFeature
    {
        /// <summary>
        /// �������ݱ�
        /// </summary>
        private Dictionary<System.Type, IDataTable> m_AllDataTable = new Dictionary<Type, IDataTable>();

        /// <summary>
        /// ��ȡ���ݱ�����
        /// </summary>
        public void ReadDataTable()
        {
            var assemblyName = CommonConfig.GetStringConfig("DataTable", "assembly_name");
            var dataReadType = (EDataReadType)CommonConfig.GetLongConfig("DataTable", "data_read_type");

            m_AllDataTable.Clear();
            if (dataReadType == EDataReadType.Binary)
            {
                var binaryPath = CommonConfig.GetStringConfig("DataTable", "binary_path");
                var directory = new DirectoryInfo(binaryPath);
                var files = directory.GetFiles();
                for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                {
                    var file = files[fileIndex];
                    if (file.Name.EndsWith(".byte"))
                    {
                        try
                        {
                            string name = file.Name.Substring(0, file.Name.LastIndexOf('.'));
                            var typeName = $"HotfixScripts.DT_{name}";
                            var assembly = System.Reflection.Assembly.Load(assemblyName);
                            var type = assembly.GetType(typeName);
                            IDataTable table = assembly.CreateInstance(typeName) as IDataTable;
                            using (var stream = new FileStream(file.FullName, FileMode.Open))
                            {
                                using (var br = new BinaryReader(stream, System.Text.Encoding.UTF8))
                                {
                                    table.FromBinary(br);
                                }
                            }
                            var dataRowTypeName = $"HotfixScripts.DR_{name}";
                            var dataRowType = assembly.GetType(dataRowTypeName);
                            m_AllDataTable.Add(dataRowType, table);
                        }
                        catch (System.Exception ex)
                        {
                            CommonLog.ConfigException(ex);
                        }
                    }
                }
            }
#pragma warning disable CS0162 // ��⵽�޷����ʵĴ���
            else if (dataReadType == EDataReadType.Json)
#pragma warning restore CS0162 // ��⵽�޷����ʵĴ���
            {
                var jsonPath = CommonConfig.GetStringConfig("DataTable", "json_path");
                var directory = new DirectoryInfo(jsonPath);
                var files = directory.GetFiles();
                for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                {
                    var file = files[fileIndex];
                    if (file.Name.EndsWith(".json"))
                    {
                        try
                        {
                            string name = file.Name.Substring(0, file.Name.LastIndexOf('.'));
                            string typeName = $"OOPS.DataTable<DT_{name}>";
                            var assembly = System.Reflection.Assembly.Load(assemblyName);
                            var type = assembly.GetType(typeName);
                            IDataTable table = assembly.CreateInstance(typeName) as IDataTable;
                            using (var stream = new FileStream(file.FullName, FileMode.Open))
                            {
                                using (var br = new StreamReader(stream, System.Text.Encoding.UTF8))
                                {
                                    table.FromJson(br.ReadToEnd());
                                }
                            }
                            m_AllDataTable.Add(type, table);
                        }
                        catch (System.Exception ex)
                        {
                           CommonLog.ConfigException(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ��ȡ�������ݱ�����
        /// </summary>
        /// <param name="id"></param>
        public T GetDataRow<T>(int id) where T : DataRow
        {
            var type = typeof(T);
            if (m_AllDataTable.TryGetValue(type, out var table))
            {
                return table.GetDataRow<T>(id);
            }

            return default(T);
        }

        /// <summary>
        /// ��ȡ�������������ݱ�
        /// </summary>
        public List<T> GetDataRows<T>(Predicate<T> predicate = null, Comparer<T> comparer = null) where T : DataRow
        {
            var type = typeof(T);
            if (m_AllDataTable.TryGetValue(type, out var table))
            {
                return table.GetDataRows(predicate, comparer);
            }

            return new List<T>(0);
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
