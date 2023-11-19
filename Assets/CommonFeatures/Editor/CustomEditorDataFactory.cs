using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// �Զ���༭������
    /// </summary>
    public class CustomEditorDataFactory
    {
        /// <summary>
        /// ��ȡ����
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
            //���ļ� �Ͷ��ļ�
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
        /// д������
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
    /// �Զ���༭���������ݻ���
    /// </summary>
    public class CustomEditorData
    {

    }
}
