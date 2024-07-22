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

        [Header("ѡ���л�ʱ�Ƿ�ˢ������toggle")]
        [SerializeField] private bool m_RefreshAllOnSwitch = false;

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
        /// ѡ�лص�
        /// </summary>
        private Func<int, UniTask> m_OnSelected;

        /// <summary>
        /// δѡ�лص�
        /// </summary>
        private Func<int, UniTask> m_OnUnselected;

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
        public async UniTask Init(Func<int, UniTask> onSelected, Func<int, UniTask> onUnselected)
        {
            if (null == m_Toggles || m_Toggles.Length == 0)
            {
                CommonLog.LogWarning("toggle group��û��ָ��toggle, �޷���ʼ��!!!");
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
