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
            if (null == m_DefaultPackage)
            {
                //��ʼ����Դϵͳ
                YooAssets.Initialize();

                // ����Ĭ�ϵ���Դ��
                m_DefaultPackage = YooAssets.CreatePackage("DefaultPackage");

                // ���ø���Դ��ΪĬ�ϵ���Դ��������ʹ��YooAssets��ؼ��ؽӿڼ��ظ���Դ�����ݡ�
                YooAssets.SetDefaultPackage(m_DefaultPackage);
            }
        }
    }
}
