using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace CommonFeatures.UIEx
{
    /// <summary>
    /// toggle group�����չ
    /// <para>���<see cref="ToggleEx"/>һ��ʹ��,Ҳ�����<see cref="PageEx"/>һ��ʹ��</para>
    /// </summary>
    public class ToggleGroupEx : MonoBehaviour
    {
        [Header("���й����toggle")]
        [SerializeField] private ToggleEx[] m_Toggles;

        [Header("��ʼѡ��toggle�±�")]
        [SerializeField] private int m_InitToggleIndex = 0;

        [Header("�Ƿ�֧�ֲ�ѡ������toggle")]
        [SerializeField] private bool m_AllowSwitchOff = false;

        /// <summary>
        /// ָ����ʼ�±�
        /// </summary>
        public int InitToggleIndex
        {
            set
            {
                m_InitToggleIndex = Mathf.Clamp(value, 0, (null == m_Toggles || m_Toggles.Length == 0) ? 0 : m_Toggles.Length - 1);
            }
        }

        /// <summary>
        /// ѡ��ί��
        /// </summary>
        private (Func<int, UniTask> onSelected, Func<int, UniTask> onUnselected) m_SelectedFunc;

        /// <summary>
        /// ת��ί��
        /// <para>����: toggle���, �±�</para>
        /// </summary>
        private (Func<ToggleEx, int, UniTask> selectTransition, Func<ToggleEx, int, UniTask> unselectTransition) m_TransitionFunc;

        /// <summary>
        /// ��ǰѡ��
        /// </summary>
        private int m_CurSelectedIndex = -1;

        private bool m_OnSelectedAnimation = false;

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="onSelected"></param>
        /// <param name="onUnselected"></param>
        /// <param name="selectTransition">ѡ�а�ť�仯</param>
        /// <param name="unselectTransition">δѡ�а�ť�仯</param>
        public async UniTask Init(
            Func<int, UniTask> onSelected, 
            Func<int, UniTask> onUnselected, 
            Func<ToggleEx, int, UniTask> selectTransition, 
            Func<ToggleEx, int, UniTask> unselectTransition)
        {
            if (null == m_Toggles || m_Toggles.Length == 0)
            {
                CommonLog.LogWarning("toggle group��û��ָ��toggle, �޷���ʼ��!!!");
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

            //ע�ᰴť
            for (int i = 0; i < m_Toggles.Length; i++)
            {
                //���Ż��հ�д��
                var index = i;
                m_Toggles[i].ToggleBtn.onClick.RemoveAllListeners();
                m_Toggles[i].ToggleBtn.onClick.AddListener(() => OnSelected(index).Forget());
            }

            await OnSelected(this.m_InitToggleIndex);
        }

        private async UniTask OnSelected(int index)
        {
            //�����ظ�ѡ��
            if (m_OnSelectedAnimation)
            {
                return;
            }

            //�Ƿ��ظ�ѡ��
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
