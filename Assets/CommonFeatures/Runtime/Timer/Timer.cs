using CommonFeatures.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Timer
{
    /// <summary>
    /// 一个计时器
    /// </summary>
    public class Timer : IReference
    {
        /// <summary>
        /// 在计时中不断触发
        /// </summary>
        private Action _OnTiming;

        /// <summary>
        /// 一轮计时结束时触发一次
        /// </summary>
        private Action _OnTimingEnd;

        /// <summary>
        /// 循环次数(负数时一直循环)
        /// </summary>
        private int _LoopTime;

        /// <summary>
        /// 一轮计时时长
        /// </summary>
        private float _Time;

        /// <summary>
        /// 当前计时时间
        /// </summary>
        private float _CurrentTime;

        /// <summary>
        /// 当前计时器是否暂停
        /// </summary>
        private bool _IsPause;

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsComplete { get => _LoopTime == 0; }

        internal void Init(float time,int loopTime,Action onTiming = null,Action onTimingEnd = null)
        {
            _Time = Mathf.Max(time, 0);
            _CurrentTime = 0;
            _LoopTime = loopTime;
            _IsPause = false;//构造计时器时立即开始计时
            _OnTiming = onTiming;
            _OnTimingEnd = onTimingEnd;
        }

        /// <summary>
        /// 暂停计时或者继续计时
        /// </summary>
        internal void PauseOrContinueTiming()
        {
            _IsPause = !_IsPause;
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        internal void PauseTiming()
        {
            _IsPause = true;
        }

        /// <summary>
        /// 继续计时
        /// </summary>
        internal void ContinueTiming()
        {
            _IsPause = false;
        }

        /// <summary>
        /// 获取计时器当前的计时时间
        /// </summary>
        /// <returns></returns>
        internal float GetCurrentTime()
        {
            return _CurrentTime;
        }

        /// <summary>
        /// 更新计时
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

