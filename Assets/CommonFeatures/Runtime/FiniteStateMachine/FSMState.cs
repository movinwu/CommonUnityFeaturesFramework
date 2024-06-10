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
        public async virtual UniTask OnInit()
        {

        }

        /// <summary>
        /// 当进入状态机
        /// </summary>
        public async virtual UniTask OnEnter()
        {

        }

        /// <summary>
        /// 当离开状态机
        /// </summary>
        public async virtual UniTask OnLeave()
        {

        }

        /// <summary>
        /// 当销毁状态机
        /// </summary>
        public virtual void OnDestroy()
        {

        }
    }
}
