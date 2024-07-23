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
        [Header("历史记录长度")]
        [SerializeField] private int m_HistoryRecordLength = 1000;

        /// <summary>
        /// 所有历史记录
        /// </summary>
        private List<string> m_History = new List<string>();

        /// <summary>
        /// 开始记录下标
        /// </summary>
        private int m_StartRecordIndex = 0;

        
    }
}
