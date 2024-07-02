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
            
        }
    }
}
