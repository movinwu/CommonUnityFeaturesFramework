using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// ͨ�ù���-Http��������
    /// </summary>
    public class CommonFeature_Http : CommonFeature
    {
        /// <summary>
        /// Ĭ�Ϸ�������ַ
        /// </summary>
        private const string DEFAULT_SERVER = "http://127.0.0.1/";

        /// <summary>
        /// Ĭ������ʱʱ��(��)
        /// </summary>
        private const int TIMEOUT = 5;

        /// <summary>
        /// Http Get����
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
            //���������ַ��
            if (string.IsNullOrEmpty(name))
            {
                CommonLog.NetError("http get�����ַ����Ϊ��");
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
            //�������
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
                //�Ƴ���ƴ�ӵ����һ�� & �ַ�
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
        /// Http Post����
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
            //���������ַ��
            if (string.IsNullOrEmpty(name))
            {
                CommonFeatures.Log.CommonLog.NetError("http post�����ַ����Ϊ��");
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
        /// Http Head����
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
            //���������ַ��
            if (string.IsNullOrEmpty(name))
            {
                CommonLog.NetError("http get�����ַ����Ϊ��");
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
            //�������
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
                //�Ƴ���ƴ�ӵ����һ�� & �ַ�
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
