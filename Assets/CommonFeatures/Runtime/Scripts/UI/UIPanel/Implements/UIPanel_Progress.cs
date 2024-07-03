using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 进度条界面,基础层,显示热更下载等逻辑进度
    /// </summary>
    public class UIPanel_Progress : UIPanelBase
    {
        [SerializeField] private Image m_Slider;
        [SerializeField] private TMP_Text m_Text;
    }
}
