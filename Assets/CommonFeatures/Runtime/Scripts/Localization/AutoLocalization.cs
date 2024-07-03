using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Localization
{
    /// <summary>
    /// �Զ������ػ����
    /// </summary>
    public class AutoLocalization : MonoBehaviour
    {
        [Header("����key")]
        [SerializeField] private List<string> m_MainLocalizationKeys;

        [Header("�ȸ�key")]
        [SerializeField] private List<int> m_HotfixLocalizationKeys;

        /// <summary>
        /// ˢ�±��ػ�����ί��
        /// </summary>
        public System.Func<List<string>, List<int>, string> RefreshLocalizationFunc { private get; set; }

        private void OnEnable()
        {
            
        }


    }
}
