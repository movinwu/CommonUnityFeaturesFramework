using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// º”‘ÿ≤„»›∆˜
    /// </summary>
    public class UILayerContainer_Loading : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Loading;

        public override UniTask LayerContainerScreenFit(Vector2 referenceResolution)
        {
            return UniTask.CompletedTask;
        }
    }
}
