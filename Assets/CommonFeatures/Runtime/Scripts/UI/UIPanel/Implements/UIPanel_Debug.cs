using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// Debug����
    /// </summary>
    public class UIPanel_Debug : UIPanelBase
    {
        [SerializeField] private TMP_Text m_ContentTxt;
        [SerializeField] private Transform m_Content;
        [SerializeField] private TMP_InputField m_InputField;
        [SerializeField] private Button m_ConfirmBtn;
        [SerializeField] private Button m_ClearBtn;

        /// <summary>
        /// ��ʷ��¼
        /// </summary>
        private List<string> m_HistoryList = new List<string>();

        /// <summary>
        /// ������ʷ��¼Ԫ��
        /// </summary>
        private GameObject[] m_AllHistoryItem;

        /// <summary>
        /// ��������
        /// </summary>
        private Dictionary<string, Func<string[], bool>> m_AllCommand = new Dictionary<string, Func<string[], bool>>();

        /// <summary>
        /// ��ʾ
        /// </summary>
        private string m_NoticeStr;

        protected override UniTask OnInit()
        {
            m_HistoryList.Clear();
            m_ConfirmBtn.onClick.RemoveAllListeners();
            m_ConfirmBtn.onClick.AddListener(() =>
            {
                var input = m_InputField.text;
                if (string.IsNullOrEmpty(input))
                {
                    AddHistory(CFM.Localization.GetMainLocalization("debug_input_empty"));
                    return;
                }
                var strs = input.Split(' ');
                var command = strs[0];
                if (m_AllCommand.ContainsKey(command))
                {
                    var excuteFunc = m_AllCommand[command];
                    if (excuteFunc(strs))
                    {
                        return;
                    }
                }
                AddHistory(CFM.Localization.GetMainLocalization("debug_input_error"));
            });
            m_ClearBtn.onClick.RemoveAllListeners();
            m_ClearBtn.onClick.AddListener(ClearHistory);

            return base.OnInit();
        }

        protected override void OnRelease()
        {
            if (null != m_AllHistoryItem)
            {
                CFM.GameObjectPool.Back(m_ContentTxt.gameObject, m_Content, m_AllHistoryItem);
            }

            base.OnRelease();
        }

        public void AddNotice()
        {
            if (m_NoticeStr.EndsWith('\n'))
            {
                m_NoticeStr = m_NoticeStr.Substring(0, m_NoticeStr.Length - 1);
            }
            AddHistory(m_NoticeStr);
        }

        private void AddHistory(string newStr)
        {
            m_HistoryList.Add(newStr);
            RefreshContent();
        }

        private void ClearHistory()
        {
            m_HistoryList.Clear();
            RefreshContent();
        }

        private void RefreshContent()
        {
            m_AllHistoryItem = CFM.GameObjectPool.Acquire(m_ContentTxt.gameObject, m_Content, count: m_HistoryList.Count, backAll: true);
            for (int i = 0; i < m_AllHistoryItem.Length; i++)
            {
                m_AllHistoryItem[i].SetActive(true);
                var txt = m_AllHistoryItem[i].GetComponent<TMP_Text>();
                txt.text = m_HistoryList[i];
            }
        }

        /// <summary>
        /// ע������
        /// </summary>
        /// <param name="command"></param>
        /// <param name="excuteFunc"></param>
        /// <param name="notice"></param>
        public void RegistCommand(string command, Func<string[], bool> excuteFunc, string notice)
        {
            if (m_AllCommand.ContainsKey(command))
            {
                CommonLog.LogWarning($"�ظ�ע������ {command}");
                return;
            }

            if (string.IsNullOrEmpty(notice) || string.IsNullOrEmpty(command))
            {
                CommonLog.LogWarning($"ע������ʧ��,�������������ʾΪ��");
                return;
            }

            if (null == excuteFunc)
            {
                CommonLog.LogWarning($"ע������ʧ��,����ִ��Ϊ��");
                return;
            }

            m_AllCommand.Add(command, excuteFunc);
            m_NoticeStr = $"{m_NoticeStr}{notice}\n";
        }
    }
}
