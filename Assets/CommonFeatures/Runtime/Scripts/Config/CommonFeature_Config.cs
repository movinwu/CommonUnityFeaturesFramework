using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Config
{
    public class CommonFeature_Config : CommonFeature
    {
        /// <summary>
        /// ���������ļ���Դ
        /// </summary>
        private Dictionary<Type, ScriptableObject> m_AllConfigAsset = new Dictionary<Type, ScriptableObject>();

        /// <summary>
        /// ���������ļ�
        /// </summary>
        [SerializeField]
        private List<ScriptableObject> m_AllConfigList;

        public override UniTask Init()
        {
            m_AllConfigAsset.Clear();
            if (null != m_AllConfigList)
            {
                m_AllConfigList.ForEach(x =>
                {
                    var assetType = x.GetType();
                    if (m_AllConfigAsset.ContainsKey(assetType))
                    {
                        CommonLog.ConfigError($"�����ļ��а����ظ������ļ� {m_AllConfigAsset[assetType].name} �� {x.name}");
                    }
                    else
                    {
                        m_AllConfigAsset.Add(x.GetType(), x);
                    }
                });
            }
            return base.Init();
        }

        public override void Release()
        {
            
        }

        /// <summary>
        /// ��ȡ�����ļ�
        /// <para>Runtimeʱʹ�ô˷�����ȡ�����ļ�,�����ظ�����</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : ScriptableObject
        {
            if (!Application.isPlaying)
            {
                CommonLog.ConfigError($"����Ϸû������ʱ�������ȡ�����ļ�,��ʹ�ú��� LoadConfig");
                return null;
            }

            var configType = typeof(T);
            if (m_AllConfigAsset.TryGetValue(configType, out var value))
            {
                return value as T;
            }

            CommonLog.ConfigError($"�����ļ�{configType.Name}ʵ��û�д洢");
            return null;
        }

#if UNITY_EDITOR

        private const string CONFIG_ASSET_PATH = "Assets/CommonFeatures/Runtime/Res/Config/";

        /// <summary>
        /// ���������ļ�
        /// <para>Editor��ʹ�ô˷�����ȡ�����ļ�</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadConfig<T>() where T : ScriptableObject
        {
            if (Application.isPlaying)
            {
                CommonLog.ConfigError($"����Ϸ����ʱ��������������ļ�,��ʹ�ú��� GetConfig");
                return null;
            }

            var configType = typeof(T);
            var configPath = $"{CONFIG_ASSET_PATH}{configType.Name}";
            var config = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(configPath);
            if (null == config)
            {
                CommonLog.ConfigError($"�����ļ�{configPath}������");
                return null;
            }
            return config;
        }
#endif
    }
}
