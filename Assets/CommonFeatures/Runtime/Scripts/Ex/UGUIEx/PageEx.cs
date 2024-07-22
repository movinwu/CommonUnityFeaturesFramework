using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UIEx
{
    /// <summary>
    /// ҳǩ���
    /// <para>���<see cref="ToggleEx"/>��<see cref="PageEx"/>һ��ʹ��</para>
    /// <para>ʵ��ʹ��ʱʹ������̳в�ʵ�ֶ�Ӧ�ĺ���</para>
    /// </summary>
    public class PageEx : MonoBehaviour
    {
        public virtual UniTask OnShow()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnHide()
        {
            return UniTask.CompletedTask;
        }
    }
}
