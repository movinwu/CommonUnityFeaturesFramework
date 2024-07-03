using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ����������
    /// </summary>
    public class UILayerContainer_Base : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Base;

        /// <summary>
        /// �ȸ��µȽ���������
        /// </summary>
        [SerializeField]
        private UIPanel_Progress m_PanelProgress;

        protected override async UniTask OnInit()
        {
            await m_PanelProgress.Init();
        }

        public override void ShowUI(UILayerContainerModel model)
        {
            
        }

        public override void HideUI(UILayerContainerModel model)
        {
            
        }
    }
}