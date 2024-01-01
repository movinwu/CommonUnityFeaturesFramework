using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    public class AssetBundlePackerWindowData : CustomEditorData
    {
        /// <summary>
        /// ��̬��Դ(���ᶯ̬���ص���Դ)·����ַ
        /// </summary>
        public List<string> StaticResourcePath;

        /// <summary>
        /// ��̬��Դ�����ļ���
        /// </summary>
        public List<string> StaticResourceIgnorePath;

        /// <summary>
        /// ��̬��Դ·����ַ
        /// </summary>
        public List<string> StreamingResourcePath;

        /// <summary>
        /// ��̬��Դ�����ļ���
        /// </summary>
        public List<string> StreamingResourceIgnorePath;

        /// <summary>
        /// �������Դɸѡ����
        /// </summary>
        public List<string> PackResourceFilter;

        public bool StaticPathFold = true;
        public bool StaticIgnorePathFold = true;
        public bool StreamingPathFold = true;
        public bool StreamingIgnorePathFold = true;
        public bool PackFilterFold = true;
    }

    /// <summary>
    /// AB���������
    /// </summary>
    public class AssetBundlePackerWindow : EditorWindow
    {
        [MenuItem("Tools/AB���������", priority = CMenuItemPriority.AssetBundlePackerWindow)]
        static void OpenWindow()
        {
            var window = GetWindow<AssetBundlePackerWindow>("AB���������", true);
            window.minSize = new Vector2(700, 300);
        }

        [MenuItem("Tools/AB��һ�����", priority = CMenuItemPriority.AssetBundlePackerOneKey)]
        private static void ResourceManifestOneKey()
        {
            CustomEditorDataFactory.ReadData<AssetBundlePackerWindowData>(DataSaveName, out var data);

            PackAssetBundle(data);
        }

        private const string DataSaveName = "AssetBundlePacker";

        private AssetBundlePackerWindowData m_Data;

        private void OnGUI()
        {
            if (null == m_Data)
            {
                CustomEditorDataFactory.ReadData(DataSaveName, out m_Data);
                if (null == m_Data.StaticResourcePath)
                {
                    m_Data.StaticResourcePath = new List<string>();
                }
                if (null == m_Data.StreamingResourcePath)
                {
                    m_Data.StreamingResourcePath = new List<string>();
                }
                if (null == m_Data.PackResourceFilter)
                {
                    m_Data.PackResourceFilter = new List<string>();
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            #region ��̬��Դ
            EditorGUILayout.BeginHorizontal();

            bool staticPathFoldClicked = GUILayout.Button(m_Data.StaticPathFold ? "���۵�" : "��չ��");
            if (staticPathFoldClicked)
            {
                m_Data.StaticPathFold = !m_Data.StaticPathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("���о�̬��Դ�ļ���(�Զ�����������ϵ,�Զ��ְ�)");

            EditorGUILayout.LabelField(m_Data.StaticResourcePath.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.StaticResourcePath.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.StaticResourcePath.Count > 0)
                {
                    m_Data.StaticResourcePath.RemoveAt(m_Data.StaticResourcePath.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.StaticPathFold)
            {
                for (int i = 0; i < m_Data.StaticResourcePath.Count; i++)
                {
                    int index = i;
                    if (GUILayout.Button(m_Data.StaticResourcePath[index]))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("ѡ��̬��Դ·��", m_Data.StaticResourcePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StaticResourcePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion ��̬��Դ

            EditorGUILayout.Space();

            #region ��̬��Դ����
            EditorGUILayout.BeginHorizontal();

            bool staticIgnorePathFoldClicked = GUILayout.Button(m_Data.StaticIgnorePathFold ? "���۵�" : "��չ��");
            if (staticIgnorePathFoldClicked)
            {
                m_Data.StaticIgnorePathFold = !m_Data.StaticIgnorePathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("���о�̬��Դ�����ļ���(�ļ�������Դ���ᱻ���)");

            EditorGUILayout.LabelField(m_Data.StaticResourceIgnorePath.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.StaticResourceIgnorePath.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.StaticResourceIgnorePath.Count > 0)
                {
                    m_Data.StaticResourceIgnorePath.RemoveAt(m_Data.StaticResourceIgnorePath.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.StaticIgnorePathFold)
            {
                for (int i = 0; i < m_Data.StaticResourceIgnorePath.Count; i++)
                {
                    int index = i;
                    if (GUILayout.Button(m_Data.StaticResourceIgnorePath[index]))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("ѡ��̬��Դ·��", m_Data.StaticResourceIgnorePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StaticResourceIgnorePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion ��̬��Դ����

            EditorGUILayout.Space();

            #region ��̬��Դ
            EditorGUILayout.BeginHorizontal();

            bool streamingFoldClicked = GUILayout.Button(m_Data.StreamingPathFold ? "���۵�" : "��չ��");
            if (streamingFoldClicked)
            {
                m_Data.StreamingPathFold = !m_Data.StreamingPathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("���ж�̬��Դ�ļ���(һ���ļ�һ����,�ֶ��ְ�)");

            EditorGUILayout.LabelField(m_Data.StreamingResourcePath.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.StreamingResourcePath.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.StreamingResourcePath.Count > 0)
                {
                    m_Data.StreamingResourcePath.RemoveAt(m_Data.StreamingResourcePath.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.StreamingPathFold)
            {
                for (int i = 0; i < m_Data.StreamingResourcePath.Count; i++)
                {
                    int index = i;
                    if (GUILayout.Button(m_Data.StreamingResourcePath[index]))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("ѡ��̬·��", m_Data.StreamingResourcePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StreamingResourcePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion ��̬��Դ

            EditorGUILayout.Space();

            #region ��̬��Դ����
            EditorGUILayout.BeginHorizontal();

            bool streamingIgnoreFoldClicked = GUILayout.Button(m_Data.StreamingIgnorePathFold ? "���۵�" : "��չ��");
            if (streamingIgnoreFoldClicked)
            {
                m_Data.StreamingIgnorePathFold = !m_Data.StreamingIgnorePathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("���ж�̬��Դ�����ļ���(�ļ�������Դ������)");

            EditorGUILayout.LabelField(m_Data.StreamingResourceIgnorePath.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.StreamingResourceIgnorePath.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.StreamingResourceIgnorePath.Count > 0)
                {
                    m_Data.StreamingResourceIgnorePath.RemoveAt(m_Data.StreamingResourceIgnorePath.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.StreamingIgnorePathFold)
            {
                for (int i = 0; i < m_Data.StreamingResourceIgnorePath.Count; i++)
                {
                    int index = i;
                    if (GUILayout.Button(m_Data.StreamingResourceIgnorePath[index]))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("ѡ��̬·��", m_Data.StreamingResourceIgnorePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StreamingResourceIgnorePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion ��̬��Դ����

            EditorGUILayout.Space();

            #region �ļ�ɸѡ����
            EditorGUILayout.BeginHorizontal();

            bool packFilterFoldClicked = GUILayout.Button(m_Data.PackFilterFold ? "���۵�" : "��չ��");
            if (packFilterFoldClicked)
            {
                m_Data.PackFilterFold = !m_Data.PackFilterFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("������Ҫ�����Դɸѡ����");

            EditorGUILayout.LabelField(m_Data.PackResourceFilter.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.PackResourceFilter.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.PackResourceFilter.Count > 0)
                {
                    m_Data.PackResourceFilter.RemoveAt(m_Data.PackResourceFilter.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.PackFilterFold)
            {
                int rowCount = 3;
                for (int i = 0; i < m_Data.PackResourceFilter.Count / rowCount + 1; i++)
                {
                    int row = i;
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < rowCount; j++)
                    {
                        int col = j;
                        if (row * rowCount + col < m_Data.PackResourceFilter.Count)
                        {
                            var suffix = m_Data.PackResourceFilter[row * rowCount + col];
                            var str = GUILayout.TextField(suffix);
                            if (!str.Equals(suffix))
                            {
                                m_Data.PackResourceFilter[row * rowCount + col] = str;
                                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            #endregion �ļ�ɸѡ����

            if (GUILayout.Button("��ʼ���AB��"))
            {
                PackAssetBundle(m_Data);
            }

            EditorGUILayout.EndVertical();
        }

        private static void PackAssetBundle(AssetBundlePackerWindowData windowData)
        {
            if ((null == windowData.StaticResourcePath
                || windowData.StaticResourcePath.Count == 0)
                && (null == windowData.StreamingResourcePath
                || windowData.StreamingResourcePath.Count == 0))
            {
                CommonLog.LogError("�����ö�̬��̬��Դ·��");
                return;
            }
            if (null == windowData.PackResourceFilter
                || windowData.PackResourceFilter.Count == 0)
            {
                CommonLog.LogError("������Ҫ�������Դɸѡ����");
                return;
            }


        }
    }
}
