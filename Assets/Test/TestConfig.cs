using CommonFeatures.Config;
using CommonFeatures.Log;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestConfig : MonoBehaviour
    {
        private void Start()
        {
            CommonLog.Trace(ConfigManager.Instance.GetStrConfig("Config", "NetWork", "server"));
            CommonLog.Trace(JsonMapper.ToJson(ConfigManager.Instance.GetLongArrayConfig("Config", "NetWork", "ports")));
        }
    }
}
