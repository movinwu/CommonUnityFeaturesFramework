using CommonFeatures.DataTable;
using CommonFeatures.Log;
using HotfixScripts;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestDataTable : MonoBehaviour
    {
        private void Start()
        {
            CFM.DataTable.ReadDataTable();
            var datas = CFM.DataTable.GetDataRows<DR_Example>();
            for (int i = 0; i < datas.Count; i++)
            {
                CommonLog.Config(LitJson.JsonMapper.ToJson(datas[i]));
                CommonLog.Config(datas[i].string_example);
            }
            var datas2 = CFM.DataTable.GetDataRows<DR_Example2>();
            for (int i = 0; i < datas2.Count; i++)
            {
                CommonLog.Config(LitJson.JsonMapper.ToJson(datas2[i]));
                CommonLog.Config(datas2[i].string_example);
            }
        }
    }
}
