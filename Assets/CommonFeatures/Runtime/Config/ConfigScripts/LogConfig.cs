using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// ��־����
    /// </summary>
    [CreateAssetMenu]
    public class LogConfig : ScriptableObject
    {
        /// <summary>
        /// ����������־
        /// </summary>
        public bool EnableNet = true;

        /// <summary>
        /// ����������־
        /// </summary>
        public bool EnableConfig = true;

        /// <summary>
        /// ������׼��־
        /// </summary>
        public bool EnableTrace = true;

        /// <summary>
        /// ������Դ��־
        /// </summary>
        public bool EnableResource = true;
    }
}
