using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OOPS
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
                    Logger.Net($"http����ɹ�, {webrequest.downloadHandler.text.ToString()}");
                },
                webrequest =>
                {
                    Logger.NetError($"http�������, {webrequest.result.ToString()}");
                });
        }
    }
}
