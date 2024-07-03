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
        /// 所有配置文件资源
        /// </summary>
        private Dictionary<Type, ScriptableObject> m_AllConfigAsset = new Dictionary<Type, ScriptableObject>();

        /// <summary>
        /// 所有配置文件
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
                        CommonLog.ConfigError($"配置文件中包含重复类型文件 {m_AllConfigAsset[assetType].name} 和 {x.name}");
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
        /// 获取配置文件
        /// <para>Runtime时使用此方法获取配置文件,无需重复加载</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : ScriptableObject
        {
            if (!Application.isPlaying)
            {
                CommonLog.ConfigError($"在游戏没有运行时不允许获取配置文件,请使用函数 LoadConfig");
                return null;
            }

            var configType = typeof(T);
            if (m_AllConfigAsset.TryGetValue(configType, out var value))
            {
                return value as T;
            }

            CommonLog.ConfigError($"配置文件{configType.Name}实例没有存储");
            return null;
        }

#if UNITY_EDITOR

        private const string CONFIG_ASSET_PATH = "Assets/CommonFeatures/Runtime/Res/Config/";

        /// <summary>
        /// 加载配置文件
        /// <para>Editor下使用此方法获取配置文件</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadConfig<T>() where T : ScriptableObject
        {
            if (Application.isPlaying)
            {
                CommonLog.ConfigError($"在游戏运行时不允许加载配置文件,请使用函数 GetConfig");
                return null;
            }

            var configType = typeof(T);
            var configPath = $"{CONFIG_ASSET_PATH}{configType.Name}";
            var config = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(configPath);
            if (null == config)
            {
                CommonLog.ConfigError($"配置文件{configPath}不存在");
                return null;
            }
            return config;
        }
#endif
    }
}
