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
        /// 静态资源(不会动态加载的资源)路径地址
        /// </summary>
        public List<string> StaticResourcePath;

        /// <summary>
        /// 静态资源忽略文件夹
        /// </summary>
        public List<string> StaticResourceIgnorePath;

        /// <summary>
        /// 动态资源路径地址
        /// </summary>
        public List<string> StreamingResourcePath;

        /// <summary>
        /// 动态资源忽略文件夹
        /// </summary>
        public List<string> StreamingResourceIgnorePath;

        /// <summary>
        /// 打包的资源筛选正则
        /// </summary>
        public List<string> PackResourceFilter;

        public bool StaticPathFold = true;
        public bool StaticIgnorePathFold = true;
        public bool StreamingPathFold = true;
        public bool StreamingIgnorePathFold = true;
        public bool PackFilterFold = true;
    }

    /// <summary>
    /// AB包打包窗口
    /// </summary>
    public class AssetBundlePackerWindow : EditorWindow
    {
        [MenuItem("Tools/AB包打包窗口", priority = CMenuItemPriority.AssetBundlePackerWindow)]
        static void OpenWindow()
        {
            var window = GetWindow<AssetBundlePackerWindow>("AB包打包窗口", true);
            window.minSize = new Vector2(700, 300);
        }

        [MenuItem("Tools/AB包一键打包", priority = CMenuItemPriority.AssetBundlePackerOneKey)]
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

            #region 静态资源
            EditorGUILayout.BeginHorizontal();

            bool staticPathFoldClicked = GUILayout.Button(m_Data.StaticPathFold ? "已折叠" : "已展开");
            if (staticPathFoldClicked)
            {
                m_Data.StaticPathFold = !m_Data.StaticPathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有静态资源文件夹(自动分析依赖关系,自动分包)");

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
                        var newPath = EditorUtility.OpenFolderPanel("选择静态资源路径", m_Data.StaticResourcePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StaticResourcePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion 静态资源

            EditorGUILayout.Space();

            #region 静态资源忽略
            EditorGUILayout.BeginHorizontal();

            bool staticIgnorePathFoldClicked = GUILayout.Button(m_Data.StaticIgnorePathFold ? "已折叠" : "已展开");
            if (staticIgnorePathFoldClicked)
            {
                m_Data.StaticIgnorePathFold = !m_Data.StaticIgnorePathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有静态资源忽略文件夹(文件夹内资源不会被打包)");

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
                        var newPath = EditorUtility.OpenFolderPanel("选择静态资源路径", m_Data.StaticResourceIgnorePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StaticResourceIgnorePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion 静态资源忽略

            EditorGUILayout.Space();

            #region 动态资源
            EditorGUILayout.BeginHorizontal();

            bool streamingFoldClicked = GUILayout.Button(m_Data.StreamingPathFold ? "已折叠" : "已展开");
            if (streamingFoldClicked)
            {
                m_Data.StreamingPathFold = !m_Data.StreamingPathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有动态资源文件夹(一个文件一个包,手动分包)");

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
                        var newPath = EditorUtility.OpenFolderPanel("选择动态路径", m_Data.StreamingResourcePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StreamingResourcePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion 动态资源

            EditorGUILayout.Space();

            #region 动态资源忽略
            EditorGUILayout.BeginHorizontal();

            bool streamingIgnoreFoldClicked = GUILayout.Button(m_Data.StreamingIgnorePathFold ? "已折叠" : "已展开");
            if (streamingIgnoreFoldClicked)
            {
                m_Data.StreamingIgnorePathFold = !m_Data.StreamingIgnorePathFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有动态资源忽略文件夹(文件夹内资源不会打包)");

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
                        var newPath = EditorUtility.OpenFolderPanel("选择动态路径", m_Data.StreamingResourceIgnorePath[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.StreamingResourceIgnorePath[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }
            #endregion 动态资源忽略

            EditorGUILayout.Space();

            #region 文件筛选正则
            EditorGUILayout.BeginHorizontal();

            bool packFilterFoldClicked = GUILayout.Button(m_Data.PackFilterFold ? "已折叠" : "已展开");
            if (packFilterFoldClicked)
            {
                m_Data.PackFilterFold = !m_Data.PackFilterFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有需要打包资源筛选正则");

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
            #endregion 文件筛选正则

            if (GUILayout.Button("开始打包AB包"))
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
                CommonLog.LogError("请设置动态或静态资源路径");
                return;
            }
            if (null == windowData.PackResourceFilter
                || windowData.PackResourceFilter.Count == 0)
            {
                CommonLog.LogError("请设置要打包的资源筛选正则");
                return;
            }


        }
    }
}
