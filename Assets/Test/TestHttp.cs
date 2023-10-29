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
                new HttpRequestParam("q", "百度"),
            };
            HttpManager.Instance.Get("https://cn.bing.com", 
                //param,
                null,
                webrequest =>
                {
                    Logger.Net($"http请求成功, {webrequest.downloadHandler.text.ToString()}");
                },
                webrequest =>
                {
                    Logger.NetError($"http请求错误, {webrequest.result.ToString()}");
                });
        }
    }
}
