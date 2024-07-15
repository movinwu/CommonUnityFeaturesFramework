using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ����������
    /// </summary>
    public class UILayerContainer_Guide : UILayerContainerBase
    {
        /// <summary>
        /// �����޸��˲㼶��������Ʒ��������
        /// </summary>
        private Dictionary<UIPanelBase, List<Canvas>> m_AllGuideObjectBelongs = new Dictionary<UIPanelBase, List<Canvas>>();

        /// <summary>
        /// ������������
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
        /// �����������
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="guideLayer"></param>
        public void AddObjectGuide(GameObject obj, EUILayer guideLayer)
        {
            if (guideLayer < EUILayer.GuideObjectBelowGuide3 || guideLayer > EUILayer.GuideObjectUpGuide6)
            {
                CommonLog.LogWarning($"��������������㼶�Ƽ��� {EUILayer.GuideObjectBelowGuide3} - {EUILayer.GuideObjectUpGuide6} ֮��, ��ǰ������������㼶Ϊ: {guideLayer}");
            }

            //�ҵ�����
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
                CommonLog.LogError($"û���ҵ��������� {obj.name} �����Ľ������");
                return;
            }

            if (!m_AllGuideObjectBelongs.ContainsKey(panel))
            {
                m_AllGuideObjectBelongs.Add(panel, new List<Canvas>());
            }

            //�Ƿ��Ѿ������������ϵ�canvas���
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

            //�Ƿ�����canvas���
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
        /// �Ƴ���������
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObjectGuide(GameObject obj)
        {
            //�ҵ�����
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
                CommonLog.LogError($"û���ҵ��������� {obj.name} �����Ľ������");
                return;
            }

            if (!m_AllGuideObjectBelongs.ContainsKey(panel))
            {
                CommonLog.LogWarning($"���� {obj.name} ������������,�޷��Ƴ�");
                return;
            }

            //�Ƿ��Ѿ������������ϵ�canvas���
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

            CommonLog.LogWarning($"���� {obj.name} ������������,�޷��Ƴ�");
        }

        /// <summary>
        /// �Ƴ�������������������
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
        /// �����������
        /// </summary>
        /// <param name="panel"></param>
        public async UniTask AddPanelGuide(UIPanelBase panel)
        {
            if (m_AllGuidePanel.Contains(panel))
            {
                Debug.LogWarning($"���� {panel.gameObject.name} �����Ϊ��������,��Ҫ�ظ����");
                return;
            }

            panel.transform.SetParent(this.transform);
            await panel.Init();
            m_AllGuidePanel.Add(panel);
        }

        /// <summary>
        /// �Ƴ���������
        /// </summary>
        /// <param name="panel"></param>
        public void RemovePanelGuide(UIPanelBase panel)
        {
            if (!m_AllGuidePanel.Contains(panel))
            {
                Debug.LogWarning($"���� {panel.gameObject.name} ������������,�޷��Ƴ�");
                return;
            }

            panel.Release();
            m_AllGuidePanel.Remove(panel);
            
        }
    }
}
