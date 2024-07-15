using CommonFeatures.Event;
using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.Localization
{
    /// <summary>
    /// 自动化本地化组件
    /// </summary>
    public class AutoLocalization : MonoBehaviour
    {
        [Header("是否热更多语言")]
        [SerializeField] private bool m_IsHotfixLocalization = true;

        [Header("主包key,\"是否热更多语言\"不勾选时生效")]
        [SerializeField] private string m_MainLocalizationKey;

        [Header("热更key,\"是否热更多语言\"勾选时生效")]
        [SerializeField] private int m_HotfixLocalizationKey;

        /// <summary>
        /// 本地化多语言格式化
        /// <para>设置了格式化文本后,在刷新多语言时会自动调用string.Format函数进行格式化</para>
        /// </summary>
        private List<string> m_LocalizationFormat = new List<string>();

        /// <summary>
        /// 脏标记
        /// </summary>
        private bool m_IsDirty;

        public void OnShow()
        {
            this.m_IsDirty = true;
            CFM.Event.AddListener<LocalizationEvent>(OnLocalizationChange);
        }

        public void OnHide()
        {
            this.m_IsDirty = false;
            CFM.Event.RemoveListener<LocalizationEvent>(OnLocalizationChange);
        }

        private void LateUpdate()
        {
            if (this.m_IsDirty)
            {
                this.m_IsDirty = false;
                if (this.m_IsHotfixLocalization)
                {
                    var str = CFM.Localization.GetHotfixLocalizationConfig(m_HotfixLocalizationKey);
                    if (m_LocalizationFormat.Count == 0)
                    {
                        RefreshLocalization(str);
                    }
                    else
                    {
                        RefreshLocalization(string.Format(str, m_LocalizationFormat.ToArray()));
                    }
                }
                else
                {
                    var str = CFM.Localization.GetMainLocalization(m_MainLocalizationKey);
                    if (m_LocalizationFormat.Count == 0)
                    {
                        RefreshLocalization(str);
                    }
                    else
                    {
                        RefreshLocalization(string.Format(str, m_LocalizationFormat.ToArray()));
                    }
                }
            }
        }

        /// <summary>
        /// 刷新本地化
        /// </summary>
        /// <param name="localizationStr"></param>
        private void RefreshLocalization(string localizationStr)
        {
            //字符文本
            var txt = this.GetComponent<TMP_Text>();
            if (null != txt)
            {
                txt.text = localizationStr;
                return;
            }

            //图片刷新
            var img = this.GetComponent<Image>();
            if (null != img)
            {
                //TODO 加载图片
                return;
            }

            CommonLog.LogWarning($"物体{this.gameObject.name}上绑定了自动本地化组件,但是没有文本或图片等组件");
        }

        private void OnLocalizationChange(IEventMessage message)
        {
            this.m_IsDirty = true;
        }

        /// <summary>
        /// 指定下标添加(修改)多语言格式化文本
        /// </summary>
        /// <param name="formatStr"></param>
        /// <param name="index"></param>
        public void AddLocalizationFormat(string formatStr, int index = 0)
        {
            //添加
            for (int i = m_LocalizationFormat.Count; i <= index; i++)
            {
                m_LocalizationFormat.Add(string.Empty);
            }
            //修改
            m_LocalizationFormat[index] = formatStr;
            //脏标记
            this.m_IsDirty = true;
        }

        /// <summary>
        /// 修改多语言格式化文本
        /// </summary>
        /// <param name="formatStr"></param>
        public void AddLocalizationFormat(params string[] formatStr)
        {
            //清空所有
            m_LocalizationFormat.Clear();
            //重新添加
            for (int i = 0; i < formatStr.Length; i++)
            {
                m_LocalizationFormat.Add(formatStr[i]);
            }
            //脏标记
            this.m_IsDirty = true;
        }

        /// <summary>
        /// 清空多语言格式化文本
        /// </summary>
        public void ClearFormatStr()
        {
            m_LocalizationFormat.Clear();
            //脏标记
            this.m_IsDirty = true;
        }

        /// <summary>
        /// 添加本地化key
        /// </summary>
        /// <param name="key"></param>
        public void AddLocalizationKey(string key)
        {
            this.m_MainLocalizationKey = key;
            this.m_IsHotfixLocalization = false;
            this.m_IsDirty = true;
        }

        /// <summary>
        /// 添加本地化key
        /// </summary>
        /// <param name="key"></param>
        public void AddLocalizationKey(int key)
        {
            this.m_HotfixLocalizationKey = key;
            this.m_IsHotfixLocalization = true;
            this.m_IsDirty = true;
        }

        /// <summary>
        /// 设置本地化
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formatStr"></param>
        public void SetLocalization(string key, params string[] formatStr)
        {
            AddLocalizationKey(key);
            AddLocalizationFormat(formatStr);
        }

        /// <summary>
        /// 设置本地化
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formatStr"></param>
        public void SetLocalization(int key, params string[] formatStr)
        {
            AddLocalizationKey(key);
            AddLocalizationFormat(formatStr);
        }
    }
}
