using CommonFeatures.Log;
using CommonFeatures.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tommy;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// ͨ�ù���-����
    /// </summary>
    public class CommonConfig
    {
        /// <summary>
        /// �����ļ���ַ
        /// </summary>
        private const string ConfigPath = "CommonFeatures/Runtime/Config/Toml";

        /// <summary>
        /// ���ñ���
        /// </summary>
        private static Dictionary<string, TomlTable> m_Table = new Dictionary<string, TomlTable>();

        /// <summary>
        /// �����Ƿ��ȡ��ʾ
        /// </summary>
        private static bool m_ReadSymbol = false;

        /// <summary>
        /// ��ȡ�ڵ�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static TomlNode GetNode(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (i == 0)
                {
                    return null;
                }
            }
            return table;
        }

        /// <summary>
        /// ��ȡ�ַ�������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static string GetStringConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsString)
                {
                    return node.AsString.Value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static long GetLongConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return 0L;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsInteger)
                {
                    return node.AsInteger.Value;
                }
            }
            return 0L;
        }

        /// <summary>
        /// ��ȡ����������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static double GetDoubleConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return 0d;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsFloat)
                {
                    return node.AsFloat.Value;
                }
                else if (node.IsInteger)
                {
                    return node.AsInteger.Value;
                }
            }
            return 0d;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return default(DateTime);
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsDateTimeLocal)
                {
                    return node.AsDateTimeLocal.Value;
                }
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static DateTimeOffset GetDateTimeOffsetConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return default(DateTimeOffset);
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsDateTimeOffset)
                {
                    return node.AsDateTimeOffset.Value;
                }
            }
            return DateTimeOffset.MinValue;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static bool GetBooleanConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return false;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsBoolean)
                {
                    return node.AsBoolean.Value;
                }
            }
            return false;
        }

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static long[] GetLongArrayConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsArray)
                {
                    var array = node.AsArray;
                    if (array.ChildrenCount == 0)
                    {
                        return new long[0];
                    }
                    long[] value = new long[array.ChildrenCount];
                    for (int j = 0; j < array.RawArray.Count; j++)
                    {
                        if (array.RawArray[j].IsInteger)
                        {
                            value[j] = array.RawArray[j].AsInteger.Value;
                        }
                    }
                    return value;
                }
            }
            return new long[0];
        }

        /// <summary>
        /// ��ȡ�ַ�����������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static string[] GetStringArrayConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsArray)
                {
                    var array = node.AsArray;
                    if (array.ChildrenCount == 0)
                    {
                        return new string[0];
                    }
                    string[] value = new string[array.ChildrenCount];
                    for (int j = 0; j < array.RawArray.Count; j++)
                    {
                        if (array.RawArray[j].IsString)
                        {
                            value[j] = array.RawArray[j].AsString.Value;
                        }
                    }
                    return value;
                }
            }
            return new string[0];
        }

        /// <summary>
        /// ��ȡ��������������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static double[] GetDoubleArrayConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsArray)
                {
                    var array = node.AsArray;
                    if (array.ChildrenCount == 0)
                    {
                        return new double[0];
                    }
                    double[] value = new double[array.ChildrenCount];
                    for (int j = 0; j < array.RawArray.Count; j++)
                    {
                        if (array.RawArray[j].IsFloat)
                        {
                            value[j] = array.RawArray[j].AsFloat.Value;
                        }
                        else if (array.RawArray[j].IsInteger)
                        {
                            value[j] = array.RawArray[j].AsInteger.Value;
                        }
                    }
                    return value;
                }
            }
            return new double[0];
        }

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static DateTime[] GetDateTimeArrayConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsArray)
                {
                    var array = node.AsArray;
                    if (array.ChildrenCount == 0)
                    {
                        return new DateTime[0];
                    }
                    DateTime[] value = new DateTime[array.ChildrenCount];
                    for (int j = 0; j < array.RawArray.Count; j++)
                    {
                        if (array.RawArray[j].IsDateTime)
                        {
                            value[j] = array.RawArray[j].AsDateTimeLocal.Value;
                        }
                    }
                    return value;
                }
            }
            return new DateTime[0];
        }

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static DateTimeOffset[] GetDateTimeOffsetArrayConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsArray)
                {
                    var array = node.AsArray;
                    if (array.ChildrenCount == 0)
                    {
                        return new DateTimeOffset[0];
                    }
                    DateTimeOffset[] value = new DateTimeOffset[array.ChildrenCount];
                    for (int j = 0; j < array.RawArray.Count; j++)
                    {
                        if (array.RawArray[j].IsDateTimeOffset)
                        {
                            value[j] = array.RawArray[j].AsDateTimeOffset.Value;
                        }
                    }
                    return value;
                }
            }
            return new DateTimeOffset[0];
        }

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public static bool[] GetBooleanArrayConfig(string name, params string[] tableNames)
        {
            ReadData();
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"����������Ϊ{name}������");
                return null;
            }
            var table = m_Table[name];
            for (int i = 0; i < tableNames.Length; i++)
            {
                var node = table[tableNames[i]];
                if (node.IsTable)
                {
                    table = node.AsTable;
                }
                else if (node.IsArray)
                {
                    var array = node.AsArray;
                    if (array.ChildrenCount == 0)
                    {
                        return new bool[0];
                    }
                    bool[] value = new bool[array.ChildrenCount];
                    for (int j = 0; j < array.RawArray.Count; j++)
                    {
                        if (array.RawArray[j].IsBoolean)
                        {
                            value[j] = array.RawArray[j].AsBoolean.Value;
                        }
                    }
                    return value;
                }
            }
            return new bool[0];
        }

        /// <summary>
        /// ��ȡtoml�ļ�
        /// </summary>
        public static void ReadData()
        {
            if (m_ReadSymbol)
            {
                return;
            }
            m_Table.Clear();
            var directoryPath = Path.Combine(Application.dataPath, ConfigPath);
            if (!Directory.Exists(directoryPath))
            {
                return;
            }
            var files = Directory.GetFiles(directoryPath, "*.toml");
            foreach (var file in files)
            {
                using (StreamReader reader = File.OpenText(file))
                {
                    var table = TOML.Parse(reader);
                    var newFile = file.Replace('\\', '/');
                    var nameStartIndex = newFile.LastIndexOf('/') + 1;
                    var nameEndIndex = newFile.LastIndexOf('.');
                    var name = file.Substring(nameStartIndex, nameEndIndex - nameStartIndex);
                    m_Table.Add(name, table);
                }
            }
            m_ReadSymbol = true;
        }
    }
}