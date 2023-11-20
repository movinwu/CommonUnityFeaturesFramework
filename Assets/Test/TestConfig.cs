using CommonFeatures.Config;
using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestConfig : MonoBehaviour
    {
        private void Start()
        {
            CommonLog.Trace(ConfigManager.Instance.GetStrConfig("NetWork", "server"));
            CommonLog.Trace(ConfigManager.Instance.GetLongArrayConfig("NetWork", "ports"));
        }
    }
}
