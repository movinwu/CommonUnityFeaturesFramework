using CommonFeatures.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestLog : MonoBehaviour
    {
        private void Start()
        {
            CommonLog.SetTags(ELogType.Trace);

            var sw = CommonLog.StartTimeWatch();
            for (int i = 0; i < 10000; i++)
            {
                var sin = Mathf.Sin(i);
                var cos = Mathf.Cos(i);
            }
            CommonLog.StopTimeWatch(sw);

            try
            {
                int k = 0;
                int j = 3 / k;
            }
            catch (Exception ex)
            {
                CommonLog.Trace(ex);
            }
        }
    }
}
