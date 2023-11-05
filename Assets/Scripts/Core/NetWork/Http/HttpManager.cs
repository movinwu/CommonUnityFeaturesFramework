using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace OOPS
{
    /// <summary>
    /// Http��������
    /// </summary>
    public class HttpManager : MonoSingletonBase<HttpManager>
    {
        /// <summary>
        /// ��������ַ
        /// </summary>
        private const string Server = "http://127.0.0.1/";

        /// <summary>
        /// Ĭ������ʱʱ��(��)
        /// </summary>
        private const int Timeout = 10;

        /// <summary>
        /// �������������http
        /// </summary>
        private Dictionary<string, HttpRequestHandler> requestingHttpDic = new Dictionary<string, HttpRequestHandler>();

        /// <summary>
        /// Http Get����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="completeCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="timeout"></param>
        public void Get(string name, List<HttpRequestParam> param, Action<UnityWebRequest> completeCallback, Action<UnityWebRequest> errorCallback, int timeout = Timeout)
        {
            //���������ַ��
            if (string.IsNullOrEmpty(name))
            {
                Logger.NetError("http get�����ַ����Ϊ��");
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
            //�������
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
            //�������ڷ���
            if (requestingHttpDic.ContainsKey(url))
            {
                Logger.NetWarning($"��������{url}�Ѿ���������,�����ظ�����");
                return;
            }

            var handler = new HttpRequestHandler();
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
        /// Http Post����
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="completeCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="timeout"></param>
        public void Post(string name, List<HttpRequestParam> param, Action<UnityWebRequest> completeCallback, Action<UnityWebRequest> errorCallback, int timeout = Timeout)
        {
            //���������ַ��
            if (string.IsNullOrEmpty(name))
            {
                Logger.NetError("http post�����ַ����Ϊ��");
            }
            //�������
            if (null == param || param.Count == 0)
            {
                Logger.NetError("post���������Ϊ��");
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
            //�������ڷ���
            if (requestingHttpDic.ContainsKey(url))
            {
                Logger.NetWarning($"��������{url}�Ѿ���������,�����ظ�����");
                return;
            }

            var handler = new HttpRequestHandler();
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

        private void Update()
        {
            if (requestingHttpDic.Count > 0)
            {
                var handlers = requestingHttpDic.Values.ToArrayPooled();
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
                    }
                }
            }
        }
    }
}
