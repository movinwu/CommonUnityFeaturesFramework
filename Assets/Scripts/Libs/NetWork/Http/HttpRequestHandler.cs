using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OOPS
{
    /// <summary>
    /// Http请求句柄,封装Http请求信息
    /// </summary>
    public class HttpRequestHandler
    {
        /// <summary>
        /// 请求url地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 请求名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public List<HttpRequestParam> Params { get; set; }

        /// <summary>
        /// 请求成功回调
        /// </summary>
        public Action<UnityWebRequest> OnSuccessCallback { get; set; }

        /// <summary>
        /// 请求错误回调
        /// </summary>
        public Action<UnityWebRequest> OnErrorCallback { get; set; }

        /// <summary>
        /// 实际请求
        /// </summary>
        public UnityWebRequest Request { get; set; }
    }
}
