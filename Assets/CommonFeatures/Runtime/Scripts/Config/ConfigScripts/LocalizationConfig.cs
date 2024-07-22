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
        [Header("������������Դ")]
        public TextAsset[] MainLocalizationAssets;

        [Header("�ָ���,���ڷָ����Ե�key��value")]
        public char SplitChar;
    }
}
