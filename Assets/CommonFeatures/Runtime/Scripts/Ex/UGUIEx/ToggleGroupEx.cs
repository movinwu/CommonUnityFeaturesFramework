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
        /// 选择委托
        /// </summary>
        private (Func<int, UniTask> onSelected, Func<int, UniTask> onUnselected) m_SelectedFunc;

        /// <summary>
        /// 转化委托
        /// <para>参数: toggle组件, 下标</para>
        /// </summary>
        private (Func<ToggleEx, int, UniTask> selectTransition, Func<ToggleEx, int, UniTask> unselectTransition) m_TransitionFunc;

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
        /// <param name="selectTransition">选中按钮变化</param>
        /// <param name="unselectTransition">未选中按钮变化</param>
        public async UniTask Init(
            Func<int, UniTask> onSelected, 
            Func<int, UniTask> onUnselected, 
            Func<ToggleEx, int, UniTask> selectTransition, 
            Func<ToggleEx, int, UniTask> unselectTransition)
        {
            if (null == m_Toggles || m_Toggles.Length == 0)
            {
                CommonLog.LogWarning("toggle group下没有指定toggle, 无法初始化!!!");
                return;
            }

            m_SelectedFunc = (onSelected, onUnselected);
            m_TransitionFunc = (selectTransition, unselectTransition);

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

            var tasks = new UniTask[6];

            if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_Toggles[m_CurSelectedIndex].Page)
            {
                tasks[2] = m_Toggles[m_CurSelectedIndex].Page.OnHide();
            }
            else
            {
                tasks[2] = UniTask.CompletedTask;
            }

            if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_TransitionFunc.unselectTransition)
            {
                tasks[4] = m_TransitionFunc.unselectTransition(m_Toggles[m_CurSelectedIndex], m_CurSelectedIndex);
            }
            else
            {
                tasks[4] = UniTask.CompletedTask;
            }

            if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_SelectedFunc.onUnselected)
            {
                tasks[0] = m_SelectedFunc.onUnselected(m_CurSelectedIndex);
            }
            else
            {
                tasks[0] = UniTask.CompletedTask;
            }
            m_CurSelectedIndex = index;
            if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_SelectedFunc.onSelected)
            {
                tasks[1] = m_SelectedFunc.onSelected(m_CurSelectedIndex);
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

            if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_TransitionFunc.selectTransition)
            {
                tasks[5] = m_TransitionFunc.selectTransition(m_Toggles[m_CurSelectedIndex], m_CurSelectedIndex);
            }
            else
            {
                tasks[5] = UniTask.CompletedTask;
            }

            await UniTask.WhenAll(tasks);

            m_OnSelectedAnimation = false;
        }

        public async UniTask Release()
        {
            m_SelectedFunc.onSelected = null;
            m_SelectedFunc.onUnselected = null;
            m_TransitionFunc.selectTransition = null;
            m_TransitionFunc.unselectTransition = null;
            if (m_CurSelectedIndex >= 0 && m_CurSelectedIndex < m_Toggles.Length && null != m_Toggles[m_CurSelectedIndex].Page)
            {
                await m_Toggles[m_CurSelectedIndex].Page.OnHide();
            }
            m_CurSelectedIndex = -1;
        }
    }
}
