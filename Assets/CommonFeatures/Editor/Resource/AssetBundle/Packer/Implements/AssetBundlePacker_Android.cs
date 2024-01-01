using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    public class AssetBundlePacker_Android : AssetBundlePackerBase
    {
        public override void PackAssetBundle()
        {
            BuildPipeline.BuildAssetBundles("", m_BuildDatasOnPack, BuildAssetBundleOptions.None, BuildTarget.Android);
        }
    }
}
