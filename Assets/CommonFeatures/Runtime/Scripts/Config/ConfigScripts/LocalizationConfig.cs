using CommonFeatures.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// ���ػ�����
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Localization", fileName = "LocalizationConfig")]
    public class LocalizationConfig : ScriptableObject
    {
        /// <summary>
        /// ������������Դ
        /// <para>�ļ���һ��Ϊ����ö��<see cref="CommonFeatures.Localization.ELanguage"/>��Ӧ����</para>
        /// </summary>
        public TextAsset[] MainHotfixAssets;

        /// <summary>
        /// �ָ���,���ڷָ����Ե�key��value
        /// </summary>
        public char SplitChar;
    }
}
