using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ����������
    /// </summary>
    public class UILayerContainer_Base : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Base;

        /// <summary>
        /// �ȸ��µȽ���������
        /// </summary>
        [SerializeField]
        private UIPanel_Progress m_PanelProgress;

        /// <summary>
        /// ���ɽ���
        /// </summary>
        [SerializeField]
        private UIPanel_Splash m_PanelSplash;

        /// <summary>
        /// ��ǰ��ʾ����
        /// </summary>
        private EBaseLayerUIType m_CurShowUIType;

        /// <summary>
        /// ���н���
        /// </summary>
        private Dictionary<EBaseLayerUIType, UIPanelBase> m_AllPanelDic = new Dictionary<EBaseLayerUIType, UIPanelBase>();

        protected override async UniTask OnInit()
        {
            var panelSplash = GameObject.Instantiate(m_PanelSplash.gameObject, this.transform).GetComponent<UIPanel_Splash>();
            var panelProgress = GameObject.Instantiate(m_PanelProgress.gameObject, this.transform).GetComponent<UIPanel_Progress>();

            m_CurShowUIType = EBaseLayerUIType.None;

            //������ͺͽ����Ӧ���
            m_AllPanelDic.Clear();
            m_AllPanelDic.Add(EBaseLayerUIType.Splash, panelSplash);
            m_AllPanelDic.Add(EBaseLayerUIType.Progress, panelProgress);
            //�������н���
            foreach (var panel in m_AllPanelDic.Values)
            {
                await panel.Init();
                panel.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ��ʾui
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public async UniTask ShowUI(EBaseLayerUIType uiType)
        {
            //���������н�����ʾĬ�ϻ���,ͬһʱ��ֻ����ʾһ������
            if (m_CurShowUIType == uiType)
            {
                return;
            }
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                m_AllPanelDic[m_CurShowUIType].Hide();
            }
            m_CurShowUIType = uiType;
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                await m_AllPanelDic[m_CurShowUIType].Show();
            }
        }

        /// <summary>
        /// ��ȡui
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public UIPanelBase GetUI(EBaseLayerUIType uiType)
        {
            if (m_CurShowUIType == uiType)
            {
                return m_AllPanelDic[m_CurShowUIType];
            }
            return null;
        }

        /// <summary>
        /// ��ȡui
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUI<T>() where T : UIPanelBase
        {
            if (m_AllPanelDic.TryGetValue(m_CurShowUIType, out var panel) && panel is T t)
            {
                return t;
            }
            return null;
        }

        /// <summary>
        /// ����ui
        /// </summary>
        public void HideUI()
        {
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                m_AllPanelDic[m_CurShowUIType].Hide();
            }
            m_CurShowUIType = EBaseLayerUIType.None;
        }

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                m_AllPanelDic[m_CurShowUIType].PanelScreenFit(referenceResolution);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            HideUI();

            foreach (var panel in m_AllPanelDic.Values)
            {
                panel.Release();
            }

            m_AllPanelDic.Clear();
        }
    }

    /// <summary>
    /// ������UI����
    /// </summary>
    public enum EBaseLayerUIType : byte
    {
        None,

        Splash,

        Progress,
    }
}
