using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    public class ExcelType_Array<T> : ExcelType<IExcelType[]> where T : IExcelType, new()
    {
        public override void SetData(string data)
        {
            var datas = data.Split('#');
            if (null == this.m_Data)
            {
                this.m_Data = new IExcelType[datas.Length];
                for (int i = 0; i < this.m_Data.Length; i++)
                {
                    this.m_Data[i] = new T();
                    this.m_Data[i].Col = this.Col;
                    this.m_Data[i].SheetName = this.SheetName;
                    this.m_Data[i].ExcelName = this.ExcelName;
                    this.m_Data[i].Name = this.Name;
                    this.m_Data[i].Summary = this.Summary;
                }
            }
            for (int i = 0; i < this.m_Data.Length; i++)
            {
                this.m_Data[i].SetData(datas[i], this.Row);
            }
        }

        public override void WriteByte(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.m_Data.Length);
            for (int i = 0; i < this.m_Data.Length; i++)
            {
                this.m_Data[i].WriteByte(binaryWriter);
            }
        }

        public override void AddJson(JsonData jsonData)
        {
            var newJsonData = new JsonData();
            newJsonData.SetJsonType(JsonType.Array);
            for (int i = 0; i < this.m_Data.Length; i++)
            {
                this.m_Data[i].AddJson(newJsonData);
            }
            jsonData.Add(newJsonData);
        }

        public override void WriteJson(JsonData jsonData)
        {
            var newJsonData = new JsonData();
            newJsonData.SetJsonType(JsonType.Array);
            for (int i = 0; i < this.m_Data.Length; i++)
            {
                this.m_Data[i].AddJson(newJsonData);
            }
            jsonData[this.Name] = newJsonData;
        }
    }
}
