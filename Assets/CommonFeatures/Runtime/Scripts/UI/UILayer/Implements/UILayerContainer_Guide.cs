using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 引导层容器
    /// </summary>
    public class UILayerContainer_Guide : UILayerContainerBase
    {
        /// <summary>
        /// 所有修改了层级的引导物品所属界面
        /// </summary>
        private Dictionary<UIPanelBase, List<Canvas>> m_AllGuideObjectBelongs = new Dictionary<UIPanelBase, List<Canvas>>();

        /// <summary>
        /// 所有引导界面
        /// </summary>
        private List<UIPanelBase> m_AllGuidePanel = new List<UIPanelBase>();

        public override EUILayer Layer => EUILayer.Guide;

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            for (int i = 0; i < m_AllGuidePanel.Count; i++)
            {
                m_AllGuidePanel[i].PanelScreenFit(referenceResolution);
            }
        }

        /// <summary>
        /// 添加引导物体
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="guideLayer"></param>
        public void AddObjectGuide(GameObject obj, EUILayer guideLayer)
        {
            if (guideLayer < EUILayer.GuideObjectBelowGuide3 || guideLayer > EUILayer.GuideObjectUpGuide6)
            {
                CommonLog.LogWarning($"引导物体的引导层级推荐在 {EUILayer.GuideObjectBelowGuide3} - {EUILayer.GuideObjectUpGuide6} 之间, 当前给定引导物体层级为: {guideLayer}");
            }

            //找到界面
            UIPanelBase panel = null;
            var parent = obj.transform;
            while (null != parent)
            {
                panel = parent.GetComponent<UIPanelBase>();
                if (null != panel)
                {
                    break;
                }
                parent = parent.parent;
            }

            if (null == panel)
            {
                CommonLog.LogError($"没有找到引导物体 {obj.name} 所属的界面基类");
                return;
            }

            if (!m_AllGuideObjectBelongs.ContainsKey(panel))
            {
                m_AllGuideObjectBelongs.Add(panel, new List<Canvas>());
            }

            //是否已经保存了物体上的canvas组件
            var canvasList = m_AllGuideObjectBelongs[panel];
            for (int i = 0; i < canvasList.Count; i++)
            {
                if (canvasList[i].gameObject.Equals(obj))
                {
                    canvasList[i].enabled = true;
                    canvasList[i].overrideSorting = true;
                    canvasList[i].sortingOrder = (int)guideLayer;
                    return;
                }
            }

            //是否已有canvas组件
            var canvas = obj.GetComponent<Canvas>();
            if (null != canvas)
            {
                canvasList.Add(canvas);
                canvas.enabled = true;
                canvas.overrideSorting = true;
                canvas.sortingOrder = (int)guideLayer;
            }
            else
            {
                canvas = obj.AddComponent<Canvas>();
                canvasList.Add(canvas);
                canvas.enabled = true;
                canvas.overrideSorting = true;
                canvas.sortingOrder = (int)guideLayer;
            }
        }

        /// <summary>
        /// 移除引导物体
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObjectGuide(GameObject obj)
        {
            //找到界面
            UIPanelBase panel = null;
            var parent = obj.transform.parent;
            while (null != parent)
            {
                panel = parent.GetComponent<UIPanelBase>();
                if (null != panel)
                {
                    break;
                }
                parent = parent.parent;
            }

            if (null == panel)
            {
                CommonLog.LogError($"没有找到引导物体 {obj.name} 所属的界面基类");
                return;
            }

            if (!m_AllGuideObjectBelongs.ContainsKey(panel))
            {
                CommonLog.LogWarning($"物体 {obj.name} 不是引导物体,无法移除");
                return;
            }

            //是否已经保存了物体上的canvas组件
            var canvasList = m_AllGuideObjectBelongs[panel];
            for (int i = 0; i < canvasList.Count; i++)
            {
                if (canvasList[i].gameObject.Equals(obj))
                {
                    canvasList[i].enabled = false;
                    canvasList.RemoveAt(i);
                    return;
                }
            }

            CommonLog.LogWarning($"物体 {obj.name} 不是引导物体,无法移除");
        }

        /// <summary>
        /// 移除界面下所有引导物体
        /// </summary>
        /// <param name="panel"></param>
        public void RemoveAllObjectGuide(UIPanelBase panel)
        {
            if (!m_AllGuideObjectBelongs.ContainsKey(panel))
            {
                return;
            }

            var canvasList = m_AllGuideObjectBelongs[panel];
            for (int i = 0; i < canvasList.Count; i++)
            {
                canvasList[i].enabled = false;
            }
            canvasList.Clear();
            m_AllGuideObjectBelongs.Remove(panel);
        }

        /// <summary>
        /// 添加引导界面
        /// </summary>
        /// <param name="panel"></param>
        public async UniTask AddPanelGuide(UIPanelBase panel)
        {
            if (m_AllGuidePanel.Contains(panel))
            {
                Debug.LogWarning($"界面 {panel.gameObject.name} 已添加为引导界面,不要重复添加");
                return;
            }

            panel.transform.SetParent(this.transform);
            await panel.Init();
            m_AllGuidePanel.Add(panel);
        }

        /// <summary>
        /// 移除引导界面
        /// </summary>
        /// <param name="panel"></param>
        public void RemovePanelGuide(UIPanelBase panel)
        {
            if (!m_AllGuidePanel.Contains(panel))
            {
                Debug.LogWarning($"界面 {panel.gameObject.name} 不是引导界面,无法移除");
                return;
            }

            panel.Release();
            m_AllGuidePanel.Remove(panel);
            
        }
    }
}
