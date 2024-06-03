using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// 通用功能-Http连接请求
    /// </summary>
    public class CommonFeature_Http : CommonFeature
    {
        /// <summary>
        /// 默认服务器地址
        /// </summary>
        private const string DEFAULT_SERVER = "http://127.0.0.1/";

        /// <summary>
        /// 默认请求超时时间(秒)
        /// </summary>
        private const int TIMEOUT = 5;

        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="requetsHeader"></param>
        /// <param name="timeout"></param>
        public async UniTask<UnityWebRequest> Get(
            string name,
            Dictionary<string, string> param, 
            Dictionary<string, string> requetsHeader = null, 
            int timeout = TIMEOUT)
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
                urlSb = new StringBuilder(DEFAULT_SERVER);
                urlSb.Append(name);
            }
            //检验参数
            if (null != param && param.Count > 0)
            {
                if (!urlSb.ToString().Contains('?'))
                {
                    urlSb.Append('?');
                }
                foreach (var pair in param)
                {
                    urlSb.Append(pair.Key);
                    urlSb.Append("=");
                    urlSb.Append(pair.Value);
                    urlSb.Append("&");
                }
                //移除多拼接的最后一个 & 字符
                urlSb.Remove(urlSb.Length - 1, 1);
            }
            string url = urlSb.ToString();

            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            if (null != requetsHeader)
            {
                foreach (var header in requetsHeader)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }
            request.timeout = timeout;
            var result = await request.SendWebRequest().ToUniTask();
            return result;
        }

        /// <summary>
        /// Http Post请求
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="requetsHeader"></param>
        /// <param name="timeout"></param>
        public async UniTask<UnityWebRequest> Post(
            string name, 
            Dictionary<string, string> param, 
            Dictionary<string, string> requetsHeader = null, 
            int timeout = TIMEOUT)
        {
            //检验给定地址名
            if (string.IsNullOrEmpty(name))
            {
                CommonFeatures.Log.CommonLog.NetError("http post请求地址不能为空");
            }
            string url;
            if (name.ToLower().StartsWith("http"))
            {
                url = name;
            }
            else
            {
                url = DEFAULT_SERVER + name;
            }

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            if (null != param)
            {
                foreach (var pair in param)
                {
                    formData.Add(new MultipartFormFileSection(pair.Key, pair.Value));
                }
            }

            var request = UnityWebRequest.Post(url, formData);
            request.timeout = timeout;
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            if (null != requetsHeader)
            {
                foreach (var header in requetsHeader)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }
            var result = await request.SendWebRequest().ToUniTask();
            return result;
        }

        /// <summary>
        /// Http Head请求
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="requetsHeader"></param>
        /// <param name="timeout"></param>
        public async UniTask<UnityWebRequest> Head(
            string name, 
            Dictionary<string, string> param, 
            Dictionary<string, string> requetsHeader = null, 
            int timeout = TIMEOUT)
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
                urlSb = new StringBuilder(DEFAULT_SERVER);
                urlSb.Append(name);
            }
            //检验参数
            if (null != param && param.Count > 0)
            {
                if (!urlSb.ToString().Contains('?'))
                {
                    urlSb.Append('?');
                }
                foreach (var pair in param)
                {
                    urlSb.Append(pair.Key);
                    urlSb.Append("=");
                    urlSb.Append(pair.Value);
                    urlSb.Append("&");
                }
                //移除多拼接的最后一个 & 字符
                urlSb.Remove(urlSb.Length - 1, 1);
            }
            string url = urlSb.ToString();

            var request = UnityWebRequest.Head(url);
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            if (null != requetsHeader)
            {
                foreach (var header in requetsHeader)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }
            request.timeout = timeout;
            var result = await request.SendWebRequest().ToUniTask();
            return result;
        }
    }
}
