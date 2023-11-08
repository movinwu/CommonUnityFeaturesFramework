using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OOPS
{
    public class ExcelWindowData : CustomEditorData 
    {
        /// <summary>
        /// excel文件夹路径
        /// </summary>
        public string excelPath;

        /// <summary>
        /// 生成的json文件路径
        /// </summary>
        public string jsonPath;

        /// <summary>
        /// 生成的byte文件路径
        /// </summary>
        public string bytePath;
    }

    public class ExcelWindow : EditorWindow
    {
        private const string DataSavePath = "Excel";

        private ExcelWindowData m_Data;

        [MenuItem("Tools/打开导表窗口")]
        private static void OpenWindow()
        {
            var window = EditorWindow.GetWindow<ExcelWindow>(); 
            window.minSize = new Vector2(700, 400);
        }

        private void OnGUI()
        {
            if (null == m_Data)
            {
                CustomEditorDataFactory.ReadData(DataSavePath, out m_Data);
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Excel路径:");
            if (GUILayout.Button(m_Data.excelPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("请选择excel文件路径", m_Data.excelPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.excelPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成json文件路径:");
            if (GUILayout.Button(m_Data.jsonPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("请选择生成json文件保存路径", m_Data.jsonPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.jsonPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成byte文件路径:");
            if (GUILayout.Button(m_Data.bytePath))
            {
                var newPath = EditorUtility.SaveFolderPanel("请选择生成byte文件保存路径", m_Data.bytePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.bytePath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //生成按钮
            if (GUILayout.Button("生成"))
            {
                if (string.IsNullOrEmpty(m_Data.excelPath))
                {
                    Debug.LogError("请设置excel文件路径");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("请设置生成json文件路径");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("请设置生成byte文件路径");
                    return;
                }

                Generate();
            }

            EditorGUILayout.EndVertical();
        }

        private void Generate()
        {

        }
    }
}
