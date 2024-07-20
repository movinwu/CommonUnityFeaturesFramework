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
    /// �Զ������ػ����
    /// </summary>
    public class AutoLocalization : MonoBehaviour
    {
        [Header("������key")]
        [SerializeField] private string m_LocalizationKey;

        /// <summary>
        /// ���ػ������Ը�ʽ��
        /// <para>�����˸�ʽ���ı���,��ˢ�¶�����ʱ���Զ�����string.Format�������и�ʽ��</para>
        /// </summary>
        private List<string> m_LocalizationFormat = new List<string>();

        /// <summary>
        /// ����
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
                var str = CFM.Localization.GetLocalizationStr(m_LocalizationKey);
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

        /// <summary>
        /// ˢ�±��ػ�
        /// </summary>
        /// <param name="localizationStr"></param>
        private void RefreshLocalization(string localizationStr)
        {
            //�ַ��ı�
            var txt = this.GetComponent<TMP_Text>();
            if (null != txt)
            {
                txt.text = localizationStr;
                return;
            }

            //ͼƬˢ��
            var img = this.GetComponent<Image>();
            if (null != img)
            {
                //TODO ����ͼƬ
                return;
            }

            CommonLog.LogWarning($"����{this.gameObject.name}�ϰ����Զ����ػ����,����û���ı���ͼƬ�����");
        }

        private void OnLocalizationChange(IEventMessage message)
        {
            this.m_IsDirty = true;
        }

        /// <summary>
        /// ָ���±����(�޸�)�����Ը�ʽ���ı�
        /// </summary>
        /// <param name="formatStr"></param>
        /// <param name="index"></param>
        public void AddLocalizationFormat(string formatStr, int index = 0)
        {
            //���
            for (int i = m_LocalizationFormat.Count; i <= index; i++)
            {
                m_LocalizationFormat.Add(string.Empty);
            }
            //�޸�
            m_LocalizationFormat[index] = formatStr;
            //����
            this.m_IsDirty = true;
        }

        /// <summary>
        /// ��ն����Ը�ʽ���ı�
        /// </summary>
        public void ClearFormatStr()
        {
            m_LocalizationFormat.Clear();
            //����
            this.m_IsDirty = true;
        }

        /// <summary>
        /// ��ӱ��ػ�key
        /// </summary>
        /// <param name="key"></param>
        public void SetLocalizationKey(string key)
        {
            this.m_LocalizationKey = key;
            this.m_IsDirty = true;
        }

        /// <summary>
        /// �޸Ķ����Ը�ʽ���ı�
        /// </summary>
        /// <param name="formatStr"></param>
        public void SetLocalizationFormat(params string[] formatStr)
        {
            //�������
            m_LocalizationFormat.Clear();
            //�������
            for (int i = 0; i < formatStr.Length; i++)
            {
                m_LocalizationFormat.Add(formatStr[i]);
            }
            //����
            this.m_IsDirty = true;
        }

        /// <summary>
        /// ���ñ��ػ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formatStr"></param>
        public void SetLocalization(string key, params string[] formatStr)
        {
            SetLocalizationKey(key);
            SetLocalizationFormat(formatStr);
        }
    }
}
