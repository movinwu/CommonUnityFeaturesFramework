using Cysharp.Threading.Tasks;
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

        public async UniTask Init()
        {
            m_Canvas = this.GetComponent<Canvas>();
            //��ʼ�������㼶
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
        /// ��Ļ����
        /// </summary>
        /// <param name="referenceResolution">Ԥ��ߴ�</param>
        /// <returns></returns>
        public abstract void LayerContainerScreenFit(Vector2 referenceResolution);
    }
}
