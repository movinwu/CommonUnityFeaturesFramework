using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ���ֲ�����
    /// </summary>
    public class UILayerContainer_Mask : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Mask;

        public override UniTask LayerContainerScreenFit(Vector2 referenceResolution)
        {
            return UniTask.CompletedTask;
        }
    }
}
