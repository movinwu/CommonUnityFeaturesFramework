using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// UI�㼶,�㼶���ֶ�ӦCanvas����Ⱦ˳��Sorting In Order��ֵ
    /// </summary>
    public enum EUILayer : sbyte
    {
        /// <summary>
        /// ������,���ڷ��ý�����Ϸ��¼����\�ȸ��½���ȹ̶�����(���ȸ�)
        /// </summary>
        Base = -3,

        /// <summary>
        /// ������,��Ϸ�еĸ��ֽ�������ڸ���������(�ȸ�)
        /// </summary>
        Float = -2,

        /// <summary>
        /// ���ֲ�,�����赲�û����(���ȸ�)
        /// </summary>
        Mask = -1,

        /// <summary>
        /// ������,���������Ľ���(�ȸ�)
        /// <para>����ʱ���������е���Ԫ�ؿ����и�������,Ԥ����1-9�Ĳ㼶��������������������</para>
        /// </summary>
        Guide = 0,

        /// <summary>
        /// ��ʾ��,������ʾ��������ʾ,����㼶�µ���ʾ�ᱻ���ؽ����ڵ�(���ȸ�)
        /// </summary>
        Tip = 10,

        /// <summary>
        /// ���ز�,���ڶ�������������ʱ�赲�û����(���ȸ�)
        /// </summary>
        Loading = 11,

        /// <summary>
        /// ��ʾ��,������ʾ��������ʾ,�ڼ��ز��Ϸ�(���ȸ�)
        /// </summary>
        Notice = 12,

        /// <summary>
        /// ���Բ�,cmd���ں͵��Դ���(���ȸ�)
        /// </summary>
        Debug = 13,
    }
}
