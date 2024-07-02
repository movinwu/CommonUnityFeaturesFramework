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

        public void Init()
        {
            m_Canvas = this.GetComponent<Canvas>();
            //初始化画布层级
            m_Canvas.overrideSorting = true;
            m_Canvas.sortingOrder = (int)Layer;

            OnInit();
        }

        protected abstract void OnInit();

        /// <summary>
        /// 显示UI函数
        /// </summary>
        /// <param name="model"></param>
        public abstract void ShowUI(UILayerContainerModel model);

        /// <summary>
        /// 关闭UI函数
        /// </summary>
        /// <param name="model"></param>
        public abstract void HideUI(UILayerContainerModel model);
    }
}
