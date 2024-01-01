using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// AB包打包基类
    /// </summary>
    public abstract class AssetBundleBuildHandlerBase
    {
        protected AssetBundleBuild[] m_BuildDatasOnPack;

        /// <summary>
        /// 打包
        /// </summary>
        public abstract void PackAssetBundle();

        /// <summary>
        /// 静态资源打包分析
        /// </summary>
        public virtual void AnalysisStaticResources()
        {

        }
    }
}
