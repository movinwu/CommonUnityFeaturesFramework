using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ���ݱ��ֶ��������ͷ�װ
    /// </summary>
    public interface IExcelType
    {
        /// <summary>
        /// �������
        /// </summary>
        public string ExcelName { get; set; }

        /// <summary>
        /// sheet����
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// ��Ӧ������
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// ��ǰ��ȡ��Ӧ������
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// �ֶ�����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �ֶ�ע��
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="data"></param>
        /// <param name="row"></param>
        public void SetData(string data, int row);

        /// <summary>
        /// д�뵽json
        /// </summary>
        /// <param name="jsonData"></param>
        public void WriteJson(JsonData jsonData);

        /// <summary>
        /// ��ӵ�json
        /// </summary>
        /// <param name="jsonData"></param>
        public void AddJson(JsonData jsonData);

        /// <summary>
        /// д�뵽byte
        /// </summary>
        /// <param name="binaryWriter"></param>
        public void WriteByte(BinaryWriter binaryWriter);
    }
}
