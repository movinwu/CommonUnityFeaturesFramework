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
        /// <summary>
        /// 数据表加载方式
        /// </summary>
        public EDataReadType DataReadType = EDataReadType.Binary;

        /// <summary>
        /// 数据字节文件地址
        /// </summary>
        public string BinaryPath = "Assets/Hotfix/Res/DataTable/Binary";

        /// <summary>
        /// 数据json文件地址
        /// </summary>
        public string JsonPath = "Assets/Hotfix/Res/DataTable/Json";

        /// <summary>
        /// 热更程序集名称
        /// </summary>
        public string AssemblyName = "HotfixScripts";
    }
}
