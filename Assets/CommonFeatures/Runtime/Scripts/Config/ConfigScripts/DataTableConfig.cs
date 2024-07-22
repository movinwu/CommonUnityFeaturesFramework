using CommonFeatures.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// ���ݱ�����
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/DataTable", fileName = "DataTableConfig")]
    public class DataTableConfig : ScriptableObject
    {
        [Header("���ݱ���ط�ʽ")]
        public EDataReadType DataReadType = EDataReadType.Binary;

        [Header("�����ֽ��ļ���ַ")]
        public string BinaryPath = "Assets/Hotfix/Res/DataTable/Binary";

        [Header("����json�ļ���ַ")]
        public string JsonPath = "Assets/Hotfix/Res/DataTable/Json";

        [Header("�ȸ���������")]
        public string AssemblyName = "HotfixScripts";
    }
}
