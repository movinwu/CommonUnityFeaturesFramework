using CommonFeatures.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// 数据表配置
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/DataTable", fileName = "DataTableConfig")]
    public class DataTableConfig : ScriptableObject
    {
        [Header("数据表加载方式")]
        public EDataReadType DataReadType = EDataReadType.Binary;

        [Header("数据字节文件地址")]
        public string BinaryPath = "Assets/Hotfix/Res/DataTable/Binary";

        [Header("数据json文件地址")]
        public string JsonPath = "Assets/Hotfix/Res/DataTable/Json";

        [Header("热更程序集名称")]
        public string AssemblyName = "HotfixScripts";
    }
}
