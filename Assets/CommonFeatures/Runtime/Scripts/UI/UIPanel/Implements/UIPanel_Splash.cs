using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ���ɽ��棬��ʾ��˾logo���߶�����
    /// </summary>
    public class UIPanel_Splash : UIPanelBase
    {
        [SerializeField] private TMP_Text m_Text;

        protected override async UniTask OnInit()
        {
            await base.OnInit();
            
            
        }
    }
}
