using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ���ظ�����:������Դ
    /// </summary>
    public class ResourceHelper_Editor : ResourceHelperBase
    {
        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            this.m_OnLoadEnd?.Invoke();
        }
    }
}
