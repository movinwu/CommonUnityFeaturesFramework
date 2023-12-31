using LitJson;
using System.IO;

namespace CommonFeatures.DataTable
{
    /// <summary>
    /// 数据表字段数据类型封装
    /// </summary>
    public interface IExcelType
    {
        /// <summary>
        /// 表格名称
        /// </summary>
        public string ExcelName { get; set; }

        /// <summary>
        /// sheet名称
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 对应的列数
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// 当前读取对应的行数
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段注释
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="row"></param>
        public void SetData(string data, int row);

        /// <summary>
        /// 写入到json
        /// </summary>
        /// <param name="jsonData"></param>
        public void WriteJson(JsonData jsonData);

        /// <summary>
        /// 添加到json
        /// </summary>
        /// <param name="jsonData"></param>
        public void AddJson(JsonData jsonData);

        /// <summary>
        /// 写入到byte
        /// </summary>
        /// <param name="binaryWriter"></param>
        public void WriteByte(BinaryWriter binaryWriter);

        /// <summary>
        /// cs模板代码_byte读取函数
        /// </summary>
        /// <param name="name">类型名称</param>
        public string CSTempleteByteReadFuncName(string name);

        /// <summary>
        /// cs模板代码_类型名
        /// </summary>
        /// <returns></returns>
        public string CSTemplateTypeName();
    }
}
