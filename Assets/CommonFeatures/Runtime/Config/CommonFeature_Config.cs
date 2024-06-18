using CommonFeatures.Log;
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

        private const string CONFIG_ASSET_PATH = "Config/";

        public override void Init()
        {
            
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
            var configType = typeof(T);
            if (m_AllConfigAsset.TryGetValue(configType, out var value))
            {
                return value as T;
            }

            var config = LoadConfig<T>();
            m_AllConfigAsset.Add(configType, config);
            return config;
        }

        /// <summary>
        /// 加载配置文件
        /// <para>Editor下使用此方法获取配置文件</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadConfig<T>() where T : ScriptableObject
        {
            var configType = typeof(T);
            var configPath = $"{CONFIG_ASSET_PATH}{configType.Name}";
            var config = Resources.Load<T>(configPath);
            if (null == config)
            {
                CommonLog.ConfigError($"Resources下配置文件{configPath}不存在");
                return null;
            }
            return config;
        }
    }
}
