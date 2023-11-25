using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// Http«Î«Û≤Œ ˝
    /// </summary>
    public class HttpRequestParam
    {
        public string Key { get; private set; }

        public string Value { get; private set; }

        public HttpRequestParam(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
