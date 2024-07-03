using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ����������,������,��ʾ�ȸ����ص��߼�����
    /// </summary>
    public class UIPanel_Progress : UIPanelBase
    {
        [SerializeField] private Image m_Slider;
        [SerializeField] private TMP_Text m_Text;
    }
}
