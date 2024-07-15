using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// 单个通用功能
    /// <para>不要在子类中继承monobehaviour的声明周期函数,若有需要,在这个类中声明虚函数,由子函数继承,并由CFM统一调用,以便控制调用顺序</para>
    /// </summary>
    public class CommonFeature : MonoBehaviour
    {
        /// <summary>
        /// 初始化功能
        /// <para>替换生命周期函数Awake</para>
        /// </summary>
        public virtual UniTask Init()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 释放函数
        /// <para>替换生命周期函数OnDestroy</para>
        /// </summary>
        public virtual void Release()
        {

        }
    }
}
