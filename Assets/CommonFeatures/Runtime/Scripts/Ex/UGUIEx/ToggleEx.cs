using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UIEx
{
    /// <summary>
    /// toggle组件拓展
    /// <para>配合<see cref="ToggleEx"/>一起使用,也可配合<see cref="PageEx"/>一起使用</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ToggleEx : MonoBehaviour
    {
        private Button m_Button;

        /// <summary>
        /// toggle按钮
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

        [Header("对应显示的页面,可为空")]
        [SerializeField] private PageEx m_Page;

        public PageEx Page { get => m_Page; }
    }
}
