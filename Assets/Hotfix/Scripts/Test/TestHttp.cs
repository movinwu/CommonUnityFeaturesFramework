using CommonFeatures.NetWork;
using CommonFeatures.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestHttp : MonoBehaviour
    {
        private void Start()
        {
            var p0 = ReferencePool.Acquire<HttpRequestParam>();
            p0.Key = "q";
            p0.Value = "�ٶ�";
            var param = new List<HttpRequestParam>()
            {
                p0,
            };
            CFM.Http.Get("https://cn.bing.com", 
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
