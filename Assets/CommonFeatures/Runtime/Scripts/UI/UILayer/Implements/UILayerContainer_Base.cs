using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 基础层容器
    /// </summary>
    public class UILayerContainer_Base : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Base;

        /// <summary>
        /// 热更新等进度条界面
        /// </summary>
        [SerializeField]
        private UIPanel_Progress m_PanelProgress;

        /// <summary>
        /// 过渡界面
        /// </summary>
        [SerializeField]
        private UIPanel_Splash m_PanelSplash;

        protected override async UniTask OnInit()
        {
            await m_PanelSplash.Init();
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
