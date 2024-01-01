using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// AB包打包基类
    /// </summary>
    public abstract class AssetBundlePackerBase
    {
        protected AssetBundleBuild[] m_BuildDatasOnPack;

        /// <summary>
        /// 打包
        /// </summary>
        public abstract void PackAssetBundle();
    }
}
