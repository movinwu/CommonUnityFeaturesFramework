using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// 自定义编辑器配置
    /// </summary>
    public class CustomEditorDataFactory
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataName"></param>
        /// <param name="data"></param>
        public static void ReadData<T>(string dataName, out T data) where T : CustomEditorData,new()
        {
            string customPath = Application.dataPath.Replace("Assets", "CustomSetting");
            if (!Directory.Exists(customPath))
            {
                Directory.CreateDirectory(customPath);
            }
            var jsonPath = Path.Combine(customPath, dataName + ".json");

            var fileExist = File.Exists(jsonPath);
            //有文件 就读文件
            if (fileExist)
            {
                var json = File.ReadAllText(jsonPath);
                data = new T();
                EditorJsonUtility.FromJsonOverwrite(json, data);
            }
            else
            {
                data = new T();
            }
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataName"></param>
        /// <param name="data"></param>
        public static void WriteData<T>(string dataName, T data) where T : CustomEditorData, new()
        {
            if (null == data)
            {
                data = new T();
            }
            string customPath = Application.dataPath.Replace("Assets", "CustomSetting");
            if (!Directory.Exists(customPath))
            {
                Directory.CreateDirectory(customPath);
            }
            var jsonPath = Path.Combine(customPath, dataName + ".json");

            if (!File.Exists(jsonPath))
            {
                File.CreateText(jsonPath);
            }

            var jsoninfo = EditorJsonUtility.ToJson(data);
            File.WriteAllText(jsonPath, jsoninfo);
        }
    }

    /// <summary>
    /// 自定义编辑器配置数据基类
    /// </summary>
    public class CustomEditorData
    {

    }
}
