using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    public class AssetBundleBuilder_Android : AssetBundleBuildHandlerBase
    {
        /// <summary>
        /// ��׿ƽ̨���
        /// </summary>
        public override void PackAssetBundle()
        {
            //BuildPipeline.BuildAssetBundles("", m_BuildDatasOnPack, BuildAssetBundleOptions.None, BuildTarget.Android);
            Debug.Log("android");
        }
    }
}
