using CommonFeatures;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ͨ�ù���-UI
    /// </summary>
    public class CommonFeature_UI : CommonFeature
    {
        [SerializeField] private UILayerContainerBase[] m_LayerContainers;

        /// <summary>
        /// ���в㼶����
        /// </summary>
        private Dictionary<EUILayer, UILayerContainerBase> m_LayerContainerDic = new Dictionary<EUILayer, UILayerContainerBase>();

        /// <summary>
        /// ���в㼶��������
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
                                CommonLog.LogError($"�㼶�ظ�,��{m_LayerContainerDic[layer].gameObject.name}��{containerBase.gameObject.name}����������ͬʱ�����˲㼶{layer}");
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
        /// ��ȡ����
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
        /// ��ʾ����UI
        /// </summary>
        /// <param name="uiType"></param>
        public async UniTask ShowBaseUI(EBaseLayerUIType uiType)
        {
            m_Model.BaseLayerUIType = uiType;
            await GetLayerContainer(EUILayer.Base).ShowUI(m_Model);
        }

        /// <summary>
        /// ���ص�ǰ�Ļ���UI
        /// </summary>
        public void HideBaseUI()
        {
            GetLayerContainer(EUILayer.Base).HideUI(m_Model);
        }

        /// <summary>
        /// ������Ļ�仯
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
                    //�������н�������
                    foreach (var container in m_LayerContainerDic.Values)
                    {
                        await container.LayerContainerScreenFit();
                    }
                }

                await UniTask.Yield();
            }
        }
    }
}
