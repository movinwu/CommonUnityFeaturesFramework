using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace CommonFeatures.UIEx
{
    /// <summary>
    /// toggle group组件拓展
    /// <para>配合<see cref="ToggleEx"/>一起使用,也可配合<see cref="PageEx"/>一起使用</para>
    /// </summary>
    public class ToggleGroupEx : MonoBehaviour
    {
        [Header("所有管理的toggle")]
        [SerializeField] private ToggleEx[] m_Toggles;

        [Header("初始选中toggle下标")]
        [SerializeField] private int m_InitToggleIndex = 0;

        [Header("是否支持不选中所有toggle")]
        [SerializeField] private bool m_AllowSwitchOff = false;

        [Header("选中切换时是否刷新所有toggle")]
        [SerializeField] private bool m_RefreshAllOnSwitch = false;

        /// <summary>
        /// 指定初始下标
        /// </summary>
        public int InitToggleIndex
        {
            set
            {
                m_InitToggleIndex = Mathf.Clamp(value, 0, (null == m_Toggles || m_Toggles.Length == 0) ? 0 : m_Toggles.Length - 1);
            }
        }

        /// <summary>
        /// 选中回调
        /// </summary>
        private Func<int, UniTask> m_OnSelected;

        /// <summary>
        /// 未选中回调
        /// </summary>
        private Func<int, UniTask> m_OnUnselected;

        /// <summary>
        /// 当前选中
        /// </summary>
        private int m_CurSelectedIndex = -1;

        private bool m_OnSelectedAnimation = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="onSelected"></param>
        /// <param name="onUnselected"></param>
        public async UniTask Init(Func<int, UniTask> onSelected, Func<int, UniTask> onUnselected)
        {
            if (null == m_Toggles || m_Toggles.Length == 0)
            {
                CommonLog.LogWarning("toggle group下没有指定toggle, 无法初始化!!!");
                return;
            }

            m_OnSelected = onSelected;
            m_OnUnselected = onUnselected;

            m_OnSelectedAnimation = false;
            m_CurSelectedIndex = -1;
            if (!m_AllowSwitchOff)
            {
                InitToggleIndex = m_InitToggleIndex;
            }

            //注册按钮
            for (int i = 0; i < m_Toggles.Length; i++)
            {
                //待优化闭包写法
                var index = i;
                m_Toggles[i].ToggleBtn.onClick.RemoveAllListeners();
                m_Toggles[i].ToggleBtn.onClick.AddListener(() => OnSelected(index).Forget());
            }

            await OnSelected(this.m_InitToggleIndex);
        }

        private async UniTask OnSelected(int index)
        {
            //不能重复选择
            if (m_OnSelectedAnimation)
            {
                return;
            }

            //是否重复选中
            if (index == m_CurSelectedIndex)
            {
                if (m_AllowSwitchOff)
                {
                    index = -1;
                }
                else
                {
                    return;
                }
            }

            m_OnSelectedAnimation = true;

            if (m_RefreshAllOnSwitch)
            {
                var tasks = new UniTask[m_Toggles.Length + 2];
                if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_Toggles[m_CurSelectedIndex].Page)
                {
                    tasks[m_Toggles.Length] = m_Toggles[m_CurSelectedIndex].Page.OnHide();
                }
                else
                {
                    tasks[m_Toggles.Length] = UniTask.CompletedTask;
                }

                m_CurSelectedIndex = index;
                for (int i = 0; i < m_Toggles.Length; i++)
                {
                    if (m_CurSelectedIndex == i)
                    {
                        if (null == m_OnSelected)
                        {
                            tasks[i] = UniTask.CompletedTask;
                        }
                        else
                        {
                            tasks[i] = m_OnSelected(i);
                        }
                    }
                    else
                    {
                        if (null == m_OnUnselected)
                        {
                            tasks[i] = UniTask.CompletedTask;
                        }
                        else
                        {
                            tasks[i] = m_OnUnselected(i);
                        }
                    }
                }

                if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_Toggles[m_CurSelectedIndex].Page)
                {
                    tasks[m_Toggles.Length + 1] = m_Toggles[m_CurSelectedIndex].Page.OnShow();
                }
                else
                {
                    tasks[m_Toggles.Length + 1] = UniTask.CompletedTask;
                }

                await UniTask.WhenAll(tasks);
            }
            else
            {
                var tasks = new UniTask[4];

                if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_Toggles[m_CurSelectedIndex].Page)
                {
                    tasks[2] = m_Toggles[m_CurSelectedIndex].Page.OnHide();
                }
                else
                {
                    tasks[2] = UniTask.CompletedTask;
                }

                if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_OnUnselected)
                {
                    tasks[0] = m_OnUnselected(m_CurSelectedIndex);
                }
                else
                {
                    tasks[0] = UniTask.CompletedTask;
                }
                m_CurSelectedIndex = index;
                if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_OnSelected)
                {
                    tasks[1] = m_OnSelected(m_CurSelectedIndex);
                }
                else
                {
                    tasks[1] = UniTask.CompletedTask;
                }

                if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_Toggles[m_CurSelectedIndex].Page)
                {
                    tasks[3] = m_Toggles[m_CurSelectedIndex].Page.OnShow();
                }
                else
                {
                    tasks[3] = UniTask.CompletedTask;
                }

                await UniTask.WhenAll(tasks);
            }

            m_OnSelectedAnimation = false;
        }

        public async UniTask Release()
        {
            m_OnSelected = null;
            m_OnUnselected = null;
            if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_Toggles[m_CurSelectedIndex].Page)
            {
                await m_Toggles[m_CurSelectedIndex].Page.OnHide();
            }
            m_CurSelectedIndex = -1;
        }
    }
}
