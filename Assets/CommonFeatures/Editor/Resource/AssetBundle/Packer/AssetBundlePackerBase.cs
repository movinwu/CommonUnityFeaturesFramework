using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// AB���������
    /// </summary>
    public abstract class AssetBundlePackerBase
    {
        protected AssetBundleBuild[] m_BuildDatasOnPack;

        /// <summary>
        /// ���
        /// </summary>
        public abstract void PackAssetBundle();
    }
}
