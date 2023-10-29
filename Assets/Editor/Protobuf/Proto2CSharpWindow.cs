using System.IO;
using UnityEditor;
using UnityEngine;

namespace OOPS
{
    class Proto2CSharpConfig
    {
        public string Namespace;
        public string ProtoExe;
        public string ProtoFilePath;
        public string Proto2CSharpPath;
    }

    public class Proto2CSharpWindow : EditorWindow
    {
        [MenuItem("Tools/ProtoתC#���� &6")]
        static void OpenWindow()
        {
            var window = GetWindow<Proto2CSharpWindow>("ProtoתC#����", true);
            window.minSize = new Vector2(700, 300);
            window.Show();
        }

        private bool _isInit = false;

        private string _exePath = string.Empty;
        private string _protoJsonPath = string.Empty;

        private void OnGUI()
        {
            if (!_isInit)
            {
                InitCfg();
                _isInit = true;
            }

            EditorGUILayout.BeginVertical();

            OnExeExeGUI();
            EditorGUILayout.Space();
            OnGenerateGUI();

            EditorGUILayout.EndVertical();
        }

        void InitCfg()
        {
            string libPath = Application.dataPath.Replace("Assets", "Library");
            _protoJsonPath = Path.Combine(libPath, "proto2CSharpConfig.json");

            var fileExist = File.Exists(_protoJsonPath);
            //���ļ� �Ͷ��ļ�
            if (fileExist)
            {
                var json = File.ReadAllText(_protoJsonPath);
                var cfg = new Proto2CSharpConfig();
                EditorJsonUtility.FromJsonOverwrite(json, cfg);

                _exePath = cfg.ProtoExe;
            }
        }

        void OnExeExeGUI()
        {
            GUILayout.Label("protoc.exe��proto�ļ���ַ��");
            GUILayout.BeginHorizontal();

            string preValue = _exePath;
            if (string.IsNullOrEmpty(preValue))
            {
                preValue = Path.GetDirectoryName(Application.dataPath);
            }
            //��������ť
            if (GUILayout.Button("Browser", GUILayout.Width(100), GUILayout.Height(25)))
            {
                //��������ѡ���ļ���
                string selectFile = EditorUtility.OpenFilePanel("Select File", preValue, "");
                preValue = selectFile;
            }

            GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
            //����ļ���·��
            using (new EditorGUI.DisabledGroupScope(true))
            {
                preValue = GUILayout.TextField(preValue, GUILayout.Height(25));
            }

            if (!_exePath.Equals(preValue))
            {
                _exePath = preValue;
                SaveFile();
            }
            GUILayout.EndHorizontal();
        }

        void SaveFile()
        {
            var info = new Proto2CSharpConfig()
            {
                ProtoExe = _exePath,
            };
            var jsoninfo = EditorJsonUtility.ToJson(info);
            File.WriteAllText(_protoJsonPath, jsoninfo);
        }

        void OnGenerateGUI()
        {
            if (GUILayout.Button("����"))
            {
                if (string.IsNullOrEmpty(_exePath))
                {
                    Debug.LogError("������protoc.exe·��");
                    return;
                }

                Generate();
            }
        }

        void Generate()
        {
            if (!File.Exists(_exePath))
            {
                Debug.LogError("protoc.exe·��������");
            }
            var exeFile = new FileInfo(_exePath);
            var dir = exeFile.Directory;
            var files = dir.GetFiles();
            foreach(var file in files)
            {
                if (file.Name.EndsWith(".proto"))
                {
                    var args = $"-I=./ {file.Name} --csharp_out=./";

                    var p = new System.Diagnostics.Process();
                    try
                    {
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.FileName = _exePath;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.Arguments = args;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.WorkingDirectory = dir.FullName;
                        p.Start();
                        p.WaitForExit();
                        p.Close();
                        Debug.Log($"proto�ļ� {file.FullName} �������");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }


            Debug.Log("proto����ļ��������");

            AssetDatabase.Refresh();
        }
    }
}
