using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// Òýµ¼²ãÈÝÆ÷
    /// </summary>
    public class UILayerContainer_Guide : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Guide;

        public override UniTask LayerContainerScreenFit(Vector2 referenceResolution)
        {
            return UniTask.CompletedTask;
        }
    }
}
