using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// 软件设置
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Application", fileName = "ApplicationConfig")]
    public class ApplicationConfig : ScriptableObject
    {
        /// <summary>
        /// 应用版本
        /// </summary>
        public string ApplicationVersion { get => Application.version; }

        [Header("热更新版本")]
        public string HotfixVersion = "0";

        public string FullVersion { get => $"{ApplicationVersion}.{HotfixVersion}"; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string ApplicationName { get => Application.productName; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get => Application.companyName; }
    }
}
