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
            id = CommonFeaturesManager.Timer.AddTimer(1f, 2, () =>
            {
                CommonLog.Log($"正在计时, 时间 {CommonFeaturesManager.Timer.GetCurrentTime(id)}");
            }, () =>
            {
                CommonLog.Log("计时结束");
            });
        }
    }
}
