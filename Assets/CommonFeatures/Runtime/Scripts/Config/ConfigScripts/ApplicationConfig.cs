using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// �������
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Application", fileName = "ApplicationConfig")]
    public class ApplicationConfig : ScriptableObject
    {
        /// <summary>
        /// Ӧ�ð汾
        /// </summary>
        public string ApplicationVersion { get => Application.version; }

        [Header("�ȸ��°汾")]
        public string HotfixVersion = "0";

        public string FullVersion { get => $"{ApplicationVersion}.{HotfixVersion}"; }

        /// <summary>
        /// Ӧ������
        /// </summary>
        public string ApplicationName { get => Application.productName; }

        /// <summary>
        /// ��˾����
        /// </summary>
        public string CompanyName { get => Application.companyName; }
    }
}
