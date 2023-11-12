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
        /// excel�ļ���·��
        /// </summary>
        public string excelPath;

        /// <summary>
        /// ���ɵ�json�ļ�·��
        /// </summary>
        public string jsonPath;

        /// <summary>
        /// ���ɵ�byte�ļ�·��
        /// </summary>
        public string bytePath;

        /// <summary>
        /// ���ɵĴ����ļ�·��
        /// </summary>
        public string codeGeneratePath;
    }

    public class ExcelWindow : EditorWindow
    {
        private const string DataSavePath = "Excel";

        private ExcelWindowData m_Data;

        [MenuItem("Tools/�򿪵�����")]
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
            EditorGUILayout.LabelField("Excel·��:");
            if (GUILayout.Button(m_Data.excelPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("��ѡ��excel�ļ�·��", m_Data.excelPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.excelPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("����json�ļ�·��:");
            if (GUILayout.Button(m_Data.jsonPath))
            {
                var newPath = EditorUtility.SaveFolderPanel("��ѡ������json�ļ�����·��", m_Data.jsonPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.jsonPath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("����byte�ļ�·��:");
            if (GUILayout.Button(m_Data.bytePath))
            {
                var newPath = EditorUtility.SaveFolderPanel("��ѡ������byte�ļ�����·��", m_Data.bytePath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    m_Data.bytePath = newPath;
                    CustomEditorDataFactory.WriteData(DataSavePath, m_Data);
                }
            }
            EditorGUILayout.EndHorizontal();

            //���ɰ�ť
            if (GUILayout.Button("����"))
            {
                if (string.IsNullOrEmpty(m_Data.excelPath))
                {
                    Debug.LogError("������excel�ļ�·��");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("����������json�ļ�·��");
                    return;
                }
                if (string.IsNullOrEmpty(m_Data.jsonPath))
                {
                    Debug.LogError("����������byte�ļ�·��");
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
                                    //��ȡ���ݷ�Χ
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

                                    //У����������
                                    if (row < 6 || col < 3)
                                    {
                                        Logger.ModelError($"�� {file.Name} sheet {sheet.Name} ���ݶ�ȡ����������ȷ, ���� {row} , ���� {col}");
                                        return;
                                    }

                                    List<IExcelType> typeList = new List<IExcelType>(col - 2);//���ݴ�����
                                    var name = sheet.Name.Substring(2);
                                    //���л�byte����
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

                                            //�������ݴ�����
                                            for (int c = 2; c < col; c++)
                                            {
                                                //ֻ��ȡ�ͻ���
                                                var clientReadSymbol = sheet.Cells[3, c].Value?.ToString().ToLower();
                                                if (string.IsNullOrEmpty(clientReadSymbol) || !clientReadSymbol.Contains('c'))
                                                {
                                                    continue;
                                                }

                                                var type = sheet.Cells[2, c].Value?.ToString().ToLower();
                                                if (string.IsNullOrEmpty(type))
                                                {
                                                    Logger.ModelError($"�� {file.Name} sheet {sheet.Name} ��������Ϊ��, ���� {col}");
                                                    continue;
                                                }
                                                var excelType = ExcelType<int>.GenerateExcelType(type, $"�� {file.Name} sheet {sheet.Name} �������Ͳ���ȷ, ���� {col}, ���� {type}");
                                                if (null != excelType)
                                                {
                                                    excelType.Col = c;
                                                    excelType.Name = sheet.Cells[4, c].Value?.ToString();
                                                    if (string.IsNullOrEmpty(excelType.Name))
                                                    {
                                                        Logger.ModelError($"�� {file.Name} sheet {sheet.Name} ��������Ϊ��, ���� {col}");
                                                        continue;
                                                    }
                                                    excelType.Summary = sheet.Cells[1, c].Value?.ToString();
                                                    excelType.ExcelName = file.Name;
                                                    excelType.SheetName = sheet.Name;
                                                    typeList.Add(excelType);
                                                }
                                            }

                                            bw.Write(row - 4);

                                            //��ȡ����
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

                                            //���л�json����
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

                                    //���ɴ���
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
                        Logger.ModelError($"��� {file.FullName} ��ȡʧ��,����Ƿ��Ѿ��򿪱��");
                        Logger.ModelException(ex);
                    }
                }
            }
            
        }
    }
}
