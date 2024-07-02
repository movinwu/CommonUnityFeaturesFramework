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

        public override void HideUI(UILayerContainerModel model)
        {
            throw new System.NotImplementedException();
        }

        public override void ShowUI(UILayerContainerModel model)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInit()
        {
            //TODO 初始化登录\热更加载等界面
        }
    }
}
