using LitJson;
using System.IO;

namespace OOPS
{
    public abstract class ExcelType<T> : IExcelType
    {
        protected T m_Data;

        public string ExcelName { get; set; }
        public string SheetName { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        public void SetData(string data, int row)
        {
            this.Row = row;
            SetData(data);
        }

        public abstract void SetData(string data);

        public abstract void WriteByte(BinaryWriter binaryWriter);

        public abstract void AddJson(JsonData jsonData);

        public abstract void WriteJson(JsonData jsonData);

        protected void LogError(string content)
        {
            Logger.ModelError($"���ݱ� {ExcelName} sheet {SheetName} ת�����ݳ���, ����: {Row}, ����: {Col}, ����: {content}");
        }

        /// <summary>
        /// ����excel����
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="errorContent"></param>
        /// <returns></returns>
        public static IExcelType GenerateExcelType(string typeName, string errorContent)
        {
            switch (typeName)
            {
                case "byte":
                    return new ExcelType_Byte();
                case "short":
                    return new ExcelType_Short();
                case "int":
                    return new ExcelType_Int();
                case "long":
                    return new ExcelType_Long();
                case "string":
                    return new ExcelType_String();
                case "float":
                    return new ExcelType_Float();
                case "double":
                    return new ExcelType_Double();
                case "char":
                    return new ExcelType_Char();
                case "bool":
                    return new ExcelType_Bool();
                case "byte[]":
                    return new ExcelType_Array<ExcelType_Byte>();
                case "short[]":
                    return new ExcelType_Array<ExcelType_Short>();
                case "int[]":
                    return new ExcelType_Array<ExcelType_Int>();
                case "long[]":
                    return new ExcelType_Array<ExcelType_Long>();
                case "string[]":
                    return new ExcelType_Array<ExcelType_String>();
                case "float[]":
                    return new ExcelType_Array<ExcelType_Float>();
                case "double[]":
                    return new ExcelType_Array<ExcelType_Double>();
                case "char[]":
                    return new ExcelType_Array<ExcelType_Char>();
                case "bool[]":
                    return new ExcelType_Array<ExcelType_Bool>();
                case "byte[][]":
                    return new ExcelType_ArrayArray<ExcelType_Byte>();
                case "short[][]":
                    return new ExcelType_ArrayArray<ExcelType_Short>();
                case "int[][]":
                    return new ExcelType_ArrayArray<ExcelType_Int>();
                case "long[][]":
                    return new ExcelType_ArrayArray<ExcelType_Long>();
                case "string[][]":
                    return new ExcelType_ArrayArray<ExcelType_String>();
                case "float[][]":
                    return new ExcelType_ArrayArray<ExcelType_Float>();
                case "double[][]":
                    return new ExcelType_ArrayArray<ExcelType_Double>();
                case "char[][]":
                    return new ExcelType_ArrayArray<ExcelType_Char>();
                case "bool[][]":
                    return new ExcelType_ArrayArray<ExcelType_Bool>();
                default:
                    if (!typeName.ToLower().Equals("none"))
                    {
                        Logger.ModelError($"���ݱ��ʽת������, ����: {typeName}, ��Ϣ: {errorContent}");
                    }
                    return null;
            }
        }

        public abstract string CSTempleteByteReadFuncName(string name);

        public abstract string CSTemplateTypeName();
    }
}
