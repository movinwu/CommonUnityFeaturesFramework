using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// 日志配置
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Log", fileName = "LogConfig")]
    public class LogConfig : ScriptableObject
    {
        [Header("开启网络日志")]
        public bool EnableNet = true;

        [Header("开启配置日志")]
        public bool EnableConfig = true;

        [Header("开启标准日志")]
        public bool EnableTrace = true;

        [Header("开启资源日志")]
        public bool EnableResource = true;
    }
}
