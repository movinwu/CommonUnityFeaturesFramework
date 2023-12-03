using CommonFeatures.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.NetWork
{
    /// <summary>
    /// Http�������
    /// </summary>
    public class HttpRequestParam : IReference
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public void Reset()
        {
            Key = string.Empty;
            Value = string.Empty;
        }
    }
}
