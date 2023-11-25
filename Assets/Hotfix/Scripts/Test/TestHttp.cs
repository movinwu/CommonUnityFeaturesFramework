using CommonFeatures.NetWork;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestHttp : MonoBehaviour
    {
        private void Start()
        {
            var param = new List<HttpRequestParam>()
            {
                new HttpRequestParam("q", "�ٶ�"),
            };
            HttpManager.Instance.Get("https://cn.bing.com", 
                //param,
                null,
                webrequest =>
                {
                    CommonFeatures.Log.CommonLog.Net($"http����ɹ�, {webrequest.downloadHandler.text.ToString()}");
                },
                webrequest =>
                {
                    CommonFeatures.Log.CommonLog.NetError($"http�������, {webrequest.result.ToString()}");
                });
        }
    }
}
