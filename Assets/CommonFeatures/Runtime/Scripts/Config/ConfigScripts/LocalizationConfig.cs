using CommonFeatures.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    /// <summary>
    /// 本地化设置
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Localization", fileName = "LocalizationConfig")]
    public class LocalizationConfig : ScriptableObject
    {
        /// <summary>
        /// 主包多语言资源
        /// <para>文件第一行为语言枚举<see cref="CommonFeatures.Localization.ELanguage"/>对应数字</para>
        /// </summary>
        public TextAsset[] MainHotfixAssets;

        /// <summary>
        /// 分隔符,用于分隔语言的key和value
        /// </summary>
        public char SplitChar;
    }
}
