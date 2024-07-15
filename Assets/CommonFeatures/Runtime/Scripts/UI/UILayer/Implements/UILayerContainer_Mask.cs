using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ÕÚÕÖ²ãÈÝÆ÷
    /// </summary>
    public class UILayerContainer_Mask : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Mask;

        [SerializeField] private UIPanel_Mask m_MaskPanelOrigin;

        private UIPanel_Mask m_MaskPanel;

        protected override async UniTask OnInit()
        {
            m_MaskPanel = GameObject.Instantiate(m_MaskPanelOrigin, this.transform);
            await m_MaskPanel.Init();

            await base.OnInit();
        }

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            m_MaskPanel.PanelScreenFit(referenceResolution);
        }

        public UniTask ShowMask()
        {
            m_MaskPanel.gameObject.SetActive(true);
            return m_MaskPanel.Show();
        }
    }
}
