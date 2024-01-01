using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// AB���������
    /// </summary>
    public abstract class AssetBundleBuildHandlerBase
    {
        protected AssetBundleBuild[] m_BuildDatasOnPack;

        /// <summary>
        /// ���
        /// </summary>
        public abstract void PackAssetBundle();

        /// <summary>
        /// ��̬��Դ�������
        /// </summary>
        public virtual void AnalysisStaticResources()
        {

        }
    }
}
