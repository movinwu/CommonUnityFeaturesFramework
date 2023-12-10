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
            CommonLog.Log(CFM.Config.GetStringConfig("ConfigTest", "NetWork", "server"));
            CommonLog.Log(JsonMapper.ToJson(CFM.Config.GetLongArrayConfig("ConfigTest", "NetWork", "ports")));
        }
    }
}
