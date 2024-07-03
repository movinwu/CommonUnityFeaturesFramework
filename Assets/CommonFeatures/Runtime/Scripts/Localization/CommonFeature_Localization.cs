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
    /// ͨ�ù���-���ػ�
    /// </summary>
    public class CommonFeature_Localization : CommonFeature
    {
        /// <summary>
        /// �����еı��ػ�����
        /// </summary>
        [Header("�����б��ػ�����")]
        [SerializeField, Tooltip("�������ı��Ͷ�������Դ���ƶ���������������,�ĵ��ļ����ƺ�����ö�����Ʊ���һ��")] 
        private TextAsset[] m_MainLocalization;

        [Header("��ǰ����")]
        [SerializeField]
        private ELanguage _CurLanguage = ELanguage.Null;

        /// <summary>
        /// �������ж���������
        /// </summary>
        private Dictionary<ELanguage, Dictionary<string, string>> m_MainLocalizationData = new Dictionary<ELanguage, Dictionary<string, string>>((int)ELanguage.Max);

        /// <summary>
        /// �ȸ����ж���������
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

                //����Ԥ����õ����Ըı��¼�
                var e = ReferencePool.Acquire<LocalizationEvent>();
                CommonFeaturesManager.Event.SendMessage(e);
            }
        }

        public override UniTask Init()
        {
            //��ȡ�������ػ�����
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
                        //����������ʹ��ini����
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

            return base.Init();
        }

        /// <summary>
        /// ����ȸ��¶���������
        /// </summary>
        /// <param name="language"></param>
        /// <param name="localizationConfig"></param>
        /// <param name="clearOld">�Ƿ����������</param>
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
        /// ��ȡ��������������
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
                    CommonLog.ConfigError($"��������{language}�ı��ػ������в�����key: {languageKey}");
                }
            }
            else
            {
                CommonLog.ConfigError($"��������{language}�ı��ػ����ò�����");
            }

            return string.Empty;
        }

        /// <summary>
        /// ��ȡ�ȸ�����������
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
                    CommonLog.ConfigError($"�ȸ�����{language}�ı��ػ������в�����key: {languageKey}");
                }
            }
            else
            {
                CommonLog.ConfigError($"�ȸ�����{language}�ı��ػ����ò�����");
            }

            return string.Empty;
        }
    }
}
