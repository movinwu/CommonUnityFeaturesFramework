using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ���ز�����
    /// </summary>
    public class UILayerContainer_Loading : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Loading;

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            
        }
    }
}
