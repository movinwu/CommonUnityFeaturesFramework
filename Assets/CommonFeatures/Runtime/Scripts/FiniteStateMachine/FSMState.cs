using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.FSM
{
    /// <summary>
    /// 有限状态机状态
    /// </summary>
    public class FSMState<T>
    {
        /// <summary>
        /// 所属状态机
        /// </summary>
        protected FSM<T> FSM { get; private set; }

        protected T Owner { get => FSM.Owner; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fsm"></param>
        internal void Init(FSM<T> fsm)
        {
            this.FSM = fsm;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual UniTask OnInit()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 当进入状态机
        /// </summary>
        public virtual UniTask OnEnter()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 当离开状态机
        /// </summary>
        public virtual UniTask OnLeave()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 当销毁状态机
        /// </summary>
        public virtual void OnDestroy()
        {

        }
    }
}
