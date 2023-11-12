using LitJson;
using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OOPS
{
    public class ExcelWindowData : CustomEditorData 
    {
        /// <summary>
        /// excel文件夹路径
        /// </summary>
        public string excelPath;

        /// <summary>
        /// 生成的json文件路径
        /// </summary>
        public string jsonPath;

        /// <summary>
        /// 生成的byte文件路径
        /// </summary>
        public string bytePath;

        /// <summary>
        /// 生成的代码文件路径
        /// </summary>
        public string codeGeneratePath;
    }

    public class ExcelWindow : EditorWindow
    {
        private const string DataSavePath = "Excel";

        private ExcelWindowData m_Data;

        [MenuItem("Tools/打开导表窗口")]
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
            EditorGUILayout.LabelField("Excel路径:");
            if (GUILayout.Button(m_Data.excelPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("请选择excel文件路径", m_Data.excelPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.excelPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成json文件路径:");
            if (GUILayout.Button(m_Data.jsonPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("请选择生成json文件保存路径", m_Data.jsonPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.jsonPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成byte文件路径:");
            if (GUILayout.Button(m_Data.bytePath))
            {
                var newPath = EditorUtility.SaveFolderPanel("请选择生成byte文件保存路径", m_Data.bytePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.bytePath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //生成按钮
            if (GUILayout.Button("生成"))
            {
                if (string.IsNullOrEmpty(m_Data.excelPath))
                {
                    Debug.LogError("请设置excel文件路径");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("请设置生成json文件路径");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("请设置生成byte文件路径");
                    return;
                }

                Generate();
            }

            EditorGUILayout.EndVertical();
        }

        private void Generate()
        {
            var directory = new DirectoryInfo(m_Data.excelPath);
            var files = directory.GetFiles();
            for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
            {
                var file = files[fileIndex];
                if (file.Name.EndsWith(".xlsx"))
                {
                    try
                    {
                        using (var package = new ExcelPackage(file))
                        {
                            for (int sheetIndex = 1; sheetIndex <= package.Workbook.Worksheets.Count; sheetIndex++)
                            {
                                var sheet = package.Workbook.Worksheets[sheetIndex];
                                if (sheet.Name.StartsWith("D_"))
                                {
                                    //读取数据范围
                                    int rowCount = sheet.Dimension.Rows;
                                    int colCount = sheet.Dimension.Columns;
                                    int row = rowCount + 1;
                                    int col = colCount + 1;
                                    for (int r = 2; r <= rowCount; r++)
                                    {
                                        if (string.IsNullOrEmpty(sheet.Cells[r, 2].Value?.ToString()))
                                        {
                                            row = r;
                                            break;
                                        }
                                    }
                                    for (int c = 2; c <= colCount; c++)
                                    {
                                        if (string.IsNullOrEmpty(sheet.Cells[2, c].Value?.ToString()))
                                        {
                                            col = c;
                                            break;
                                        }
                                    }

                                    //校验行列数据
                                    if (row < 6 || col < 3)
                                    {
                                        Logger.ModelError($"表 {file.Name} sheet {sheet.Name} 数据读取行列数不正确, 行数 {row} , 列数 {col}");
                                        return;
                                    }

                                    List<IExcelType> typeList = new List<IExcelType>(col - 2);//数据处理类
                                    var name = sheet.Name.Substring(2);
                                    //序列化byte数据
                                    var byteName = name + ".byte";
                                    var byteFullPath = Path.Combine(m_Data.bytePath, byteName);
                                    if (File.Exists(byteFullPath))
                                    {
                                        File.Delete(byteFullPath);
                                    }
                                    using (var byteStream = File.Create(byteFullPath))
                                    {
                                        using (var bw = new BinaryWriter(byteStream))
                                        {
                                            JsonData jsonData = new JsonData();

                                            //生成数据处理类
                                            for (int c = 2; c < col; c++)
                                            {
                                                //只读取客户端
                                                var clientReadSymbol = sheet.Cells[3, c].Value?.ToString().ToLower();
                                                if (string.IsNullOrEmpty(clientReadSymbol) || !clientReadSymbol.Contains('c'))
                                                {
                                                    continue;
                                                }

                                                var type = sheet.Cells[2, c].Value?.ToString().ToLower();
                                                if (string.IsNullOrEmpty(type))
                                                {
                                                    Logger.ModelError($"表 {file.Name} sheet {sheet.Name} 数据类型为空, 列数 {col}");
                                                    continue;
                                                }
                                                var excelType = ExcelType<int>.GenerateExcelType(type, $"表 {file.Name} sheet {sheet.Name} 数据类型不正确, 列数 {col}, 类型 {type}");
                                                if (null != excelType)
                                                {
                                                    excelType.Col = c;
                                                    excelType.Name = sheet.Cells[4, c].Value?.ToString();
                                                    if (string.IsNullOrEmpty(excelType.Name))
                                                    {
                                                        Logger.ModelError($"表 {file.Name} sheet {sheet.Name} 数据名称为空, 列数 {col}");
                                                        continue;
                                                    }
                                                    excelType.Summary = sheet.Cells[1, c].Value?.ToString();
                                                    excelType.ExcelName = file.Name;
                                                    excelType.SheetName = sheet.Name;
                                                    typeList.Add(excelType);
                                                }
                                            }

                                            bw.Write(row - 4);

                                            //读取数据
                                            for (int r = 5; r < row; r++)
                                            {
                                                var rowJsonData = new JsonData();
                                                for (int c = 0; c < typeList.Count; c++)
                                                {
                                                    var excelType = typeList[c];
                                                    var data = sheet.Cells[r, excelType.Col].Value?.ToString();
                                                    if (string.IsNullOrEmpty(data))
                                                    {
                                                        data = string.Empty;
                                                    }
                                                    excelType.SetData(data, r);
                                                    excelType.WriteJson(rowJsonData);
                                                    excelType.WriteByte(bw);
                                                }
                                                jsonData.Add(rowJsonData);
                                            }

                                            //序列化json数据
                                            var jsonName = name + ".json";
                                            var jsonFullPath = Path.Combine(m_Data.jsonPath, jsonName);
                                            if (File.Exists(jsonFullPath))
                                            {
                                                File.Delete(jsonFullPath);
                                            }
                                            using (var jsonStream = File.Create(jsonFullPath))
                                            {
                                                using (var sw = new StreamWriter(jsonStream))
                                                {
                                                    sw.Write(jsonData.ToJson());
                                                }
                                            }
                                        }
                                    }

                                    //生成代码
                                    if (typeList.Count == 0)
                                    {
                                        continue;
                                    }

                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Logger.ModelError($"表格 {file.FullName} 读取失败,检查是否已经打开表格");
                        Logger.ModelException(ex);
                    }
                }
            }
            
        }
    }
}
