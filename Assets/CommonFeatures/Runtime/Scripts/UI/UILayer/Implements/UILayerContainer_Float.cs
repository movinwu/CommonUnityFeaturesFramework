using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ¸¡¶¯²ãÈÝÆ÷
    /// </summary>
    public class UILayerContainer_Float : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Float;

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            throw new System.NotImplementedException();
        }
    }
}
