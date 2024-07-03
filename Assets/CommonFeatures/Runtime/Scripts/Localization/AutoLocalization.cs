using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Localization
{
    /// <summary>
    /// 自动化本地化组件
    /// </summary>
    public class AutoLocalization : MonoBehaviour
    {
        [Header("主包key")]
        [SerializeField] private List<string> m_MainLocalizationKeys;

        [Header("热更key")]
        [SerializeField] private List<int> m_HotfixLocalizationKeys;

        /// <summary>
        /// 刷新本地化语言委托
        /// </summary>
        public System.Func<List<string>, List<int>, string> RefreshLocalizationFunc { private get; set; }

        private void OnEnable()
        {
            
        }


    }
}
