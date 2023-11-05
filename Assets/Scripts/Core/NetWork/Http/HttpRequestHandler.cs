using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OOPS
{
    /// <summary>
    /// Http������,��װHttp������Ϣ
    /// </summary>
    public class HttpRequestHandler
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
    }
}
