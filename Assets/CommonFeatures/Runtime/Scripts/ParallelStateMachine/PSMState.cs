using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
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
        /// ״̬��״̬
        /// </summary>
        public EPSMState State { get; set; } = EPSMState.PrepareToRun;

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="psm"></param>
        internal async UniTask Init(PSM<T> psm)
        {
            this.PSM = psm;

            State = EPSMState.PrepareToRun;

            await this.OnInit();
        }

        /// <summary>
        /// ����״̬
        /// </summary>
        public async UniTask Enter()
        {
            //�Ѿ���ʼִ�л�ִ����ϵ�״̬�����ظ�ִ��
            if (this.State != EPSMState.PrepareToRun)
            {
                return;
            }

            //��Ҫ�ȴ�ִ��ǰ״̬ȫ��ִ����ϲ��ܼ���ִ��
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

            //��ʽ��ʼ��ǰ״ִ̬��
            this.State = EPSMState.Running;
            await this.OnEnter();
        }

        /// <summary>
        /// �뿪״̬
        /// </summary>
        public async UniTask Leave()
        {
            //���������е�״̬�޷��뿪״̬
            if (this.State != EPSMState.Running)
            {
                return;
            }

            //��ʽ�뿪״̬
            this.State = EPSMState.Ran;
            await this.OnLeave();

            //������������״̬
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
        /// ָ����ǰ״̬��ִ��ǰ״̬
        /// <para>��ǰ״ִ̬�е�ǰ��������ִ��ǰ״ִ̬�����</para>
        /// </summary>
        /// <returns></returns>
        protected virtual List<PSMState<T>> GetPreStates()
        {
            return new List<PSMState<T>>(0);
        }

        /// <summary>
        /// ָ����ǰ״̬��ִ�к�״̬
        /// <para>��ǰ״ִ̬����Ϻ����������״ִ̬��(����״̬����У���Ƿ�Ҫִ��)</para>
        /// </summary>
        /// <returns></returns>
        protected virtual List<PSMState<T>> GetNextStates()
        {
            return new List<PSMState<T>>(0);
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        protected virtual async UniTask OnInit()
        {

        }

        /// <summary>
        /// ������״̬��
        /// </summary>
        protected virtual async UniTask OnEnter()
        {

        }

        /// <summary>
        /// ���뿪״̬��
        /// </summary>
        protected virtual async UniTask OnLeave()
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
