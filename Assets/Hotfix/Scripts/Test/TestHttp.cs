using CommonFeatures.NetWork;
using CommonFeatures.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestHttp : MonoBehaviour
    {
        private async void Start()
        {
            var param = new Dictionary<string, string>()
            {
                { "q", "百度" },
            };
            var result = await CommonFeaturesManager.Http.Get("https://cn.bing.com", param);
            if (result.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                CommonFeatures.Log.CommonLog.Net($"http请求成功, {result.downloadHandler.text.ToString()}");
            }
            else
            {
                CommonFeatures.Log.CommonLog.NetError($"http请求错误, {result.result.ToString()}");
            }
        }
    }
}
