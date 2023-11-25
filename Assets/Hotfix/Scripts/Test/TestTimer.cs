using CommonFeatures.Log;
using CommonFeatures.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestTimer : MonoBehaviour
    {
        private void Start()
        {
            ulong id = 1;
            id = TimerManager.Instance.AddTimer(1f, 2, () =>
            {
                CommonLog.Trace($"���ڼ�ʱ, ʱ�� {TimerManager.Instance.GetCurrentTime(id)}");
            }, () =>
            {
                CommonLog.Trace("��ʱ����");
            });
        }
    }
}
