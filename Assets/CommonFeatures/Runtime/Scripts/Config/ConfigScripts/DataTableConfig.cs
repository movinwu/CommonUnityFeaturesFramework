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
        /// <summary>
        /// ���ݱ���ط�ʽ
        /// </summary>
        public EDataReadType DataReadType = EDataReadType.Binary;

        /// <summary>
        /// �����ֽ��ļ���ַ
        /// </summary>
        public string BinaryPath = "Assets/Hotfix/Res/DataTable/Binary";

        /// <summary>
        /// ����json�ļ���ַ
        /// </summary>
        public string JsonPath = "Assets/Hotfix/Res/DataTable/Json";

        /// <summary>
        /// �ȸ���������
        /// </summary>
        public string AssemblyName = "HotfixScripts";
    }
}
