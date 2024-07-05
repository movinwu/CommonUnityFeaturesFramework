using CommonFeatures;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 通用功能-UI
    /// </summary>
    public class CommonFeature_UI : CommonFeature
    {
        [SerializeField] private UILayerContainerBase[] m_LayerContainers;

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
    }
}
