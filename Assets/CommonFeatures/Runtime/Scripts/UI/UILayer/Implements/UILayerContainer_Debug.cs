using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// µ÷ÊÔ²ãÈÝÆ÷
    /// </summary>
    public class UILayerContainer_Debug : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Debug;

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            throw new System.NotImplementedException();
        }
    }
}
