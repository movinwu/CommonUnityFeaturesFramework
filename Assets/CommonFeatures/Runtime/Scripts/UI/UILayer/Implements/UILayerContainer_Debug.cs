using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ���Բ�����
    /// </summary>
    public class UILayerContainer_Debug : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Debug;

        [SerializeField] private UIPanel_Debug m_DebugPanelOrigin;

        [Header("�������ʱ��")]
        [SerializeField] private float m_ContinueClickTime = 0.5f;

        [Header("�����������")]
        [SerializeField] private int m_ContinueClickCount = 3;

        private UIPanel_Debug m_DebugPanel;

        /// <summary>
        /// ���ʱ�����
        /// </summary>
        private Queue<float> m_ClickListener = new Queue<float>();

        /// <summary>
        /// �Ƿ����������ʾ
        /// </summary>
        private bool m_IsShowing;

        protected override async UniTask OnInit()
        {
            m_DebugPanel = GameObject.Instantiate(m_DebugPanelOrigin, this.transform);
            await m_DebugPanel.Init();
            m_IsShowing = false;

            await base.OnInit();
        }

        public override async UniTask OnUpdate()
        {
            await m_DebugPanel.OnUpdate();

            //����tab��
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (m_IsShowing)
                {
                    m_IsShowing = false;
                    m_DebugPanel.Hide();
                }
                else
                {
                    m_IsShowing = true;
                    await m_DebugPanel.Show();
                }
            }
            //����F1��
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (m_IsShowing)
                {
                    m_DebugPanel.AddNotice();
                }
            }
            //�����������
            var time = Time.unscaledTime;
            if (Input.GetMouseButtonDown(0))
            {
                m_ClickListener.Enqueue(time);
            }
            while (m_ClickListener.Count > 0)
            {
                var peekTime = m_ClickListener.Peek();
                if (time - peekTime >= m_ContinueClickTime)
                {
                    m_ClickListener.Dequeue();
                }
                else
                {
                    break;
                }
            }
            if (m_ClickListener.Count >= m_ContinueClickCount)
            {
                m_ClickListener.Clear();
                if (m_IsShowing)
                {
                    m_IsShowing = false;
                    m_DebugPanel.Hide();
                }
                else
                {
                    m_IsShowing = true;
                    await m_DebugPanel.Show();
                }
            }

            await base.OnUpdate();
        }

        public override void LayerContainerScreenFit(Vector2 referenceResolution)
        {
            m_DebugPanel.PanelScreenFit(referenceResolution);
        }
    }
}
