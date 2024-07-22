using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    public class UIPanel_Mask : UIPanelBase
    {
        [SerializeField] private Image m_MaskBg;

        protected override UniTask OnInit()
        {
            m_MaskBg.gameObject.SetActive(false);

            return base.OnInit();
        }

        protected override UniTask OnShow()
        {
            m_MaskBg.gameObject.SetActive(true);

            return base.OnShow();
        }

        protected override void OnHide()
        {
            m_MaskBg.gameObject.SetActive(false);

            base.OnHide();
        }
    }
}
