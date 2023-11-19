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
            DataTableManager.Instance.ReadDataTable();
            var datas = DataTableManager.Instance.GetDataRows<DR_Example>();
            for (int i = 0; i < datas.Count; i++)
            {
                CommonLog.Model(LitJson.JsonMapper.ToJson(datas[i]));
                CommonLog.Model(datas[i].string_example);
            }
            var datas2 = DataTableManager.Instance.GetDataRows<DR_Example2>();
            for (int i = 0; i < datas2.Count; i++)
            {
                CommonLog.Model(LitJson.JsonMapper.ToJson(datas2[i]));
                CommonLog.Model(datas2[i].string_example);
            }
        }
    }
}
