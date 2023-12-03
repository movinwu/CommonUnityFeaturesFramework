using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.NetWork
{
    public class Proto2CSharpWindowData : CustomEditorData
    {
        /// <summary>
        /// proto.exe 路径
        /// </summary>
        public string ProtoExePath;

        /// <summary>
        /// 创建的C#通信协议文件夹
        /// </summary>
        public string GenerateCSharpProtocolPath;
    }

    public class Proto2CSharpWindow : EditorWindow
    {
        [MenuItem("Tools/Proto代码生成窗口", priority = CMenuItemPriority.Proto2CSharpWindow)]
        static void OpenWindow()
        {
            var window = GetWindow<Proto2CSharpWindow>("Proto转C#工具", true);
            window.minSize = new Vector2(700, 300);
        }

        [MenuItem("Tools/Proto代码一键生成", priority = CMenuItemPriority.Proto2CSharpOneKey)]
        private static void Proto2CSharpOneKey()
        {
            CustomEditorDataFactory.ReadData<Proto2CSharpWindowData>(DataSaveName, out var data);
            if (null == data
                || string.IsNullOrEmpty(data.GenerateCSharpProtocolPath)
                || string.IsNullOrEmpty(data.ProtoExePath)
                || !Directory.Exists(data.GenerateCSharpProtocolPath)
                || !Directory.Exists(data.ProtoExePath))
            {
                Debug.LogError("路径不正确,请打开proto代码生成窗口设置正确的路径");
                return;
            }

            Generate(data);
        }

        private Proto2CSharpWindowData m_Data;

        private const string DataSaveName = "Proto2CSharp";

        private const string GenerateCSTemplete =
@"//------------------------------------------------------------
// oops-framework-cocos2unity
// Copyright © #YEAR# movinwu. All rights reserved.
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：#TIME#
//------------------------------------------------------------

using CommonFeatures.Log;
using CommonFeatures.NetWork;
using CommonFeatures.Pool;

namespace HotfixScripts
{
    public class #NAME#Protocol : Protocol<#NAME#>
    {
        public override short MsgId => throw new System.NotImplementedException();

        protected override void OnReceive()
        {
            throw new System.NotImplementedException();
        }
    }
}";

        private void OnGUI()
        {
            if (null == m_Data)
            {
                CustomEditorDataFactory.ReadData(DataSaveName, out m_Data);
            }

            EditorGUILayout.Space();

            //proto.exe
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("proto.exe文件及proto数据路径(放在同一个文件夹下):");
            if (GUILayout.Button(m_Data.ProtoExePath))
            {
                var newPath = EditorUtility.OpenFolderPanel("请选择proto.exe文件路径", m_Data.ProtoExePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.ProtoExePath = newPath;
                    CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //proto创建c#协议文件夹
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成的proto协议文件保存路径:");
            if (GUILayout.Button(m_Data.GenerateCSharpProtocolPath))
            {
                var newPath = EditorUtility.OpenFolderPanel("请选择proto协议文件保存路径", m_Data.GenerateCSharpProtocolPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.GenerateCSharpProtocolPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //生成按钮
            if (GUILayout.Button("生成"))
            {
                if (string.IsNullOrEmpty(m_Data.ProtoExePath))
                {
                    Debug.LogError("请设置proto.exe及proto文件路径");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.GenerateCSharpProtocolPath))
                {
                    Debug.LogError("请设置通信协议文件保存路径");
                    return;
                }

                Generate(m_Data);
            }

            EditorGUILayout.EndVertical();
        }

        private static void Generate(Proto2CSharpWindowData windowData)
        {
            string exeFilePath = string.Empty;
            var dir = new DirectoryInfo(windowData.ProtoExePath);
            var files = dir.GetFiles();
            foreach(var file in files)
            {
                if (file.Name.ToLower().Equals("protoc.exe"))
                {
                    exeFilePath = file.FullName;
                }
            }
            if (string.IsNullOrEmpty(exeFilePath))
            {
                Debug.LogError($"proto文件路径 {windowData.ProtoExePath} 中没有找到protoc.exe文件");
                return;
            }
            List<string> fileNames = new List<string>();
            foreach(var file in files)
            {
                if (file.Name.EndsWith(".proto"))
                {
                    var args = $"-I=./ {file.Name} --csharp_out=./";

                    var p = new System.Diagnostics.Process();
                    try
                    {
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.FileName = exeFilePath;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.Arguments = args;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.WorkingDirectory = dir.FullName;
                        p.Start();
                        p.WaitForExit();
                        p.Close();
                        Debug.Log($"proto文件 {file.FullName} 生成完毕");
                        fileNames.Add(file.Name.Replace(".proto", ""));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }


            Debug.Log("proto相关文件生成完毕");

            if (!Directory.Exists(windowData.GenerateCSharpProtocolPath))
            {
                Directory.CreateDirectory(windowData.GenerateCSharpProtocolPath);
            }

            for (int i = 0; i < fileNames.Count; i++)
            {
                var fileName = fileNames[i] + "Protocol.cs";
                var filePath = Path.Combine(windowData.GenerateCSharpProtocolPath, fileName);
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, GenerateCSTemplete
                        .Replace("#NAME#", fileNames[i])
                        .Replace("#YEAR#", System.DateTime.Now.Year.ToString())
                        .Replace("#TIME#", System.DateTime.Now.ToString()));
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
