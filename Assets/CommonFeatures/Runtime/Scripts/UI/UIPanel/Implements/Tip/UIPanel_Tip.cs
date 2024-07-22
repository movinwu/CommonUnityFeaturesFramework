using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 提示界面
    /// </summary>
    public class UIPanel_Tip : UIPanelBase
    {
        [SerializeField] private TMP_Text m_Text;

        [Header("文字动画参数")]
        [SerializeField] private float m_UpTime = 0.5f;
        [SerializeField] private float m_UpLength = 100f;

        private Queue<string> m_TextQueue = new Queue<string>();

        private Vector2 m_OriginPos;

        private bool m_Showing;

        protected override UniTask OnInit()
        {
            m_Text.gameObject.SetActive(false);
            m_TextQueue.Clear();
            m_OriginPos = m_Text.GetComponent<RectTransform>().anchoredPosition;
            m_Showing = false;

            return base.OnInit();
        }

        protected override UniTask OnShow()
        {
            return base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="tip"></param>
        public void ShowTip(string tip)
        {
            m_TextQueue.Enqueue(tip);

            StartTipShow().Forget();
        }

        /// <summary>
        /// 开始提示显示
        /// </summary>
        /// <returns></returns>
        private async UniTask StartTipShow()
        {
            if (m_Showing || m_TextQueue.Count == 0)
            {
                return;
            }

            m_Showing = true;
            m_Text.gameObject.SetActive(true);
            m_Text.text = m_TextQueue.Dequeue();
            m_Text.GetComponent<RectTransform>().anchoredPosition = m_OriginPos;
            await m_Text.transform.DOMoveY(m_UpLength, m_UpTime).AwaitForComplete();
            m_Showing = false;
            await StartTipShow();
        }
    }
}
