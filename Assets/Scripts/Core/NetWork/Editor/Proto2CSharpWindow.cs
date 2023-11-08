using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OOPS
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
        [MenuItem("Tools/Proto转C#")]
        static void OpenWindow()
        {
            var window = GetWindow<Proto2CSharpWindow>("Proto转C#工具", true);
            window.minSize = new Vector2(700, 300);
        }

        private Proto2CSharpWindowData m_Data;

        private const string DataSaveName = "Proto2CSharp";

        private const string GenerateCSTemplete =
@"namespace OOPS
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

                Generate();
            }

            EditorGUILayout.EndVertical();
        }

        void Generate()
        {
            string exeFilePath = string.Empty;
            var dir = new DirectoryInfo(m_Data.ProtoExePath);
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
                Debug.LogError($"proto文件路径 {m_Data.ProtoExePath} 中没有找到protoc.exe文件");
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

            if (!Directory.Exists(m_Data.GenerateCSharpProtocolPath))
            {
                Directory.CreateDirectory(m_Data.GenerateCSharpProtocolPath);
            }

            for (int i = 0; i < fileNames.Count; i++)
            {
                var fileName = fileNames[i] + "Protocol.cs";
                var filePath = Path.Combine(m_Data.GenerateCSharpProtocolPath, fileName);
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, GenerateCSTemplete.Replace("#NAME#", fileNames[i]));
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
