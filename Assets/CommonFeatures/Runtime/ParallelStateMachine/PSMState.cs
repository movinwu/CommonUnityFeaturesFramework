using CommonFeatures.Log;
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
        /// 状态运行前的所有状态
        /// </summary>
        public List<PSMState<T>> PrePSMState { get; private set; }

        /// <summary>
        /// 运行中的运行前状态
        /// </summary>
        public List<PSMState<T>> RunningPrePSMState { get; private set; }

        /// <summary>
        /// 状态运行后运行的所有状态
        /// </summary>
        public List<PSMState<T>> NextPSMState { get; private set; }

        /// <summary>
        /// 状态机状态
        /// </summary>
        public EPSMState State { get; set; } = EPSMState.NotInited;

        /// <summary>
        /// 是否完成运行
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="psm"></param>
        internal void Init(PSM<T> psm)
        {
            if (null == this.PrePSMState)
            {
                this.PrePSMState = new List<PSMState<T>>();
            }
            else
            {
                this.PrePSMState.Clear();
            }

            if (null == this.RunningPrePSMState)
            {
                this.RunningPrePSMState = new List<PSMState<T>>();
            }
            else
            {
                this.RunningPrePSMState.Clear();
            }

            if (null == this.NextPSMState)
            {
                this.NextPSMState = new List<PSMState<T>>();
            }
            else
            {
                this.NextPSMState.Clear();
            }

            this.PSM = psm;

            State = EPSMState.NotRunning;
        }

        /// <summary>
        /// 添加运行前状态
        /// </summary>
        /// <param name="prePSM"></param>
        public void AddPrePSM(PSMState<T> prePSM)
        {
            if (this.PrePSMState.Contains(prePSM))
            {
                CommonLog.LogError($"重复添加运行前状态{prePSM}");
                return;
            }
            if (prePSM.NextPSMState.Contains(this))
            {
                CommonLog.LogError($"重复添加运行后状态{prePSM}");
                return;
            }
            this.PrePSMState.Add(prePSM);
            prePSM.NextPSMState.Add(this);
        }

        /// <summary>
        /// 添加运行后状态
        /// </summary>
        /// <param name="nextPSM"></param>
        public void AddNextPSM(PSMState<T> nextPSM)
        {
            if (this.PrePSMState.Contains(nextPSM))
            {
                CommonLog.LogError($"重复添加运行后状态{nextPSM}");
                return;
            }
            if (nextPSM.PrePSMState.Contains(this))
            {
                CommonLog.LogError($"重复添加运行前状态{nextPSM}");
                return;
            }
            this.NextPSMState.Add(nextPSM);
            nextPSM.PrePSMState.Add(this);
        }

        /// <summary>
        /// 开始状态
        /// </summary>
        public void StartStateRunning(PSMState<T> preState)
        {
            //没有开始状态监听，开始状态监听
            if (this.State == EPSMState.NotRunning)
            {
                this.RunningPrePSMState.Clear();
                this.PrePSMState.ForEach(x => this.RunningPrePSMState.Add(x));
                this.State = EPSMState.PrepareToRun;
            }

            //已经开始状态监听
            if (this.State == EPSMState.PrepareToRun)
            {
                this.RunningPrePSMState.Remove(preState);

                //可以开始运行了
                if (this.RunningPrePSMState.Count == 0)
                {
                    this.OnEnter();
                    this.State = EPSMState.Running;
                }
            }
        }

        /// <summary>
        /// 结束状态运行
        /// </summary>
        public void StopStateRunning()
        {
            if (this.State == EPSMState.Running)
            {
                this.OnLeave();
                this.State = EPSMState.NotRunning;

                //通知后续状态，开始运行
                for (int i = 0; i < NextPSMState.Count; i++)
                {
                    NextPSMState[i].StartStateRunning(this);
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {

        }

        /// <summary>
        /// 当进入状态机
        /// </summary>
        protected virtual void OnEnter()
        {

        }

        /// <summary>
        /// 每帧轮询函数
        /// </summary>
        public virtual void OnTick()
        {

        }

        /// <summary>
        /// 当离开状态机
        /// </summary>
        protected virtual void OnLeave()
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
