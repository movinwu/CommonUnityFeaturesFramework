using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// 日志配置
    /// </summary>
    [CreateAssetMenu]
    public class LogConfig : ScriptableObject
    {
        /// <summary>
        /// 开启网络日志
        /// </summary>
        public bool EnableNet = true;

        /// <summary>
        /// 开启配置日志
        /// </summary>
        public bool EnableConfig = true;

        /// <summary>
        /// 开启标准日志
        /// </summary>
        public bool EnableTrace = true;

        /// <summary>
        /// 开启资源日志
        /// </summary>
        public bool EnableResource = true;
    }
}
