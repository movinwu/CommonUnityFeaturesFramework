using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ͨ�ù���-��Դ
    /// </summary>
    public class CommonFeature_Resource : CommonFeature
    {
        /// <summary>
        /// Ĭ����Դ��
        /// </summary>
        private ResourcePackage m_DefaultPackage;

        public override void Init()
        {
            //��ʼ����Դϵͳ
            YooAssets.Initialize();
        }
    }
}
