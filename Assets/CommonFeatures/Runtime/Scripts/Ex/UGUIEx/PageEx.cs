using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UIEx
{
    /// <summary>
    /// 页签组件
    /// <para>配合<see cref="ToggleEx"/>和<see cref="PageEx"/>一起使用</para>
    /// <para>实际使用时使用子类继承并实现对应的函数</para>
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
