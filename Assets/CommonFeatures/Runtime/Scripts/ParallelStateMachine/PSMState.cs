using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// 并行状态机状态
    /// </summary>
    public class PSMState<T>
    {
        /// <summary>
        /// 所属状态机
        /// </summary>
        protected PSM<T> PSM { get; private set; }

        protected T Owner { get => PSM.Owner; }

        /// <summary>
        /// 状态机状态
        /// </summary>
        public EPSMState State { get; set; } = EPSMState.PrepareToRun;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="psm"></param>
        internal async UniTask Init(PSM<T> psm)
        {
            this.PSM = psm;

            State = EPSMState.PrepareToRun;

            await this.OnInit();
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        public async UniTask Enter()
        {
            //已经开始执行或执行完毕的状态机不重复执行
            if (this.State != EPSMState.PrepareToRun)
            {
                return;
            }

            //需要等待执行前状态全部执行完毕才能继续执行
            var preStates = this.GetPreStates();
            if (null != preStates)
            {
                for (int i = 0; i < preStates.Count; i++)
                {
                    if (preStates[i].State != EPSMState.Ran)
                    {
                        return;
                    }
                }
            }

            //正式开始当前状态执行
            this.State = EPSMState.Running;
            await this.OnEnter();
        }

        /// <summary>
        /// 离开状态
        /// </summary>
        public async UniTask Leave()
        {
            //不在运行中的状态无法离开状态
            if (this.State != EPSMState.Running)
            {
                return;
            }

            //正式离开状态
            this.State = EPSMState.Ran;
            await this.OnLeave();

            //尝试启动后续状态
            var nextStates = this.GetNextStates();
            if(null != nextStates)
            {
                for (int i = 0; i < nextStates.Count; i++)
                {
                    await nextStates[i].Enter();
                }
            }
        }

        /// <summary>
        /// 指定当前状态机执行前状态
        /// <para>当前状态执行的前提是所有执行前状态执行完毕</para>
        /// </summary>
        /// <returns></returns>
        protected virtual List<PSMState<T>> GetPreStates()
        {
            return new List<PSMState<T>>(0);
        }

        /// <summary>
        /// 指定当前状态机执行后状态
        /// <para>当前状态执行完毕后会启动后续状态执行(后续状态自行校验是否要执行)</para>
        /// </summary>
        /// <returns></returns>
        protected virtual List<PSMState<T>> GetNextStates()
        {
            return new List<PSMState<T>>(0);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual async UniTask OnInit()
        {

        }

        /// <summary>
        /// 当进入状态机
        /// </summary>
        protected virtual async UniTask OnEnter()
        {

        }

        /// <summary>
        /// 当离开状态机
        /// </summary>
        protected virtual async UniTask OnLeave()
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
