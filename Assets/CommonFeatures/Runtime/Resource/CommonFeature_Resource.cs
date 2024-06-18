using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 通用功能-资源
    /// </summary>
    public class CommonFeature_Resource : CommonFeature
    {
        /// <summary>
        /// 默认资源包
        /// </summary>
        private ResourcePackage m_DefaultPackage;

        public override void Init()
        {
            //初始化资源系统
            YooAssets.Initialize();
        }
    }
}
