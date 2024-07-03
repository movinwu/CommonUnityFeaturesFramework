using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源使用者接口
    /// <para>要求实现<see cref="IDisposable"/>,在其中调用<see cref="CommonFeature_Resource.ReleaseUsingHandle(IResourceUser)"/>释放资源,或者手动调用此函数释放资源(推荐两种释放方式都实现)</para>
    /// </summary>
    public interface IResourceUser : IDisposable
    {
        /// <summary>
        /// 资源包名
        /// </summary>
        string PackageName { get; }
    }
}
