using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOPS
{
    public class TestLog : MonoBehaviour
    {
        private void Start()
        {
            Logger.SetTags(ELogType.Trace);

            var sw = Logger.StartTimeWatch();
            for (int i = 0; i < 10000; i++)
            {
                var sin = Mathf.Sin(i);
                var cos = Mathf.Cos(i);
            }
            Logger.StopTimeWatch(sw);

            try
            {
                int k = 0;
                int j = 3 / k;
            }
            catch (Exception ex)
            {
                Logger.Trace(ex);
            }
        }
    }
}
