using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ��������
    /// </summary>
    public enum EResourceLoadType : sbyte
    {
        /// <summary>
        /// Զ��AB(��Զ�˼���AB��)
        /// </summary>
        RemoteAB,

        /// <summary>
        /// ����AB(��StreamingAssets�ļ�����ֱ�Ӽ���AB��,�����ȸ���ʹ��)
        /// </summary>
        LocalAB,

        /// <summary>
        /// ������Դ(Editor����ģʽ)
        /// </summary>
        Editor,
    }
}
