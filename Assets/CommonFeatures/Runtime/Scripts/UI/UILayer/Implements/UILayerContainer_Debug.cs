using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 调试层容器
    /// </summary>
    public class UILayerContainer_Debug : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Debug;

        [SerializeField] private UIPanel_Debug m_DebugPanelOrigin;

        [Header("连续点击时间")]
        [SerializeField] private float m_ContinueClickTime = 0.5f;

        [Header("连续点击次数")]
        [SerializeField] private int m_ContinueClickCount = 3;

        private UIPanel_Debug m_DebugPanel;

        /// <summary>
        /// 点击时间监听
        /// </summary>
        private Queue<float> m_ClickListener = new Queue<float>();

        /// <summary>
        /// 是否界面正在显示
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

            //监听tab键
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
            //监听F1键
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (m_IsShowing)
                {
                    m_DebugPanel.AddNotice();
                }
            }
            //监听连续点击
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
