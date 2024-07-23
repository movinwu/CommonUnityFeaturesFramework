using CommonFeatures.Log;
using CommonFeatures.UIEx;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// Debug界面
    /// </summary>
    public class UIPanel_Debug : UIPanelBase
    {
        [SerializeField] private ToggleGroupEx m_ToggleGroup;

        [SerializeField] private UIPanel_Debug_CommandPage m_CommandPage;

        protected override UniTask OnShow()
        {
            return m_ToggleGroup.Init(
                onSelected: index =>
                {
                    CommonLog.LogError($"选中{index}");
                    return UniTask.CompletedTask;
                },
                onUnselected: index =>
                {
                    CommonLog.LogError($"取消选中{index}");
                    return UniTask.CompletedTask;
                },
                selectTransition: (toggle, index) =>
                {
                    toggle.GetComponent<Image>().color = Color.red;
                    return UniTask.CompletedTask;
                }, 
                unselectTransition: (toggle, index) =>
                {
                    toggle.GetComponent<Image>().color = Color.white;
                    return UniTask.CompletedTask;
                });
        }

        public void AddNotice()
        {
            m_CommandPage.AddNotice();
        }

        public void RegistCommand(string command, Func<string[], bool> excuteFunc, string notice)
        {
            m_CommandPage.RegistCommand(command, excuteFunc, notice);
        }
    }
}
