using CommonFeatures.Pool;
using CommonFeatures.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.Timer
{
    /// <summary>
    /// ��ʱ��������
    /// </summary>
    public class CommonFeature_Timer : CommonFeature
    {
        #region �ֶκ�����
        /// <summary>
        /// �洢���м�ʱ�����ֵ�
        /// </summary>
        private Dictionary<ulong, Timer> _timerDic = new Dictionary<ulong, Timer>();
        #endregion

        /// <summary>
        /// ���ָ�����Ƶļ�ʱ�������Ὺʼ��ʱ
        /// </summary>
        /// <param name="seconds">��ʱ��һ�ּ�ʱ�ĺ���ֵ</param>
        /// <param name="loopTime">��ʱ���Ƿ�ѭ����ʱ</param>
        /// <param name="onTiming">��ʱ�в���ִ��</param>
        /// <param name="onTimingEnd">ÿ�ּ�ʱ����ʱִ��һ��</param>
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
        /// ��ͣ���߼���ָ����ʱ��
        /// </summary>
        /// <param name="id">��ʱ��id</param>
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
        /// ��ָͣ����ʱ��
        /// </summary>
        /// <param name="id">��ʱ��id</param>
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
        /// ����ָ����ʱ��
        /// </summary>
        /// <param name="id">��ʱ��id</param>
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
        /// ��ͣ���м�ʱ��
        /// </summary>
        public void PauseAllTimer()
        {
            foreach(var pairs in _timerDic)
            {
                pairs.Value.PauseTiming();
            }
        }

        /// <summary>
        /// �������м�ʱ��
        /// </summary>
        public void ContinueAllTimer()
        {
            foreach(var pairs in _timerDic)
            {
                pairs.Value.ContinueTiming();
            }
        }

        /// <summary>
        /// ��ȡָ����ʱ���ĵ�ǰ��ʱ����ֵ
        /// ����ֵ-1�����ʱ��������
        /// </summary>
        /// <param name="id">��ʱ������</param>
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
        /// ����ָ��timer
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
        /// ÿ֡�������м�ʱ������������ɵļ�ʱ��
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

