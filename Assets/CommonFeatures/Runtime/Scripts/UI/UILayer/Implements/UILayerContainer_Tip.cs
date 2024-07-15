using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 提示层容器
    /// </summary>
    public class UILayerContainer_Tip : UILayerContainerBase
    {
        [SerializeField] private UIPanel_Tip m_TipPanelOrigin;

        public override EUILayer Layer => EUILayer.Tip;

        private UIPanel_Tip m_TipPanel;

        protected override async UniTask OnInit()
        {
            m_TipPanel = GameObject.Instantiate(m_TipPanelOrigin, this.transform);
            await m_TipPanel.Init();
            m_TipPanel.gameObject.SetActive(true);

            await base.OnInit();
        }

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            m_TipPanel.PanelScreenFit(referenceResolution);
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="tip"></param>
        public void ShowTip(string tip)
        {
            m_TipPanel.ShowTip(tip);
        }
    }
}
