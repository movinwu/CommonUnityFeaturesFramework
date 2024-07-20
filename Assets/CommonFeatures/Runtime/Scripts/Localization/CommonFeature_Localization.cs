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
    /// ͨ�ù���-���ػ�
    /// </summary>
    public class CommonFeature_Localization : CommonFeature
    {
        [Header("��ǰ����")]
        [SerializeField]
        private ELanguage _CurLanguage = ELanguage.Null;

        /// <summary>
        /// ���ж���������
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

                //����Ԥ����õ����Ըı��¼�
                var e = CFM.ReferencePool.Acquire<LocalizationEvent>();
                CFM.Event.SendMessage(e);
            }
        }

        public override UniTask Init()
        {
            //��ȡ�������ػ�����
            m_LocalizationData.Clear();

            //���û�����ó�ʼ��������,�����û���ǰ������
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

            //��Ҫ��֤����Configģ���������ٶ�ȡ������
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
        /// ��ȡ����������
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
                CommonLog.LogError("��ȡ���������ó���, ��������һ��");
                return;
            }
            //��һ��Ϊ������������
            if (int.TryParse(contents[0], out var enumValue))
            {
                var languageType = (ELanguage)enumValue;
                if (languageType <= ELanguage.Null || languageType >= ELanguage.Max)
                {
                    CommonLog.LogError($"��ȡ���������ó���, �������� {languageType} ����ȷ");
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
                        CommonLog.LogError($"��ȡ���������ó���, ����: {i + 1}, ����: {languageType}");
                        continue;
                    }
                    var dic = m_LocalizationData[languageType];
                    if (dic.ContainsKey(keyValuePair[0]))
                    {
                        CommonLog.LogError($"�����Լ��ظ�, ����: {languageType}, ��: {keyValuePair[0]}, �ظ�ֵ1: {dic[keyValuePair[0]]}, �ظ�ֵ2: {keyValuePair[1]}");
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
                CommonLog.LogError($"��ȡ���������ó���, �������� {contents[0]} ����ȷ");
            }
        }

        /// <summary>
        /// ��ȡ��������������
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
                    CommonLog.ConfigError($"��������{language}�ı��ػ������в�����key: {languageKey}");
                }
            }
            else
            {
                CommonLog.ConfigError($"��������{language}�ı��ػ����ò�����");
            }

            return string.Empty;
        }
    }
}
