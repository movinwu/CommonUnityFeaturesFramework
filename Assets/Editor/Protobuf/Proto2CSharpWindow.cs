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
        [MenuItem("Tools/Proto转C#工具 &6")]
        static void OpenWindow()
        {
            var window = GetWindow<Proto2CSharpWindow>("Proto转C#工具", true);
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
            //有文件 就读文件
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
            GUILayout.Label("protoc.exe及proto文件地址：");
            GUILayout.BeginHorizontal();

            string preValue = _exePath;
            if (string.IsNullOrEmpty(preValue))
            {
                preValue = Path.GetDirectoryName(Application.dataPath);
            }
            //输出浏览按钮
            if (GUILayout.Button("Browser", GUILayout.Width(100), GUILayout.Height(25)))
            {
                //弹出窗口选择文件夹
                string selectFile = EditorUtility.OpenFilePanel("Select File", preValue, "");
                preValue = selectFile;
            }

            GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
            //输出文件夹路径
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
            if (GUILayout.Button("生成"))
            {
                if (string.IsNullOrEmpty(_exePath))
                {
                    Debug.LogError("请设置protoc.exe路径");
                    return;
                }

                Generate();
            }
        }

        void Generate()
        {
            if (!File.Exists(_exePath))
            {
                Debug.LogError("protoc.exe路径不存在");
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
                        Debug.Log($"proto文件 {file.FullName} 生成完毕");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }


            Debug.Log("proto相关文件生成完毕");

            AssetDatabase.Refresh();
        }
    }
}
