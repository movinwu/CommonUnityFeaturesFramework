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

        public override UniTask LayerContainerScreenFit()
        {
            return UniTask.CompletedTask;
        }
    }
}
