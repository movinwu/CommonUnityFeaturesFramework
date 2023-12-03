using CommonFeatures.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// Http������,��װHttp������Ϣ
    /// </summary>
    public class HttpRequestHandler : IReference
    {
        /// <summary>
        /// ����url��ַ
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public List<HttpRequestParam> Params { get; set; }

        /// <summary>
        /// ����ɹ��ص�
        /// </summary>
        public Action<UnityWebRequest> OnSuccessCallback { get; set; }

        /// <summary>
        /// �������ص�
        /// </summary>
        public Action<UnityWebRequest> OnErrorCallback { get; set; }

        /// <summary>
        /// ʵ������
        /// </summary>
        public UnityWebRequest Request { get; set; }

        public void Reset()
        {
            URL = string.Empty;
            Name = string.Empty;
            Params = null;
            OnSuccessCallback = null;
            OnErrorCallback = null;
            Request = null;
        }
    }
}
