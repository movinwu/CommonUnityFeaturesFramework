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

        /// <summary>
        /// 显示UI函数
        /// </summary>
        /// <param name="model"></param>
        public virtual UniTask ShowUI(UILayerContainerModel model)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 关闭UI函数
        /// </summary>
        /// <param name="model"></param>
        public virtual void HideUI(UILayerContainerModel model)
        {

        }

        /// <summary>
        /// 屏幕适配
        /// </summary>
        /// <param name="referenceResolution">预设尺寸</param>
        /// <returns></returns>
        public abstract UniTask LayerContainerScreenFit(Vector2 referenceResolution);
    }
}
