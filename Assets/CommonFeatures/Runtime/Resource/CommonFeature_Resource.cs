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
    /// ͨ�ù���-��Դ
    /// </summary>
    public class CommonFeature_Resource : CommonFeature
    {
        /// <summary>
        /// Ĭ����Դ��
        /// </summary>
        private ResourcePackage m_DefaultPackage;

        public override void Init()
        {
            //��ʼ����Դϵͳ
            YooAssets.Initialize();
        }

        /// <summary>
        /// �첽���ص�����Դ
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
        /// ͬ�����ص�����Դ
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
        /// �첽������Ϸ����
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
        /// ͬ��������Ϸ����
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
        /// �첽������Դ����������Դ
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
        /// ͬ��������Դ����������Դ
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
        /// �첽���س���
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
