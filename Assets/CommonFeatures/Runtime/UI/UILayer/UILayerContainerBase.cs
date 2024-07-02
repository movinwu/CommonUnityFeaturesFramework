using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// UI�㼶��������
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class UILayerContainerBase : MonoBehaviour
    {
        /// <summary>
        /// ��Ӧ�㼶
        /// </summary>
        public abstract EUILayer Layer { get; }

        /// <summary>
        /// ��Ӧ����
        /// </summary>
        protected Canvas m_Canvas;

        public void Init()
        {
            m_Canvas = this.GetComponent<Canvas>();
            //��ʼ�������㼶
            m_Canvas.overrideSorting = true;
            m_Canvas.sortingOrder = (int)Layer;

            OnInit();
        }

        protected abstract void OnInit();

        /// <summary>
        /// ��ʾUI����
        /// </summary>
        /// <param name="model"></param>
        public abstract void ShowUI(UILayerContainerModel model);

        /// <summary>
        /// �ر�UI����
        /// </summary>
        /// <param name="model"></param>
        public abstract void HideUI(UILayerContainerModel model);
    }
}
