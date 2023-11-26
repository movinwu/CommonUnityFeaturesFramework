using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 资源加载类型
    /// </summary>
    public enum EResourceLoadType : sbyte
    {
        /// <summary>
        /// 远程AB(从远端加载AB包)
        /// </summary>
        RemoteAB,

        /// <summary>
        /// 本地AB(从StreamingAssets文件夹下直接加载AB包,测试热更新使用)
        /// </summary>
        LocalAB,

        /// <summary>
        /// 本地资源(Editor开发模式)
        /// </summary>
        Editor,
    }
}
