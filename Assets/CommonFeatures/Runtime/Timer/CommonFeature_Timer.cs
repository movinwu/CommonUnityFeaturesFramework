using CommonFeatures.Pool;
using CommonFeatures.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.Timer
{
    /// <summary>
    /// 计时器管理器
    /// </summary>
    public class CommonFeature_Timer : CommonFeature
    {
        #region 字段和属性
        /// <summary>
        /// 存储所有计时器的字典
        /// </summary>
        private Dictionary<ulong, Timer> _timerDic = new Dictionary<ulong, Timer>();
        #endregion

        /// <summary>
        /// 添加指定名称的计时器，不会开始计时
        /// </summary>
        /// <param name="seconds">计时器一轮计时的毫秒值</param>
        /// <param name="loopTime">计时器是否循环计时</param>
        /// <param name="onTiming">计时中不断执行</param>
        /// <param name="onTimingEnd">每轮计时结束时执行一次</param>
        /// <returns></returns>
        public ulong AddTimer(float seconds,int loopTime,Action onTiming = null,Action onTimingEnd = null)
        {
            var id = UniqueIDUtility.GenerateUniqueID();

            var timer = ReferencePool.Acquire<Timer>();
            timer.Init(seconds, loopTime, onTiming, onTimingEnd);
            _timerDic.Add(id, timer);
            return id;
        }

        /// <summary>
        /// 暂停或者继续指定计时器
        /// </summary>
        /// <param name="id">计时器id</param>
        /// <returns></returns>
        public bool PauseOrContinueTimer(ulong id)
        {
            if (_timerDic.ContainsKey(id))
            {
                _timerDic[id].PauseOrContinueTiming();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 暂停指定计时器
        /// </summary>
        /// <param name="id">计时器id</param>
        /// <returns></returns>
        public bool PauseTimer(ulong id)
        {
            if (_timerDic.ContainsKey(id))
            {
                _timerDic[id].PauseTiming();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 继续指定计时器
        /// </summary>
        /// <param name="id">计时器id</param>
        /// <returns></returns>
        public bool ContinueTimer(ulong id)
        {
            if (_timerDic.ContainsKey(id))
            {
                _timerDic[id].ContinueTiming();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 暂停所有计时器
        /// </summary>
        public void PauseAllTimer()
        {
            foreach(var pairs in _timerDic)
            {
                pairs.Value.PauseTiming();
            }
        }

        /// <summary>
        /// 继续所有计时器
        /// </summary>
        public void ContinueAllTimer()
        {
            foreach(var pairs in _timerDic)
            {
                pairs.Value.ContinueTiming();
            }
        }

        /// <summary>
        /// 获取指定计时器的当前计时毫秒值
        /// 返回值-1代表计时器不存在
        /// </summary>
        /// <param name="id">计时器名称</param>
        /// <returns></returns>
        public float GetCurrentTime(ulong id)
        {
            if (_timerDic.ContainsKey(id))
            {
                return _timerDic[id].GetCurrentTime();
            }
            return -1;
        }

        /// <summary>
        /// 销毁指定timer
        /// </summary>
        /// <param name="id"></param>
        public void DeleteTimer(ulong id)
        {
            if (_timerDic.TryGetValue(id, out var timer))
            {
                _timerDic.Remove(id);
                ReferencePool.Back(timer);
            }
        }

        /// <summary>
        /// 每帧调用所有计时器和销毁已完成的计时器
        /// </summary>
        public override void Tick()
        {
            var deltaTime = Time.deltaTime;
            if(_timerDic.Count > 0)
            {
                foreach (var pairs in _timerDic)
                {
                    pairs.Value.UpdateTiming(deltaTime);
                }
            }
            var ids = _timerDic.Keys.ToArray();
            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                if (_timerDic[id].IsComplete)
                {
                    DeleteTimer(id);
                }
            }
        }
    }
}

