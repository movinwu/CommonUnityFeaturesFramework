using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.FSM
{
    /// <summary>
    /// ����״̬��״̬
    /// </summary>
    public class FSMState<T>
    {
        /// <summary>
        /// ����״̬��
        /// </summary>
        protected FSM<T> FSM { get; private set; }

        protected T Owner { get => FSM.Owner; }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="fsm"></param>
        internal void Init(FSM<T> fsm)
        {
            this.FSM = fsm;
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public async virtual UniTask OnInit()
        {

        }

        /// <summary>
        /// ������״̬��
        /// </summary>
        public async virtual UniTask OnEnter()
        {

        }

        /// <summary>
        /// ���뿪״̬��
        /// </summary>
        public async virtual UniTask OnLeave()
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
