using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ������:Զ�̼���AB��Դ
    /// </summary>
    public class ResourceHelper_RemoteAB : ResourceHelperBase
    {
        protected override void Load()
        {
            this.m_OnLoadStart?.Invoke();
            this.m_OnLoadEnd?.Invoke();
        }
    }
}
