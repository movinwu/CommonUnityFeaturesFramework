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
    /// 配置管理
    /// </summary>
    public class CommonFeature_Config : CommonFeature
    {
        /// <summary>
        /// 配置文件地址
        /// </summary>
        private const string ConfigPath = "CommonFeatures/Runtime/Config/Toml";

        /// <summary>
        /// 配置表格
        /// </summary>
        private Dictionary<string, TomlTable> m_Table = new Dictionary<string, TomlTable>();

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public TomlNode GetNode(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取字符串配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public string GetStringConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取整型配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public long GetLongConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取浮点型配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public double GetDoubleConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取日期配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTime GetDateTimeConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取日期配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTimeOffset GetDateTimeOffsetConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取布尔配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public bool GetBooleanConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取整型数组配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public long[] GetLongArrayConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取字符串数组配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public string[] GetStringArrayConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取浮点型数组配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public double[] GetDoubleArrayConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取日期数组配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTime[] GetDateTimeArrayConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取日期数组配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public DateTimeOffset[] GetDateTimeOffsetArrayConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 获取布尔数组配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public bool[] GetBooleanArrayConfig(string name, params string[] tableNames)
        {
            if (!m_Table.ContainsKey(name))
            {
                CommonLog.ConfigError($"不存在名称为{name}的配置");
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
        /// 读取toml文件
        /// </summary>
        public override void Init()
        {
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
        }
    }
}
