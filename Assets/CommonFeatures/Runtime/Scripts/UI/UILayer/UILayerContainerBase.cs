using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// UI层级容器基类
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class UILayerContainerBase : MonoBehaviour
    {
        /// <summary>
        /// 对应层级
        /// </summary>
        public abstract EUILayer Layer { get; }

        /// <summary>
        /// 对应画布
        /// </summary>
        protected Canvas m_Canvas;

        public async UniTask Init()
        {
            m_Canvas = this.GetComponent<Canvas>();
            //初始化画布层级
            m_Canvas.overrideSorting = true;
            m_Canvas.sortingOrder = (int)Layer;

            await OnInit();
        }

        protected virtual UniTask OnInit()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnUpdate()
        {
            return UniTask.CompletedTask;
        }

        public void Release()
        {
            OnRelease();
        }

        protected virtual void OnRelease()
        {

        }

        /// <summary>
        /// 屏幕适配
        /// </summary>
        /// <param name="referenceResolution">预设尺寸</param>
        /// <returns></returns>
        public abstract void LayerContainerScreenFit(Vector2 referenceResolution);
    }
}
