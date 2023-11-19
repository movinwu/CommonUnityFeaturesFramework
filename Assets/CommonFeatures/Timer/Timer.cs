using CommonFeatures.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Timer
{
    /// <summary>
    /// һ����ʱ��
    /// </summary>
    public class Timer : IReference
    {
        /// <summary>
        /// �ڼ�ʱ�в��ϴ���
        /// </summary>
        private Action _OnTiming;

        /// <summary>
        /// һ�ּ�ʱ����ʱ����һ��
        /// </summary>
        private Action _OnTimingEnd;

        /// <summary>
        /// ѭ������(����ʱһֱѭ��)
        /// </summary>
        private int _LoopTime;

        /// <summary>
        /// һ�ּ�ʱʱ��
        /// </summary>
        private float _Time;

        /// <summary>
        /// ��ǰ��ʱʱ��
        /// </summary>
        private float _CurrentTime;

        /// <summary>
        /// ��ǰ��ʱ���Ƿ���ͣ
        /// </summary>
        private bool _IsPause;

        /// <summary>
        /// �Ƿ������
        /// </summary>
        public bool IsComplete { get => _LoopTime == 0; }

        internal void Init(float time,int loopTime,Action onTiming = null,Action onTimingEnd = null)
        {
            _Time = Mathf.Max(time, 0);
            _CurrentTime = 0;
            _LoopTime = loopTime;
            _IsPause = false;//�����ʱ��ʱ������ʼ��ʱ
            _OnTiming = onTiming;
            _OnTimingEnd = onTimingEnd;
        }

        /// <summary>
        /// ��ͣ��ʱ���߼�����ʱ
        /// </summary>
        internal void PauseOrContinueTiming()
        {
            _IsPause = !_IsPause;
        }

        /// <summary>
        /// ��ͣ��ʱ
        /// </summary>
        internal void PauseTiming()
        {
            _IsPause = true;
        }

        /// <summary>
        /// ������ʱ
        /// </summary>
        internal void ContinueTiming()
        {
            _IsPause = false;
        }

        /// <summary>
        /// ��ȡ��ʱ����ǰ�ļ�ʱʱ��
        /// </summary>
        /// <returns></returns>
        internal float GetCurrentTime()
        {
            return _CurrentTime;
        }

        /// <summary>
        /// ���¼�ʱ
        /// </summary>
        internal void UpdateTiming(float deltaTime)
        {
            if (!_IsPause)
            {
                if(_CurrentTime < _Time)
                {
                    _CurrentTime += deltaTime;
                    _OnTiming?.Invoke();
                }
                else
                {
                    _CurrentTime -= _Time;
                    _LoopTime--;
                    _OnTimingEnd?.Invoke();
                }
            }
        }

        public void Reset()
        {
            _OnTiming = null;
            _OnTimingEnd = null;
        }
    }
}

