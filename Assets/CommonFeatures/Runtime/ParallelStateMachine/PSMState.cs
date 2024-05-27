using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.PSM
{
    /// <summary>
    /// ����״̬��״̬
    /// </summary>
    public class PSMState<T>
    {
        /// <summary>
        /// ����״̬��
        /// </summary>
        protected PSM<T> PSM { get; private set; }

        protected T Owner { get => PSM.Owner; }

        /// <summary>
        /// ״̬����ǰ������״̬
        /// </summary>
        public List<PSMState<T>> PrePSMState { get; private set; }

        /// <summary>
        /// �����е�����ǰ״̬
        /// </summary>
        public List<PSMState<T>> RunningPrePSMState { get; private set; }

        /// <summary>
        /// ״̬���к����е�����״̬
        /// </summary>
        public List<PSMState<T>> NextPSMState { get; private set; }

        /// <summary>
        /// ״̬��״̬
        /// </summary>
        public EPSMState State { get; set; } = EPSMState.NotInited;

        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// ��ʼ��
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
        /// �������ǰ״̬
        /// </summary>
        /// <param name="prePSM"></param>
        public void AddPrePSM(PSMState<T> prePSM)
        {
            if (this.PrePSMState.Contains(prePSM))
            {
                CommonLog.LogError($"�ظ��������ǰ״̬{prePSM}");
                return;
            }
            if (prePSM.NextPSMState.Contains(this))
            {
                CommonLog.LogError($"�ظ�������к�״̬{prePSM}");
                return;
            }
            this.PrePSMState.Add(prePSM);
            prePSM.NextPSMState.Add(this);
        }

        /// <summary>
        /// ������к�״̬
        /// </summary>
        /// <param name="nextPSM"></param>
        public void AddNextPSM(PSMState<T> nextPSM)
        {
            if (this.PrePSMState.Contains(nextPSM))
            {
                CommonLog.LogError($"�ظ�������к�״̬{nextPSM}");
                return;
            }
            if (nextPSM.PrePSMState.Contains(this))
            {
                CommonLog.LogError($"�ظ��������ǰ״̬{nextPSM}");
                return;
            }
            this.NextPSMState.Add(nextPSM);
            nextPSM.PrePSMState.Add(this);
        }

        /// <summary>
        /// ��ʼ״̬
        /// </summary>
        public void StartStateRunning(PSMState<T> preState)
        {
            //û�п�ʼ״̬��������ʼ״̬����
            if (this.State == EPSMState.NotRunning)
            {
                this.RunningPrePSMState.Clear();
                this.PrePSMState.ForEach(x => this.RunningPrePSMState.Add(x));
                this.State = EPSMState.PrepareToRun;
            }

            //�Ѿ���ʼ״̬����
            if (this.State == EPSMState.PrepareToRun)
            {
                this.RunningPrePSMState.Remove(preState);

                //���Կ�ʼ������
                if (this.RunningPrePSMState.Count == 0)
                {
                    this.OnEnter();
                    this.State = EPSMState.Running;
                }
            }
        }

        /// <summary>
        /// ����״̬����
        /// </summary>
        public void StopStateRunning()
        {
            if (this.State == EPSMState.Running)
            {
                this.OnLeave();
                this.State = EPSMState.NotRunning;

                //֪ͨ����״̬����ʼ����
                for (int i = 0; i < NextPSMState.Count; i++)
                {
                    NextPSMState[i].StartStateRunning(this);
                }
            }
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public virtual void OnInit()
        {

        }

        /// <summary>
        /// ������״̬��
        /// </summary>
        protected virtual void OnEnter()
        {

        }

        /// <summary>
        /// ÿ֡��ѯ����
        /// </summary>
        public virtual void OnTick()
        {

        }

        /// <summary>
        /// ���뿪״̬��
        /// </summary>
        protected virtual void OnLeave()
        {

        }

        /// <summary>
        /// ������״̬��
        /// </summary>
        public virtual void OnDestroy()
        {

        }
    }
}
