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
                { "q", "�ٶ�" },
            };
            var result = await CommonFeaturesManager.Http.Get("https://cn.bing.com", param);
            if (result.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                CommonFeatures.Log.CommonLog.Net($"http����ɹ�, {result.downloadHandler.text.ToString()}");
            }
            else
            {
                CommonFeatures.Log.CommonLog.NetError($"http�������, {result.result.ToString()}");
            }
        }
    }
}
