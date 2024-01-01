using CommonFeatures.Config;
using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    public class ResourceManifestWindowData : CustomEditorData
    {
        /// <summary>
        /// 清单文件夹列表
        /// </summary>
        public List<string> ManifestDirectoryList;

        /// <summary>
        /// 忽略文件夹列表
        /// </summary>
        public List<string> IgnoreDirectoryList;

        /// <summary>
        /// 进入清单文件的正则匹配
        /// </summary>
        public List<string> GenerateRegexList;

        public bool ManifestFold = true;
        public bool IgnoreFold = true;
        public bool GenerateFold = true;
    }

    /// <summary>
    /// 资源清单窗口
    /// </summary>
    public class ResourceMainfestWindow : EditorWindow
    {
        [MenuItem("Tools/资源清单文件生成窗口", priority = CMenuItemPriority.ResourceManifestWindow)]
        static void OpenWindow()
        {
            var window = GetWindow<ResourceMainfestWindow>("资源清单生成窗口", true);
            window.minSize = new Vector2(700, 300);
        }

        [MenuItem("Tools/资源清单文件一键生成", priority = CMenuItemPriority.ResourceManifestOneKey)]
        private static void ResourceManifestOneKey()
        {
            CustomEditorDataFactory.ReadData<ResourceManifestWindowData>(DataSaveName, out var data);

            GenerateManifest(data);
        }

        private const string DataSaveName = "ResourceManifest";

        private ResourceManifestWindowData m_Data;

        private void OnGUI()
        {
            if (null == m_Data)
            {
                CustomEditorDataFactory.ReadData(DataSaveName, out m_Data);
                if (null == m_Data.ManifestDirectoryList)
                {
                    m_Data.ManifestDirectoryList = new List<string>();
                }
                if (null == m_Data.IgnoreDirectoryList)
                {
                    m_Data.IgnoreDirectoryList = new List<string>();
                }
                if (null == m_Data.GenerateRegexList)
                {
                    m_Data.GenerateRegexList = new List<string>();
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            bool manifestFoldClicked = GUILayout.Button(m_Data.ManifestFold ? "已折叠" : "已展开");
            if (manifestFoldClicked)
            {
                m_Data.ManifestFold = !m_Data.ManifestFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有清单生成文件夹");

            EditorGUILayout.LabelField(m_Data.ManifestDirectoryList.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.ManifestDirectoryList.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.ManifestDirectoryList.Count > 0)
                {
                    m_Data.ManifestDirectoryList.RemoveAt(m_Data.ManifestDirectoryList.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.ManifestFold)
            {
                for (int i = 0; i < m_Data.ManifestDirectoryList.Count; i++)
                {
                    int index = i;
                    if (GUILayout.Button(m_Data.ManifestDirectoryList[index]))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("选择清单路径", m_Data.ManifestDirectoryList[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.ManifestDirectoryList[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            bool ignoreFoldClicked = GUILayout.Button(m_Data.IgnoreFold ? "已折叠" : "已展开");
            if (ignoreFoldClicked)
            {
                m_Data.IgnoreFold = !m_Data.IgnoreFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有忽略文件夹");

            EditorGUILayout.LabelField(m_Data.IgnoreDirectoryList.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.IgnoreDirectoryList.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.IgnoreDirectoryList.Count > 0)
                {
                    m_Data.IgnoreDirectoryList.RemoveAt(m_Data.IgnoreDirectoryList.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.IgnoreFold)
            {
                for (int i = 0; i < m_Data.IgnoreDirectoryList.Count; i++)
                {
                    int index = i;
                    if (GUILayout.Button(m_Data.IgnoreDirectoryList[index]))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("选择忽略路径", m_Data.IgnoreDirectoryList[index], "");
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            m_Data.IgnoreDirectoryList[index] = newPath;
                            CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            bool generateFoldClicked = GUILayout.Button(m_Data.GenerateFold ? "已折叠" : "已展开");
            if (generateFoldClicked)
            {
                m_Data.GenerateFold = !m_Data.GenerateFold;
                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
            }

            GUILayout.Label("所有清单后缀");

            EditorGUILayout.LabelField(m_Data.GenerateRegexList.Count.ToString());

            if (GUILayout.Button("+"))
            {
                m_Data.GenerateRegexList.Add(string.Empty);
            }

            if (GUILayout.Button("-"))
            {
                if (m_Data.GenerateRegexList.Count > 0)
                {
                    m_Data.GenerateRegexList.RemoveAt(m_Data.GenerateRegexList.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!m_Data.GenerateFold)
            {
                int rowCount = 3;
                for (int i = 0; i < m_Data.GenerateRegexList.Count / rowCount + 1; i++)
                {
                    int row = i;
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < rowCount; j++)
                    {
                        int col = j;
                        if (row * rowCount + col < m_Data.GenerateRegexList.Count)
                        {
                            var suffix = m_Data.GenerateRegexList[row * rowCount + col];
                            var str = GUILayout.TextField(suffix);
                            if (!str.Equals(suffix))
                            {
                                m_Data.GenerateRegexList[row * rowCount + col] = str;
                                CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("生成清单文件"))
            {
                GenerateManifest(m_Data);
            }

            EditorGUILayout.EndVertical();
        }

        private static void GenerateManifest(ResourceManifestWindowData windowData)
        {
            //检查文件路径
            if (windowData.ManifestDirectoryList.Count == 0 || windowData.GenerateRegexList.Count == 0)
            {
                CommonLog.ResourceError("资源清单文件生成失败, 没有指定清单生成文件夹或者没有指定清单生成后缀");
                return;
            }

            Dictionary<string, string> manifest = new Dictionary<string, string>();
            HashSet<string> ignoreFolderSet = new HashSet<string>();
            for (int i = 0; i < windowData.IgnoreDirectoryList.Count; i++)
            {
                ignoreFolderSet.Add(windowData.IgnoreDirectoryList[i]);
            }

            for (int i = 0; i < windowData.ManifestDirectoryList.Count; i++)
            {
                var path = windowData.ManifestDirectoryList[i];
                var directoryInfo = new DirectoryInfo(path);
                AddFileIntoManifest(manifest, directoryInfo, ignoreFolderSet);
            }

            //读取清单文件地址
            var filePath = Path.Combine(Application.dataPath, CommonConfig.GetStringConfig("Resource", "manifest_path"));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(manifest.Count.ToString());
            char spliteChar = CommonConfig.GetStringConfig("Resource", "manifest_split_symbol")[0];
            foreach (var file in manifest)
            {
                if (file.Value.Contains(spliteChar))
                {
                    CommonLog.ResourceWarning($"{file.Value}\n文件路径或文件名中包含字符{spliteChar}, 无法添加到清单文件中");
                    continue;
                }
                sb.AppendLine($"{file.Key}{spliteChar}{file.Value}");
            }
            File.WriteAllText(filePath, sb.ToString());
            CommonLog.Resource("生成清单文件成功");
            AssetDatabase.Refresh();

            //将文件添加到清单中
            void AddFileIntoManifest(Dictionary<string, string> manifest, DirectoryInfo directory, HashSet<string> ignoreSet)
            {
                var files = directory.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    string full = files[i].FullName;
                    full = full.Replace('\\', '/');
                    var name = full.Substring(full.LastIndexOf('/'));
                    for (int j = 0; j < windowData.GenerateRegexList.Count; j++)
                    {
                        if (Regex.IsMatch(name, windowData.GenerateRegexList[j]))
                        {
                            if (manifest.ContainsKey(name))
                            {
                                CommonLog.ResourceError($"资源 {name} 被重复添加到清单文件中, 检查资源是否重名.\n {manifest[name]}\n{full}");
                                continue;
                            }
                            var path = full.Substring(full.IndexOf("Assets/") + 7);
                            manifest.Add(name, path);
                            break;
                        }
                    }
                }

                var directories = directory.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    if (!ignoreSet.Contains(directories[i].FullName.Replace('\\', '/')))
                    {
                        //递归
                        AddFileIntoManifest(manifest, directories[i], ignoreSet);
                    }
                }
            }
        }
    }
}
