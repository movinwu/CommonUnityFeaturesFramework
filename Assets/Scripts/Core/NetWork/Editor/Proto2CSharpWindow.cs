using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OOPS
{
    public class Proto2CSharpWindowData : CustomEditorData
    {
        /// <summary>
        /// proto.exe ·��
        /// </summary>
        public string ProtoExePath;

        /// <summary>
        /// ������C#ͨ��Э���ļ���
        /// </summary>
        public string GenerateCSharpProtocolPath;
    }

    public class Proto2CSharpWindow : EditorWindow
    {
        [MenuItem("Tools/ProtoתC#")]
        static void OpenWindow()
        {
            var window = GetWindow<Proto2CSharpWindow>("ProtoתC#����", true);
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
            EditorGUILayout.LabelField("proto.exe�ļ���proto����·��(����ͬһ���ļ�����):");
            if (GUILayout.Button(m_Data.ProtoExePath))
            {
                var newPath = EditorUtility.OpenFolderPanel("��ѡ��proto.exe�ļ�·��", m_Data.ProtoExePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.ProtoExePath = newPath;
                    CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //proto����c#Э���ļ���
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("���ɵ�protoЭ���ļ�����·��:");
            if (GUILayout.Button(m_Data.GenerateCSharpProtocolPath))
            {
                var newPath = EditorUtility.OpenFolderPanel("��ѡ��protoЭ���ļ�����·��", m_Data.GenerateCSharpProtocolPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.GenerateCSharpProtocolPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSaveName, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //���ɰ�ť
            if (GUILayout.Button("����"))
            {
                if (string.IsNullOrEmpty(m_Data.ProtoExePath))
                {
                    Debug.LogError("������proto.exe��proto�ļ�·��");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.GenerateCSharpProtocolPath))
                {
                    Debug.LogError("������ͨ��Э���ļ�����·��");
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
                Debug.LogError($"proto�ļ�·�� {m_Data.ProtoExePath} ��û���ҵ�protoc.exe�ļ�");
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
                        Debug.Log($"proto�ļ� {file.FullName} �������");
                        fileNames.Add(file.Name.Replace(".proto", ""));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }


            Debug.Log("proto����ļ��������");

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
