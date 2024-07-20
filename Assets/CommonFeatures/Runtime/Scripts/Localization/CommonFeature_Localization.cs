using CommonFeatures.Config;
using CommonFeatures.Log;
using CommonFeatures.Pool;
using CommonFeatures.Utility;
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
        [Header("当前语言")]
        [SerializeField]
        private ELanguage _CurLanguage = ELanguage.Null;

        /// <summary>
        /// 所有多语言配置
        /// </summary>
        private Dictionary<ELanguage, Dictionary<string, string>> m_LocalizationData = new Dictionary<ELanguage, Dictionary<string, string>>((int)ELanguage.Max);

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
                var e = CFM.ReferencePool.Acquire<LocalizationEvent>();
                CFM.Event.SendMessage(e);
            }
        }

        public override UniTask Init()
        {
            //读取主包本地化配置
            m_LocalizationData.Clear();

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

            //需要保证配置Config模块加载完毕再读取多语言
            var localizationConfig = CFM.Config.GetConfig<LocalizationConfig>();
            var assets = localizationConfig.MainHotfixAssets;
            if (null != localizationConfig.MainHotfixAssets)
            {
                for (int i = 0; i < assets.Length; i++)
                {
                    ReadLocalization(assets[i].text, localizationConfig.SplitChar);
                }
            }

            return base.Init();
        }

        /// <summary>
        /// 读取多语言配置
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="splitChar"></param>
        public void ReadLocalization(string txt, char splitChar)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return;
            }
            var contents = txt.Split("\r\n");
            if (contents.Length <= 1)
            {
                CommonLog.LogError("读取多语言配置出错, 行数至少一行");
                return;
            }
            //第一行为语言类型数字
            if (int.TryParse(contents[0], out var enumValue))
            {
                var languageType = (ELanguage)enumValue;
                if (languageType <= ELanguage.Null || languageType >= ELanguage.Max)
                {
                    CommonLog.LogError($"读取多语言配置出错, 语言类型 {languageType} 不正确");
                    return;
                }

                if (!m_LocalizationData.ContainsKey(languageType))
                {
                    m_LocalizationData.Add(languageType, new Dictionary<string, string>());
                }

                for (int i = 1; i < contents.Length; i++)
                {
                    var keyValuePair = contents[i].Split(splitChar);
                    if (keyValuePair.Length != 2)
                    {
                        CommonLog.LogError($"读取多语言配置出错, 行数: {i + 1}, 语言: {languageType}");
                        continue;
                    }
                    var dic = m_LocalizationData[languageType];
                    if (dic.ContainsKey(keyValuePair[0]))
                    {
                        CommonLog.LogError($"多语言键重复, 语言: {languageType}, 键: {keyValuePair[0]}, 重复值1: {dic[keyValuePair[0]]}, 重复值2: {keyValuePair[1]}");
                        continue;
                    }
                    else
                    {
                        m_LocalizationData[languageType].Add(keyValuePair[0], keyValuePair[1]);
                    }
                }
            }
            else
            {
                CommonLog.LogError($"读取多语言配置出错, 语言类型 {contents[0]} 不正确");
            }
        }

        /// <summary>
        /// 获取主包多语言配置
        /// </summary>
        /// <param name="languageKey"></param>
        /// <param name="language"></param>
        public string GetLocalizationStr(string languageKey, ELanguage language = ELanguage.Null)
        {
            if (language == ELanguage.Null)
            {
                language = CurLanguage;
            }

            if (m_LocalizationData.ContainsKey(language))
            {
                var config = m_LocalizationData[language];
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
    }
}
