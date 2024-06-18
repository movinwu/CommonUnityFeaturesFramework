using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// ��Դ��������
    /// </summary>
    public enum EResourceLoadType : byte
    {
        /// <summary>
        /// �༭��ģ��ģʽ
        /// </summary>
        Editor,

        /// <summary>
        /// ����AB(AB��Դ����ڱ���StreamingAssets��)
        /// </summary>
        LocalAB,

        /// <summary>
        /// Զ��AB(AB��Դ�����Զ�̷�����)
        /// </summary>
        RemoteAB,

        /// <summary>
        /// WebGLԶ��AB(AB��Դ�����Զ�̷�����,WebGL����)
        /// </summary>
        WebGLRemoteAB,
    }
}
