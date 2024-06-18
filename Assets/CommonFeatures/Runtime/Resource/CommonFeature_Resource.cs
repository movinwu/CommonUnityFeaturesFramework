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
            if (null == m_DefaultPackage)
            {
                //初始化资源系统
                YooAssets.Initialize();

                // 创建默认的资源包
                m_DefaultPackage = YooAssets.CreatePackage("DefaultPackage");

                // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
                YooAssets.SetDefaultPackage(m_DefaultPackage);
            }
        }
    }
}
