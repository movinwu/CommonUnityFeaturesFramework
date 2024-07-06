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

            await panelSplash.Init();
            await panelProgress.Init();

            m_CurShowUIType = EBaseLayerUIType.None;

            //������ͺͽ����Ӧ���
            m_AllPanelDic.Clear();
            m_AllPanelDic.Add(EBaseLayerUIType.Splash, panelSplash);
            m_AllPanelDic.Add(EBaseLayerUIType.Progress, panelProgress);
            //�������н���
            foreach (var panel in m_AllPanelDic.Values)
            {
                panel.gameObject.SetActive(false);
            }
        }

        public override async UniTask ShowUI(UILayerContainerModel model)
        {
            //���������н�����ʾĬ�ϻ���,ͬһʱ��ֻ����ʾһ������
            if (m_CurShowUIType == model.BaseLayerUIType)
            {
                return;
            }
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                m_AllPanelDic[m_CurShowUIType].Hide();
            }
            m_CurShowUIType = model.BaseLayerUIType;
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                await m_AllPanelDic[m_CurShowUIType].Show();
            }
        }

        public override void HideUI(UILayerContainerModel model)
        {
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                m_AllPanelDic[m_CurShowUIType].Hide();
            }
            m_CurShowUIType = EBaseLayerUIType.None;
        }

        public override async UniTask LayerContainerScreenFit(Vector2 referenceResolution)
        {
            if (m_AllPanelDic.ContainsKey(m_CurShowUIType))
            {
                await m_AllPanelDic[m_CurShowUIType].PanelScreenFit(referenceResolution);
            }
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
