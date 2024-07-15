using CommonFeatures;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 通用功能-UI
    /// </summary>
    public class CommonFeature_UI : CommonFeature
    {
        [SerializeField] private UILayerContainerBase[] m_LayerContainers;

        [SerializeField] private CanvasScaler m_CanvasScaler;

        [SerializeField] private Vector2 m_CanvasReferenceResolution;

        /// <summary>
        /// 所有层级容器和类型对应关系
        /// </summary>
        private Dictionary<Type, UILayerContainerBase> m_LayerContainerDic = new Dictionary<Type, UILayerContainerBase>();

        /// <summary>
        /// 之前的屏幕缓存
        /// </summary>
        private Rect m_PreScreenRect;

        public override async UniTask Init()
        {
            m_LayerContainerDic.Clear();

            if (null != m_LayerContainers && m_LayerContainers.Length > 0)
            {
                for (int i = 0; i < m_LayerContainers.Length; i++)
                {
                    var layerContainer = m_LayerContainers[i];
                    var type = layerContainer.GetType();
                    if (m_LayerContainerDic.ContainsKey(type))
                    {
                        CommonLog.LogError($"类型重复,在{m_LayerContainerDic[type].gameObject.name}和{layerContainer.gameObject.name}两个层级容器类型相同");
                    }
                    else
                    {
                        m_LayerContainerDic.Add(type, layerContainer);
                    }
                }
            }

            foreach (var layerContainer in m_LayerContainerDic.Values)
            {
                await layerContainer.Init();
            }

            m_PreScreenRect = Rect.zero;
        }

        /// <summary>
        /// 获取UI容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetLayerContainer<T>() where T : UILayerContainerBase
        {
            var layerType = typeof(T);
            if (m_LayerContainerDic.ContainsKey(layerType))
            {
                return m_LayerContainerDic[layerType] as T;
            }
            return null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            //监听屏幕变化
            var curScreenRect = Screen.safeArea;
            if (m_PreScreenRect != curScreenRect)
            {
                m_PreScreenRect = curScreenRect;

                var referenceResolution = GetCanvasReferenceResolution();

                //遍历所有界面容器
                foreach (var container in m_LayerContainerDic.Values)
                {
                    container.LayerContainerScreenFit(referenceResolution);
                }
            }
        }

        /// <summary>
        /// 获取屏幕定义分辨率
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCanvasReferenceResolution()
        {
            //采用Scale With Screen Size方案
            if (m_CanvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                m_CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            }

            //设置屏幕分辨率
            if (!m_CanvasScaler.referenceResolution.Equals(m_CanvasReferenceResolution))
            {
                m_CanvasScaler.referenceResolution = m_CanvasReferenceResolution;
            }

            //固定适配权重
            if (m_CanvasScaler.matchWidthOrHeight != 0)
            {
                m_CanvasScaler.matchWidthOrHeight = 0;
            }

            return m_CanvasScaler.referenceResolution;
        }

        public override void Release()
        {
            base.Release();

            foreach (var container in m_LayerContainerDic.Values)
            {
                container.Release();
            }

            m_LayerContainerDic.Clear();
        }
    }
}
