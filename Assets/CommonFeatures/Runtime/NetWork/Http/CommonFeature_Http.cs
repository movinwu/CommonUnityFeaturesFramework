using CommonFeatures.Log;
using CommonFeatures.Pool;
using CommonFeatures.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通用功能-Http连接请求
    /// </summary>
    public class CommonFeature_Http : CommonFeature
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        private const string Server = "http://127.0.0.1/";

        /// <summary>
        /// 默认请求超时时间(秒)
        /// </summary>
        private const int Timeout = 10;

        /// <summary>
        /// 所有正在请求的http
        /// </summary>
        private Dictionary<string, HttpRequestHandler> requestingHttpDic = new Dictionary<string, HttpRequestHandler>();

        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="completeCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="timeout"></param>
        public void Get(string name, List<HttpRequestParam> param, Action<UnityWebRequest> completeCallback, Action<UnityWebRequest> errorCallback, int timeout = Timeout)
        {
            //检验给定地址名
            if (string.IsNullOrEmpty(name))
            {
                CommonLog.NetError("http get请求地址不能为空");
            }
            StringBuilder urlSb;
            if (name.ToLower().StartsWith("http"))
            {
                urlSb = new StringBuilder(name);
            }
            else
            {
                urlSb = new StringBuilder(Server);
                urlSb.Append(name);
            }
            //检验参数
            if (null != param && param.Count > 0)
            {
                if (!urlSb.ToString().Contains('?'))
                {
                    urlSb.Append('?');
                }
                for (int i = 0; i < param.Count; i++)
                {
                    urlSb.Append(param[i].Key);
                    urlSb.Append("=");
                    urlSb.Append(param[i].Value);
                    if (i < param.Count - 1)
                    {
                        urlSb.Append("&");
                    }
                }
            }
            string url = urlSb.ToString();
            //检验正在发送
            if (requestingHttpDic.ContainsKey(url))
            {
                CommonLog.NetWarning($"网络请求{url}已经在请求中,不能重复请求");
                return;
            }

            var handler = ReferencePool.Acquire<HttpRequestHandler>();
            handler.URL = url;
            handler.OnSuccessCallback = completeCallback;
            handler.OnErrorCallback = errorCallback;
            handler.Params = param;
            handler.Name = name;
            requestingHttpDic.Add(url, handler);
            handler.Request = UnityWebRequest.Get(url);
            handler.Request.timeout = timeout;
            handler.Request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            handler.Request.SendWebRequest();
        }

        /// <summary>
        /// Http Post请求
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="completeCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="timeout"></param>
        public void Post(string name, List<HttpRequestParam> param, Action<UnityWebRequest> completeCallback, Action<UnityWebRequest> errorCallback, int timeout = Timeout)
        {
            //检验给定地址名
            if (string.IsNullOrEmpty(name))
            {
                CommonFeatures.Log.CommonLog.NetError("http post请求地址不能为空");
            }
            //检验参数
            if (null == param || param.Count == 0)
            {
                CommonFeatures.Log.CommonLog.NetError("post请求表单不能为空");
                return;
            }
            string url;
            if (name.ToLower().StartsWith("http"))
            {
                url = name;
            }
            else
            {
                url = Server + name;
            }
            //检验正在发送
            if (requestingHttpDic.ContainsKey(url))
            {
                CommonFeatures.Log.CommonLog.NetWarning($"网络请求{url}已经在请求中,不能重复请求");
                return;
            }

            var handler = ReferencePool.Acquire<HttpRequestHandler>();
            handler.URL = url;
            handler.OnSuccessCallback = completeCallback;
            handler.OnErrorCallback = errorCallback;
            handler.Params = param;
            handler.Name = name;
            requestingHttpDic.Add(url, handler);

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            for (int i = 0; i < param.Count; i++)
            {
                formData.Add(new MultipartFormFileSection(param[i].Key, param[i].Value));
            }

            handler.Request = UnityWebRequest.Post(url, formData);
            handler.Request.timeout = timeout;
            handler.Request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            handler.Request.SendWebRequest();
        }

        public override void Tick()
        {
            if (requestingHttpDic.Count > 0)
            {
                var handlers = requestingHttpDic.Values.ToArray();
                foreach (var handler in handlers)
                {
                    if (handler.Request.isDone)
                    {
                        requestingHttpDic.Remove(handler.URL);
                        switch (handler.Request.result)
                        {
                            case UnityWebRequest.Result.Success:
                                handler.OnSuccessCallback?.Invoke(handler.Request);
                                break;
                            case UnityWebRequest.Result.ConnectionError:
                            case UnityWebRequest.Result.ProtocolError:
                            case UnityWebRequest.Result.DataProcessingError:
                                handler.OnErrorCallback?.Invoke(handler.Request);
                                break;
                        }
                        handler.Request.Dispose();
                        if (null != handler.Params)
                        {
                            for (int i = 0; i < handler.Params.Count; i++)
                            {
                                ReferencePool.Back(handler.Params[i]);
                            }
                        }
                        ReferencePool.Back(handler);
                    }
                }
            }
        }
    }
}
