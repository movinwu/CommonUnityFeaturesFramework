using CommonFeatures.Log;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// 打包类名称
        /// </summary>
        public string PackerClassName;

        /// <summary>
        /// 热更新包输出文件夹
        /// </summary>
        public string OutputPath;

        /// <summary>
        /// 是否自动将打包好的文件拷贝到streamingAssets下
        /// </summary>
        public bool CopyToStreamingAssets;

        /// <summary>
        /// 是否所有内容强制重新构建AB包
        /// </summary>
        public bool ForceRebuildAll;

        /// <summary>
        /// AB包版本号
        /// </summary>
        public int ABVersion;

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

        private string[] m_AllPackerClass;

        private int m_PackerClassIndex = -1;

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

            if (null == m_AllPackerClass)
            {
                var parentType = typeof(AssetBundleBuildHandlerBase);
                m_AllPackerClass = Assembly.GetAssembly(parentType)
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(parentType))
                    .Select(t => t.ToString())
                    .ToArray();

                //找到上次选中的类
                for (int i = 0; i < m_AllPackerClass.Length; i++)
                {
                    if (m_AllPackerClass[i].Equals(m_Data.PackerClassName))
                    {
                        m_PackerClassIndex = i;
                        break;
                    }
                }
                m_PackerClassIndex = Mathf.Max(m_PackerClassIndex, 0);
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

            EditorGUILayout.Space();

            #region 打包实现类
            var packerIndex = EditorGUILayout.Popup("选择打包操作类", m_PackerClassIndex, m_AllPackerClass);
            if (packerIndex != m_PackerClassIndex)
            {
                m_PackerClassIndex = packerIndex;
                m_Data.PackerClassName = m_AllPackerClass[m_PackerClassIndex];
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }
            #endregion 打包实现类

            EditorGUILayout.Space();

            #region AB包输出地址
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("AB包输出地址");

            if (GUILayout.Button(m_Data.OutputPath))
            {
                var newPath = EditorUtility.OpenFolderPanel("选择输出地址", m_Data.OutputPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.OutputPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                }
            }

            EditorGUILayout.EndHorizontal();
            #endregion AB包输出地址

            EditorGUILayout.Space();

            #region 是否自动拷贝\是否强制重新构建\版本号
            EditorGUILayout.BeginHorizontal();

            var isCopy = GUILayout.Toggle(m_Data.CopyToStreamingAssets, "打包完成后自动拷贝到StreamingAssets下");
            if (isCopy != m_Data.CopyToStreamingAssets)
            {
                m_Data.CopyToStreamingAssets = isCopy;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            var isForceRebuild = GUILayout.Toggle(m_Data.ForceRebuildAll, "是否重新构建所有资源");
            if (isForceRebuild != m_Data.ForceRebuildAll)
            {
                m_Data.ForceRebuildAll = isForceRebuild;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            var version = EditorGUILayout.TextField("版本号", m_Data.ABVersion.ToString());
            if (int.TryParse(version, out var v))
            {
                m_Data.ABVersion = v;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            EditorGUILayout.EndHorizontal();
            #endregion 是否自动拷贝\是否强制重新构建\版本号

            EditorGUILayout.Space();

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
            if (string.IsNullOrEmpty(windowData.PackerClassName))
            {
                CommonLog.LogError("打包类不正确");
                return;
            }

            var packerType = Assembly.GetAssembly(typeof(AssetBundleBuildHandlerBase)).GetType(windowData.PackerClassName);
            var packer = packerType.Assembly.CreateInstance(packerType.ToString()) as AssetBundleBuildHandlerBase;
            packer.PackAssetBundle();

            ++windowData.ABVersion;
            CustomEditorDataFactory.WriteData(DataSaveName, windowData);
        }
    }
}
