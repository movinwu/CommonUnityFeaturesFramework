using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OOPS
{
    public class ExcelType_Int : ExcelType<int>
    {
        public override void SetData(string data)
        {
            if (!int.TryParse(data, out this.m_Data))
            {
                LogError(data);
            }
        }

        public override void WriteByte(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.m_Data);
        }

        public override void AddJson(JsonData jsonData)
        {
            jsonData.Add(this.m_Data);
        }

        public override void WriteJson(JsonData jsonData)
        {
            jsonData[this.Name] = this.m_Data;
        }
    }
}
