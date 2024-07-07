using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// 进度条界面,基础层,显示热更下载等逻辑进度
    /// </summary>
    public class UIPanel_Progress : UIPanelBase
    {
        [SerializeField] private Image m_Slider;
        [SerializeField] private TMP_Text m_Text;

        private CancellationTokenSource m_Token;

        protected override UniTask OnInit()
        {
            HideProgress();

            return base.OnInit();
        }

        /// <summary>
        /// 显示进度条
        /// </summary>
        public void ShowProgress(System.Func<string> getTxt, System.Func<float> getProgress)
        {
            if (null != m_Token)
            {
                m_Token.Cancel();
                m_Token.Dispose();
                m_Token = null;
            }

            m_Token = new CancellationTokenSource();
            UniTaskAsyncEnumerable.EveryUpdate().ForEachAsync(x =>
            {
                m_Slider.fillAmount = Mathf.Clamp01(getProgress?.Invoke() ?? 0);
                m_Text.text = getTxt?.Invoke() ?? string.Empty;
            }, m_Token.Token);
        }

        /// <summary>
        /// 隐藏进度条
        /// </summary>
        public void HideProgress()
        {
            if (null != m_Token)
            {
                m_Token.Cancel();
                m_Token.Dispose();
                m_Token = null;
            }
        }
    }
}
