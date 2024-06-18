using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源加载类型
    /// </summary>
    public enum EResourceLoadType : byte
    {
        /// <summary>
        /// 编辑器模拟模式
        /// </summary>
        Editor,

        /// <summary>
        /// 本地AB(AB资源存放在本地StreamingAssets下)
        /// </summary>
        LocalAB,

        /// <summary>
        /// 远程AB(AB资源存放在远程服务器)
        /// </summary>
        RemoteAB,

        /// <summary>
        /// WebGL远程AB(AB资源存放在远程服务器,WebGL特有)
        /// </summary>
        WebGLRemoteAB,
    }
}
