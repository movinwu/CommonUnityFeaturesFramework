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
            CommonLog.Log(CommonConfig.GetStringConfig("ConfigTest", "NetWork", "server"));
            CommonLog.Log(JsonMapper.ToJson(CommonConfig.GetLongArrayConfig("ConfigTest", "NetWork", "ports")));
        }
    }
}
