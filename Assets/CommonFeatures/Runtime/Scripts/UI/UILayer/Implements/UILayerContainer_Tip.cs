using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// Ã· æ≤„»›∆˜
    /// </summary>
    public class UILayerContainer_Tip : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Tip;

        public override void HideUI(UILayerContainerModel model)
        {
            throw new System.NotImplementedException();
        }

        public override void ShowUI(UILayerContainerModel model)
        {
            throw new System.NotImplementedException();
        }

        protected override async UniTask OnInit()
        {
            
        }
    }
}
