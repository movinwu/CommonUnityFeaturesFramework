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
    /// ���ù���
    /// </summary>
    public class ConfigManager : SingletonBase<ConfigManager>
    {
        private ConfigManager() { ReadToml(); }

        /// <summary>
        /// �����ļ���ַ
        /// </summary>
        private const string ConfigPath = "CommonFeatures/Config/Toml/Config.toml";

        /// <summary>
        /// ���ñ��
        /// </summary>
        private TomlTable m_Table;

        /// <summary>
        /// ��ȡ�ڵ�
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public TomlNode GetNode(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public string GetStrConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public long GetLongConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public double GetDoubleConfig(params string[] tableNames)
        {
            var table = m_Table;
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
            }
            return 0d;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTime GetDateTimeConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTimeOffset GetDateTimeOffsetConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public bool GetBooleanConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public long[] GetLongArrayConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public string[] GetStringArrayConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public double[] GetDoubleArrayConfig(params string[] tableNames)
        {
            var table = m_Table;
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
                    }
                    return value;
                }
            }
            return new double[0];
        }

        /// <summary>
        /// ��ȡ������������
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTime[] GetDateTimeArrayConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTimeOffset[] GetDateTimeOffsetArrayConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public bool[] GetBooleanArrayConfig(params string[] tableNames)
        {
            var table = m_Table;
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
        private void ReadToml()
        {
            if (null == m_Table)
            {
                using (StreamReader reader = File.OpenText(Path.Combine(Application.dataPath, ConfigPath)))
                {
                    m_Table = TOML.Parse(reader);
                }
            }
        }
    }
}
