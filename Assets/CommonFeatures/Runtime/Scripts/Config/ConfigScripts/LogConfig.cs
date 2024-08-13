using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// ��־����
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Log", fileName = "LogConfig")]
    public class LogConfig : ScriptableObject
    {
        [Header("����������־")]
        public bool EnableNet = true;

        [Header("������׼��־")]
        public bool EnableTrace = true;

        [Header("������Դ��־")]
        public bool EnableResource = true;

        [Header("����debug��־")]
        public bool EnableDebug = true;
    }
}
