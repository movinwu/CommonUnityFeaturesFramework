using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OOPS
{
    public class ExcelWindowData : CustomEditorData 
    {
        /// <summary>
        /// excel�ļ���·��
        /// </summary>
        public string excelPath;

        /// <summary>
        /// ���ɵ�json�ļ�·��
        /// </summary>
        public string jsonPath;

        /// <summary>
        /// ���ɵ�byte�ļ�·��
        /// </summary>
        public string bytePath;
    }

    public class ExcelWindow : EditorWindow
    {
        private const string DataSavePath = "Excel";

        private ExcelWindowData m_Data;

        [MenuItem("Tools/�򿪵�����")]
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
            EditorGUILayout.LabelField("Excel·��:");
            if (GUILayout.Button(m_Data.excelPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("��ѡ��excel�ļ�·��", m_Data.excelPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.excelPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("����json�ļ�·��:");
            if (GUILayout.Button(m_Data.jsonPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("��ѡ������json�ļ�����·��", m_Data.jsonPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.jsonPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("����byte�ļ�·��:");
            if (GUILayout.Button(m_Data.bytePath))
            {
                var newPath = EditorUtility.SaveFolderPanel("��ѡ������byte�ļ�����·��", m_Data.bytePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.bytePath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //���ɰ�ť
            if (GUILayout.Button("����"))
            {
                if (string.IsNullOrEmpty(m_Data.excelPath))
                {
                    Debug.LogError("������excel�ļ�·��");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("����������json�ļ�·��");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("����������byte�ļ�·��");
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
