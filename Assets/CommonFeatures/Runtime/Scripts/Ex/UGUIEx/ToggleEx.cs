using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UIEx
{
    /// <summary>
    /// toggle�����չ
    /// <para>���<see cref="ToggleEx"/>һ��ʹ��,Ҳ�����<see cref="PageEx"/>һ��ʹ��</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ToggleEx : MonoBehaviour
    {
        private Button m_Button;

        /// <summary>
        /// toggle��ť
        /// </summary>
        internal Button ToggleBtn
        {
            get
            {
                if (null == m_Button)
                {
                    m_Button = GetComponent<Button>();
                }
                return m_Button;
            }
        }

        [Header("��Ӧ��ʾ��ҳ��,��Ϊ��")]
        [SerializeField] private PageEx m_Page;

        public PageEx Page { get => m_Page; }
    }
}
