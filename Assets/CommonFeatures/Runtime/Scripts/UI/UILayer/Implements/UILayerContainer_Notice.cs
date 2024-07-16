using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 提示层容器
    /// </summary>
    public class UILayerContainer_Notice : UILayerContainerBase
    {
        [SerializeField] private UIPanel_Notice m_NoticePanelOrigin;

        public override EUILayer Layer => EUILayer.Notice;

        private UIPanel_Notice m_NoticePanel;

        protected override async UniTask OnInit()
        {
            m_NoticePanel = GameObject.Instantiate(m_NoticePanelOrigin, this.transform);
            await m_NoticePanel.Init();
            m_NoticePanel.gameObject.SetActive(true);

            await base.OnInit();
        }

        public override async UniTask OnUpdate()
        {
            await m_NoticePanel.OnUpdate();
            await base.OnUpdate();
        }

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            m_NoticePanel.PanelScreenFit(referenceResolution);
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="notice"></param>
        public void ShowNotice(string notice)
        {
            m_NoticePanel.ShowNotice(notice);
        }
    }
}
