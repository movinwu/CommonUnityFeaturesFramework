using CommonFeatures.Log;
using CommonFeatures.UIEx;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    public class UIPanel_Debug_ConsolePage : PageEx
    {
        [Header("��ʷ��¼����")]
        [SerializeField] private int m_HistoryRecordLength = 1000;

        /// <summary>
        /// ������ʷ��¼
        /// </summary>
        private List<string> m_History = new List<string>();

        /// <summary>
        /// ��ʼ��¼�±�
        /// </summary>
        private int m_StartRecordIndex = 0;

        
    }
}
