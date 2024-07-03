using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դʹ���߽ӿ�
    /// <para>Ҫ��ʵ��<see cref="IDisposable"/>,�����е���<see cref="CommonFeature_Resource.ReleaseUsingHandle(IResourceUser)"/>�ͷ���Դ,�����ֶ����ô˺����ͷ���Դ(�Ƽ������ͷŷ�ʽ��ʵ��)</para>
    /// </summary>
    public interface IResourceUser : IDisposable
    {
        /// <summary>
        /// ��Դ����
        /// </summary>
        string PackageName { get; }
    }
}
