using HotfixScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    public class TestDataTable : MonoBehaviour
    {
        private void Start()
        {
            DataTableManager.Instance.ReadDataTable();
            var datas = DataTableManager.Instance.GetDataRows<DR_Example>();
            for (int i = 0; i < datas.Count; i++)
            {
                Logger.Model(LitJson.JsonMapper.ToJson(datas[i]));
            }
        }
    }
}
