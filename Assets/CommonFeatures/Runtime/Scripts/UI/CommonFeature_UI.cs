using CommonFeatures;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
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
        /// 所有层级容器
        /// </summary>
        private Dictionary<EUILayer, UILayerContainerBase> m_LayerContainerDic = new Dictionary<EUILayer, UILayerContainerBase>();

        /// <summary>
        /// 所有层级容器数据
        /// </summary>
        private UILayerContainerModel m_Model;

        public override async UniTask Init()
        {
            m_Model = new UILayerContainerModel();

            m_LayerContainerDic.Clear();

            if (null != m_LayerContainers && m_LayerContainers.Length > 0)
            {
                for (int i = 0; i < m_LayerContainers.Length; i++)
                {
                    var layerContainer = m_LayerContainers[i];
                    if (null != layerContainer)
                    {
                        var containerBase = layerContainer.GetComponent<UILayerContainerBase>();
                        if (null != containerBase)
                        {
                            var layer = containerBase.Layer;
                            if (m_LayerContainerDic.ContainsKey(layer))
                            {
                                CommonLog.LogError($"层级重复,在{m_LayerContainerDic[layer].gameObject.name}和{containerBase.gameObject.name}两个物体上同时挂载了层级{layer}");
                            }
                            else
                            {
                                m_LayerContainerDic.Add(layer, containerBase);
                            }
                        }
                    }
                }
            }

            foreach (var layerContainer in m_LayerContainerDic.Values)
            {
                await layerContainer.Init();
            }

            ListenScreenSizeChange().Forget();
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private UILayerContainerBase GetLayerContainer(EUILayer layer)
        {
            if (m_LayerContainerDic.ContainsKey(layer))
            {
                return m_LayerContainerDic[layer];
            }
            return null;
        }

        /// <summary>
        /// 显示基础UI
        /// </summary>
        /// <param name="uiType"></param>
        public async UniTask ShowBaseUI(EBaseLayerUIType uiType)
        {
            m_Model.BaseLayerUIType = uiType;
            await GetLayerContainer(EUILayer.Base).ShowUI(m_Model);
        }

        /// <summary>
        /// 隐藏当前的基础UI
        /// </summary>
        public void HideBaseUI()
        {
            GetLayerContainer(EUILayer.Base).HideUI(m_Model);
        }

        /// <summary>
        /// 监听屏幕变化
        /// </summary>
        /// <returns></returns>
        private async UniTask ListenScreenSizeChange()
        {
            var preScreenRect = Rect.zero;
            while (true)
            {
                var curScreenRect = Screen.safeArea;
                if (preScreenRect != curScreenRect)
                {
                    preScreenRect = curScreenRect;

                    var referenceResolution = GetCanvasReferenceResolution();

                    //遍历所有界面容器
                    foreach (var container in m_LayerContainerDic.Values)
                    {
                        await container.LayerContainerScreenFit(referenceResolution);
                    }
                }

                await UniTask.Yield();
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
    }
}
