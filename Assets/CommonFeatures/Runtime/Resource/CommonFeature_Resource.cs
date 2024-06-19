using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace CommonFeatures.Resource
{
    /// <summary>
    /// 通用功能-资源
    /// </summary>
    public class CommonFeature_Resource : CommonFeature
    {
        /// <summary>
        /// 默认资源包
        /// </summary>
        private ResourcePackage m_DefaultPackage;

        public override void Init()
        {
            //初始化资源系统
            YooAssets.Initialize();
        }

        /// <summary>
        /// 异步加载单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<T> LoadAssetAsync<T>(string location,
            uint priority = 0) where T : UnityEngine.Object
        {
            var handle = m_DefaultPackage.LoadAssetAsync<T>(location, priority);
            await handle.ToUniTask();

            var obj = handle.GetAssetObject<T>();
            return obj;
        }

        /// <summary>
        /// 同步加载单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns></returns>
        public T LoadAssetSync<T>(string location) where T : UnityEngine.Object
        {
            var handle = m_DefaultPackage.LoadAssetSync<T>(location);

            var obj = handle.GetAssetObject<T>();
            return obj;
        }

        /// <summary>
        /// 异步加载游戏物体
        /// </summary>
        /// <param name="location"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<GameObject> LoadAssetAsync(string location,
            uint priority = 0)
        {
            var handle = m_DefaultPackage.LoadAssetAsync<GameObject>(location, priority);
            await handle.ToUniTask();

            var instantiateOperation = handle.InstantiateAsync();
            await instantiateOperation.ToUniTask();
            return instantiateOperation.Result;
        }

        /// <summary>
        /// 同步加载游戏物体
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public GameObject LoadGameObjectSync(string location)
        {
            var handle = m_DefaultPackage.LoadAssetSync<GameObject>(location);

            var result = handle.InstantiateSync();
            return result;
        }

        /// <summary>
        /// 异步加载资源包内所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <param name="progress"></param>
        /// <param name="timing"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<T[]> LoadAllAssetsAsync<T>(string location,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            uint priority = 0) where T : UnityEngine.Object
        {
            var handle = m_DefaultPackage.LoadAllAssetsAsync<T>(location, priority);
            await handle.ToUniTask(progress, timing);

            var obj = handle.AllAssetObjects
                .Select(x => x as T)
                .ToArray();
            return obj;
        }

        /// <summary>
        /// 同步加载资源包内所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public T[] LoadAllAssetSync<T>(string location, uint priority = 0) where T : UnityEngine.Object
        {
            var handle = m_DefaultPackage.LoadAllAssetsAsync<T>(location, priority);

            var obj = handle.AllAssetObjects
                .Select(x => x as T)
                .ToArray();
            return obj;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location"></param>
        /// <param name="progress"></param>
        /// <param name="timing"></param>
        /// <param name="sceneMode"></param>
        /// <param name="suspendLoad"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask LoadSceneAsync(string location,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            uint priority = 0)
        {
            var handle = m_DefaultPackage.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            await handle.ToUniTask(progress, timing);
        }
    }
}
