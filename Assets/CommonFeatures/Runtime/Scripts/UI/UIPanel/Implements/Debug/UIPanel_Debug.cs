using CommonFeatures.Log;
using CommonFeatures.UIEx;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CommonFeatures.UI
{
    /// <summary>
    /// DebugΩÁ√Ê
    /// </summary>
    public class UIPanel_Debug : UIPanelBase
    {
        [SerializeField] private ToggleGroupEx m_ToggleGroup;

        [SerializeField] private UIPanel_Debug_CommandPage m_CommandPage;

        [SerializeField] private Button m_CloseButton;

        protected override UniTask OnShow()
        {
            m_CloseButton.onClick.RemoveAllListeners();
            m_CloseButton.onClick.AddListener(() =>
            {
                CFM.UI.GetLayerContainer<UILayerContainer_Debug>().HideDebugPanel();
            });

            return m_ToggleGroup.Init(
                onSelected: index =>
                {
                    return UniTask.CompletedTask;
                },
                onUnselected: index =>
                {
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
