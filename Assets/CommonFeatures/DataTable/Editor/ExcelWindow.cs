using LitJson;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.DataTable
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

        /// <summary>
        /// 代码模板_头部
        /// </summary>
        private const string GenerateCSTemplete_DR_Header =
@"//------------------------------------------------------------
// oops-framework-cocos2unity
// Copyright © #YEAR# movinwu. All rights reserved.
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：#TIME#
//------------------------------------------------------------

using CommonFeatures.DataTable;
using System.IO;

namespace HotfixScripts
{
    /// <summary>
    /// #NAME#数据
    /// </summary>
    public class DR_#NAME# : DataRow
    {
        public override int ID { get => id; }

";

        /// <summary>
        /// 代码模板_数据行_属性
        /// </summary>
        private const string GenerateCSTemplete_DR_Attribute =
@"        /// <summary>
        /// #SUMMARY#
        /// </summary>
        public #TYPE# #NAME# { get; private set; }

";

        /// <summary>
        /// 代码模板_数据行_函数
        /// </summary>
        private const string GenerateCSTemplete_DR_Function =
@"        public override void FromBinary(BinaryReader br)
        {
";

        /// <summary>
        /// 代码模板_数据行_函数内容
        /// </summary>
        private const string GenerateCSTemplete_DR_FunctionContent =
@"            #READ#;
";

        /// <summary>
        /// 代码模板_数据行_尾部
        /// </summary>
        private const string GenerateCSTemplete_DR_Tail =
@"        }
    }
}
";

        /// <summary>
        /// 代码模板_数据表
        /// </summary>
        private const string GenerateCSTemplete_DT = @"//------------------------------------------------------------
// oops-framework-cocos2unity
// Copyright © #YEAR# movinwu. All rights reserved.
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：#TIME#
//------------------------------------------------------------

using CommonFeatures.DataTable;
using CommonFeatures.Log;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HotfixScripts
{
    /// <summary>
    /// 数据表基类
    /// </summary>
    public class DT_#NAME# : IDataTable
    {
        /// <summary>
        /// 所有数据
        /// </summary>
        private Dictionary<int, DR_#NAME#> m_AllDataDic;

        /// <summary>
        /// 所有数据
        /// </summary>
        private DR_#NAME#[] m_AllDataArray;

        public void FromBinary(BinaryReader reader)
        {
            m_AllDataDic = new Dictionary<int, DR_#NAME#>();
            int count = reader.ReadInt32();
            m_AllDataArray = new DR_#NAME#[count];
            for (int i = 0; i < count; i++)
            {
                var dataRow = new DR_#NAME#();
                dataRow.FromBinary(reader);
                m_AllDataDic.Add(dataRow.ID, dataRow);
                m_AllDataArray[i] = dataRow;
            }
        }

        public void FromJson(string json)
        {
            m_AllDataArray = JsonMapper.ToObject<DR_#NAME#[]>(json);
            m_AllDataDic = new Dictionary<int, DR_#NAME#>();
            for (int i = 0; i < m_AllDataArray.Length; i++)
            {
                m_AllDataDic.Add(m_AllDataArray[i].ID, m_AllDataArray[i]);
            }
        }

        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <param name=""id""></param>
        /// <returns></returns>
        public T GetDataRow<T>(int id) where T : DataRow
        {
            if (null == m_AllDataDic)
            {
                CommonLog.ModelError($""读取表格 {typeof(T)} 时发现表格没有初始化"");
                return default(T);
            }

            if (m_AllDataDic.TryGetValue(id, out var t))
            {
                return t as T;
            }
            return default(T);
        }

        /// <summary>
        /// 获取满足指定条件,按照指定顺序排列的所有数据
        /// </summary>
        /// <param name=""predicate""></param>
        /// <param name=""comparer""></param>
        /// <returns></returns>
        public List<T> GetDataRows<T>(Predicate<T> predicate = null, Comparer<T> comparer = null) where T : DataRow
        {
            if (null == m_AllDataArray)
            {
                CommonLog.ModelError($""读取表格 {typeof(T)} 时发现表格没有初始化"");
                return new List<T>(0);
            }

            if (null == predicate && null == comparer)
            {
                return m_AllDataArray.Select(x => x as T).ToList();
            }
            else if (null == predicate)
            {
                var result = m_AllDataArray.Select(x => x as T).ToList();
                result.Sort(comparer);
                return result;
            }
            else if (null == comparer)
            {
                return m_AllDataArray.Select(x => x as T).Where(x => predicate(x)).ToList();
            }
            else
            {
                var result = m_AllDataArray.Select(x => x as T).Where(x => predicate(x)).ToList();
                result.Sort(comparer);
                return result;
            }
        }
    }
}
";

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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成代码文件路径:");
            if (GUILayout.Button(m_Data.codeGeneratePath))
            {
                var newPath = EditorUtility.SaveFolderPanel("请选择生成代码文件保存路径", m_Data.codeGeneratePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.codeGeneratePath = newPath;
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
                if (string.IsNullOrEmpty(m_Data.bytePath))
                {
                    Debug.LogError("请设置生成byte文件路径");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.codeGeneratePath))
                {
                    Debug.LogError("请设置生成代码文件路径");
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
                                        CommonFeatures.Log.CommonLog.ModelError($"表 {file.Name} sheet {sheet.Name} 数据读取行列数不正确, 行数 {row} , 列数 {col}");
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
                                        using (var bw = new BinaryWriter(byteStream, System.Text.Encoding.UTF8))
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
                                                    CommonFeatures.Log.CommonLog.ModelError($"表 {file.Name} sheet {sheet.Name} 数据类型为空, 列数 {col}");
                                                    continue;
                                                }
                                                var excelType = ExcelType<int>.GenerateExcelType(type, $"表 {file.Name} sheet {sheet.Name} 数据类型不正确, 列数 {col}, 类型 {type}");
                                                if (null != excelType)
                                                {
                                                    excelType.Col = c;
                                                    excelType.Name = sheet.Cells[4, c].Value?.ToString();
                                                    if (string.IsNullOrEmpty(excelType.Name))
                                                    {
                                                        CommonFeatures.Log.CommonLog.ModelError($"表 {file.Name} sheet {sheet.Name} 数据名称为空, 列数 {col}");
                                                        continue;
                                                    }
                                                    excelType.Summary = sheet.Cells[1, c].Value?.ToString();
                                                    excelType.ExcelName = file.Name;
                                                    excelType.SheetName = sheet.Name;
                                                    typeList.Add(excelType);
                                                }
                                            }

                                            bw.Write(row - 5);

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
                                                using (var sw = new StreamWriter(jsonStream, System.Text.Encoding.UTF8))
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

                                    if (!Directory.Exists(m_Data.codeGeneratePath))
                                    {
                                        Directory.CreateDirectory(m_Data.codeGeneratePath);
                                    }

                                    var fileName = $"DR_{name}.cs";
                                    var filePath = Path.Combine(m_Data.codeGeneratePath, fileName);
                                    if (File.Exists(filePath))
                                    {
                                        File.Delete(filePath);
                                    }
                                    System.Text.StringBuilder generateCSTemplete = new System.Text.StringBuilder(GenerateCSTemplete_DR_Header
                                        .Replace("#NAME#", name)
                                        .Replace("#YEAR#", System.DateTime.Now.Year.ToString())
                                        .Replace("#TIME#", System.DateTime.Now.ToString()));
                                    for (int i = 0; i < typeList.Count; i++)
                                    {
                                        generateCSTemplete.Append(GenerateCSTemplete_DR_Attribute
                                            .Replace("#NAME#", typeList[i].Name))
                                            .Replace("#SUMMARY#", typeList[i].Summary)
                                            .Replace("#TYPE#", typeList[i].CSTemplateTypeName());
                                    }
                                    generateCSTemplete.Append(GenerateCSTemplete_DR_Function);
                                    for (int i = 0; i < typeList.Count; i++)
                                    {
                                        generateCSTemplete.Append(GenerateCSTemplete_DR_FunctionContent
                                            .Replace("#READ#", typeList[i].CSTempleteByteReadFuncName("this." + typeList[i].Name)));
                                    }
                                    generateCSTemplete.Append(GenerateCSTemplete_DR_Tail);
                                    File.WriteAllText(filePath, generateCSTemplete.ToString());

                                    var dtFileName = $"DT_{name}.cs";
                                    var dtFilePath = Path.Combine(m_Data.codeGeneratePath, dtFileName);
                                    if (File.Exists(dtFilePath))
                                    {
                                        File.Delete(dtFilePath);
                                    }

                                    File.WriteAllText(dtFilePath, GenerateCSTemplete_DT
                                        .Replace("#NAME#", name)
                                        .Replace("#YEAR#", System.DateTime.Now.Year.ToString())
                                        .Replace("#TIME#", System.DateTime.Now.ToString()));
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        CommonFeatures.Log.CommonLog.ModelError($"表格 {file.FullName} 读取失败,检查是否已经打开表格");
                        CommonFeatures.Log.CommonLog.ModelException(ex);
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
