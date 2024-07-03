using CommonFeatures.Log;
using CommonFeatures.Pool;
using Cysharp.Threading.Tasks;
using INIParser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Localization
{
    /// <summary>
    /// 通用功能-本地化
    /// </summary>
    public class CommonFeature_Localization : CommonFeature
    {
        /// <summary>
        /// 主包中的本地化配置
        /// </summary>
        [Header("主包中本地化配置")]
        [SerializeField, Tooltip("多语言文本和多语言资源名称都可以在这里配置,文档文件名称和语言枚举名称保持一致")] 
        private TextAsset[] m_MainLocalization;

        [Header("当前语言")]
        [SerializeField]
        private ELanguage _CurLanguage = ELanguage.Null;

        /// <summary>
        /// 主包所有多语言配置
        /// </summary>
        private Dictionary<ELanguage, Dictionary<string, string>> m_MainLocalizationData = new Dictionary<ELanguage, Dictionary<string, string>>((int)ELanguage.Max);

        /// <summary>
        /// 热更所有多语言配置
        /// </summary>
        private Dictionary<ELanguage, Dictionary<int, string>> m_HotfixLocalizationData = new Dictionary<ELanguage, Dictionary<int, string>>((int)ELanguage.Max);

        public ELanguage CurLanguage
        {
            get
            {
                return _CurLanguage;
            }
            set
            {
                _CurLanguage = value;

                //发出预定义好的语言改变事件
                var e = ReferencePool.Acquire<LocalizationEvent>();
                CommonFeaturesManager.Event.SendMessage(e);
            }
        }

        public override UniTask Init()
        {
            //读取主包本地化配置
            m_MainLocalizationData.Clear();

            if (null != m_MainLocalization)
            {
                for (int i = 0; i < m_MainLocalization.Length; i++)
                {
#if UNITY_WEBGL
                    if (Enum.TryParse(typeof(ELanguage), m_MainLocalization[i].name, out var result) && result is ELanguage type)
#else
                    if (Enum.TryParse<ELanguage>(m_MainLocalization[i].name, out var type))
#endif
                    {
                        //主包多语言使用ini配置
                        var parser = new IniDataParser();
                        var data = parser.Parse(m_MainLocalization[i].text);
                        var localizationData = new Dictionary<string, string>();
                        for (int j = 0; j < data.Sections.Count; j++)
                        {
                            for (int k = 0; k < data.Sections[j].Properties.Count; k++)
                            {
                                var property = data.Sections[j].Properties[k];
                                localizationData.Add(property.Key, property.Value);
                            }
                        }

                        m_MainLocalizationData.Add(type, localizationData);
                    }
                }
            }

            //如果没有设置初始化多语言,采用用户当前多语言
            if (_CurLanguage == ELanguage.Null
                || _CurLanguage == ELanguage.Max)
            {
                var applicationLanguage = Application.systemLanguage;
                if (applicationLanguage == SystemLanguage.ChineseSimplified)
                {
                    _CurLanguage = ELanguage.ChineseSimplified;
                }
                else
                {
                    _CurLanguage = ELanguage.English;
                }
            }

            return base.Init();
        }

        /// <summary>
        /// 添加热更新多语言配置
        /// </summary>
        /// <param name="language"></param>
        /// <param name="localizationConfig"></param>
        /// <param name="clearOld">是否清除旧配置</param>
        public void AddHotfixLocalizationConfig(ELanguage language, Dictionary<int, string> localizationConfig, bool clearOld = true)
        {
            if (clearOld)
            {
                if (m_HotfixLocalizationData.ContainsKey(language))
                {
                    m_HotfixLocalizationData.Remove(language);
                }
            }

            if (m_HotfixLocalizationData.ContainsKey(language))
            {
                var curConfig = m_HotfixLocalizationData[language];
                foreach (var config in localizationConfig)
                {
                    if (curConfig.ContainsKey(config.Key))
                    {
                        curConfig[config.Key] = config.Value;
                    }
                    else
                    {
                        curConfig.Add(config.Key, config.Value);
                    }
                }
            }
            else
            {
                m_HotfixLocalizationData.Add(language, localizationConfig);
            }
        }

        /// <summary>
        /// 获取主包多语言配置
        /// </summary>
        /// <param name="languageKey"></param>
        /// <param name="language"></param>
        public string GetMainLocalization(string languageKey, ELanguage language = ELanguage.Null)
        {
            if (language == ELanguage.Null)
            {
                language = CurLanguage;
            }

            if (m_MainLocalizationData.ContainsKey(language))
            {
                var config = m_MainLocalizationData[language];
                if (config.ContainsKey(languageKey))
                {
                    return config[languageKey];
                }
                else
                {
                    CommonLog.ConfigError($"主包语言{language}的本地化配置中不存在key: {languageKey}");
                }
            }
            else
            {
                CommonLog.ConfigError($"主包语言{language}的本地化配置不存在");
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取热更多语言配置
        /// </summary>
        /// <param name="languageKey"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetHotfixLocalizationConfig(int languageKey, ELanguage language = ELanguage.Null)
        {
            if (language == ELanguage.Null)
            {
                language = CurLanguage;
            }

            if (m_HotfixLocalizationData.ContainsKey(language))
            {
                var config = m_HotfixLocalizationData[language];
                if (config.ContainsKey(languageKey))
                {
                    return config[languageKey];
                }
                else
                {
                    CommonLog.ConfigError($"热更语言{language}的本地化配置中不存在key: {languageKey}");
                }
            }
            else
            {
                CommonLog.ConfigError($"热更语言{language}的本地化配置不存在");
            }

            return string.Empty;
        }
    }
}
