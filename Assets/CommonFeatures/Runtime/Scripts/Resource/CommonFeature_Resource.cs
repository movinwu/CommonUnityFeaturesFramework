using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System;
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
        /// 所有资源包
        /// </summary>
        private List<ResourcePackage> m_AllResourcePackagePacker;

        /// <summary>
        /// 所有资源句柄
        /// </summary>
        private Dictionary<ResourceHandleBasePacker, HashSet<IResourceUser>> m_AllResourceHandle;

        /// <summary>
        /// 所有待移除的资源句柄
        /// </summary>
        private List<ResourceHandleBasePacker> m_AllHandleToRelease;

        private const string SINGLE_FLAG = "single";
        private const string ALL_FLAG = "all";
        private const string OBJECT_FLAG = "object";

        public override void Init()
        {
            m_AllResourcePackagePacker = new List<ResourcePackage>();
            m_AllResourceHandle = new Dictionary<ResourceHandleBasePacker, HashSet<IResourceUser>>();
            m_AllHandleToRelease = new List<ResourceHandleBasePacker>();

            //初始化资源系统
            YooAssets.Initialize();
        }

        /// <summary>
        /// 异步加载单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<T> LoadAssetAsync<T>(IResourceUser user, string location,
            uint priority = 0) where T : UnityEngine.Object
        {
            var package = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == package)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return default(T);
            }

            var loadedHandle = InternalGetHandle(user, location, SINGLE_FLAG);
            if (null != loadedHandle.Handle)
            {
                return (loadedHandle.Handle as AssetHandle).GetAssetObject<T>();
            }

            var handle = package.LoadAssetAsync<T>(location, priority);
            await handle.ToUniTask();

            if (null != loadedHandle.Handle)
            {
                //已经通过其他方式加载了,释放刚加载的句柄
                handle.Release();
                //使用加载好的句柄
                handle = loadedHandle.Handle as AssetHandle;
            }
            else
            {
                loadedHandle.Handle = handle;
            }

            var obj = handle.GetAssetObject<T>();

            if (!m_AllResourceHandle.ContainsKey(loadedHandle) || !m_AllResourceHandle[loadedHandle].Contains(user))
            {
                CommonLog.ResourceError($"资源使用者 {user.GetType()} 提前释放了资源");

                if (!m_AllResourceHandle.ContainsKey(loadedHandle))
                {
                    handle.Release();
                    loadedHandle.Handle = null;
                }
            }

            return obj;
        }

        /// <summary>
        /// 同步加载单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public T LoadAssetSync<T>(IResourceUser user, string location) where T : UnityEngine.Object
        {
            var package = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == package)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return default(T);
            }

            var loadedHandle = InternalGetHandle(user, location, SINGLE_FLAG);
            if (null != loadedHandle.Handle)
            {
                return (loadedHandle.Handle as AssetHandle).GetAssetObject<T>();
            }

            var handle = package.LoadAssetSync<T>(location);

            loadedHandle.Handle = handle;

            var obj = handle.GetAssetObject<T>();
            return obj;
        }

        /// <summary>
        /// 异步加载游戏物体
        /// </summary>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<GameObject> LoadGameObjectAsync(IResourceUser user, string location,
            uint priority = 0)
        {
            var package = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == package)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return null;
            }

            var loadedHandle = InternalGetHandle(user, location, OBJECT_FLAG);
            AssetHandle handle = null;
            if (null != loadedHandle.Handle)
            {
                handle = loadedHandle.Handle as AssetHandle;
            }
            else
            {
                handle = package.LoadAssetAsync<GameObject>(location, priority);
                await handle.ToUniTask();

                if (null != loadedHandle.Handle)
                {
                    //已经通过其他方式加载了,释放刚加载的句柄
                    handle.Release();
                    //使用加载好的句柄
                    handle = loadedHandle.Handle as AssetHandle;
                }
                else
                {
                    loadedHandle.Handle = handle;
                }
            }

            var instantiateOperation = handle.InstantiateAsync();
            await instantiateOperation.ToUniTask();

            if (!m_AllResourceHandle.ContainsKey(loadedHandle) || !m_AllResourceHandle[loadedHandle].Contains(user))
            {
                CommonLog.ResourceError($"资源使用者 {user.GetType()} 提前释放了资源");

                if (!m_AllResourceHandle.ContainsKey(loadedHandle))
                {
                    handle.Release();
                    loadedHandle.Handle = null;
                }
            }

            return instantiateOperation.Result;
        }

        /// <summary>
        /// 同步加载游戏物体
        /// </summary>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public GameObject LoadGameObjectSync(IResourceUser user, string location)
        {
            var package = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == package)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return null;
            }

            var loadedHandle = InternalGetHandle(user, location, OBJECT_FLAG);
            if (null != loadedHandle.Handle)
            {
                return (loadedHandle.Handle as AssetHandle).InstantiateSync();
            }

            var handle = package.LoadAssetSync<GameObject>(location);

            loadedHandle.Handle = handle;

            var result = handle.InstantiateSync();
            return result;
        }

        /// <summary>
        /// 异步加载资源包内所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <param name="progress"></param>
        /// <param name="timing"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<T[]> LoadAllAssetsAsync<T>(IResourceUser user, string location,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            uint priority = 0) where T : UnityEngine.Object
        {
            var package = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == package)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return new T[0];
            }

            var loadedHandle = InternalGetHandle(user, location, ALL_FLAG);
            if (null != loadedHandle.Handle)
            {
                return (loadedHandle.Handle as AllAssetsHandle).AllAssetObjects
                    .Select(x => x as T)
                    .ToArray();
            }

            var handle = package.LoadAllAssetsAsync<T>(location, priority);
            await handle.ToUniTask(progress, timing);

            if (null != loadedHandle.Handle)
            {
                //已经通过其他方式加载了,释放刚加载的句柄
                handle.Release();
                //使用加载好的句柄
                handle = loadedHandle.Handle as AllAssetsHandle;
            }
            else
            {
                loadedHandle.Handle = handle;
            }

            var obj = handle.AllAssetObjects
                .Select(x => x as T)
                .ToArray();

            if (!m_AllResourceHandle.ContainsKey(loadedHandle) || !m_AllResourceHandle[loadedHandle].Contains(user))
            {
                CommonLog.ResourceError($"资源使用者 {user.GetType()} 提前释放了资源");

                if (!m_AllResourceHandle.ContainsKey(loadedHandle))
                {
                    handle.Release();
                    loadedHandle.Handle = null;
                }
            }

            return obj;
        }

        /// <summary>
        /// 同步加载资源包内所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public T[] LoadAllAssetSync<T>(IResourceUser user, string location) where T : UnityEngine.Object
        {
            var package = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == package)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return new T[0];
            }

            var loadedHandle = InternalGetHandle(user, location, ALL_FLAG);
            if (null != loadedHandle.Handle)
            {
                return (loadedHandle.Handle as AllAssetsHandle).AllAssetObjects
                    .Select(x => x as T)
                    .ToArray();
            }

            var handle = package.LoadAllAssetsSync<T>(location);

            loadedHandle.Handle = handle;

            var obj = handle.AllAssetObjects
                .Select(x => x as T)
                .ToArray();
            return obj;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <param name="progress"></param>
        /// <param name="timing"></param>
        /// <param name="sceneMode"></param>
        /// <param name="suspendLoad"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask LoadSceneAsync(IResourceUser user, string location,
            IProgress<float> progress = null,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            uint priority = 0)
        {
            var package = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == package)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return;
            }

            var handle = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
            await handle.ToUniTask(progress, timing);
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <param name="sceneMode"></param>
        /// <returns></returns>
        public void LoadSceneSync(IResourceUser user, string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single)
        {
            var packer = m_AllResourcePackagePacker.Where(x => x.PackageName.Equals(user.PackageName)).FirstOrDefault();
            if (null == packer)
            {
                CommonLog.ResourceError($"资源包{user.PackageName}不存在");
                return;
            }

            packer.LoadSceneSync(location, sceneMode);
        }

        /// <summary>
        /// 释放使用者正在使用的所有资源句柄
        /// </summary>
        /// <param name="user"></param>
        public void ReleaseUsingHandle(IResourceUser user)
        {
            m_AllHandleToRelease.Clear();

            foreach (var pair in m_AllResourceHandle)
            {
                //不论是否包含,直接移除
                pair.Value.Remove(user);
                //移除当前使用者为0的句柄
                if (pair.Value.Count == 0)
                {
                    m_AllHandleToRelease.Add(pair.Key);
                }
            }

            for (int i = 0; i < m_AllHandleToRelease.Count; i++)
            {
                m_AllResourceHandle.Remove(m_AllHandleToRelease[i]);
                var handle = m_AllHandleToRelease[i].Handle;
                if (null != handle)
                {
                    if (handle is AssetHandle assetHandle)
                    {
                        assetHandle.Release();
                    }
                    else if (handle is AllAssetsHandle allAssetHandle)
                    {
                        allAssetHandle.Release();
                    }
                    else if (handle is RawFileHandle rawFileHandle)
                    {
                        rawFileHandle.Release();
                    }
                    else if (handle is SubAssetsHandle subAssetHandle)
                    {
                        subAssetHandle.Release();
                    }

                    m_AllHandleToRelease[i].Handle = null;
                }
            }

            m_AllHandleToRelease.Clear();
        }

        /// <summary>
        /// 内部获取资源句柄
        /// </summary>
        /// <param name="user"></param>
        /// <param name="location"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private ResourceHandleBasePacker InternalGetHandle(IResourceUser user, string location, string flag)
        {
            location = $"{location}_{flag}";
            var loadedHandle = m_AllResourceHandle.Keys.Where(x => x.Location.Equals(location)).FirstOrDefault();
            if (null != loadedHandle)
            {
                var loadedUsers = m_AllResourceHandle[loadedHandle];
                if (!loadedUsers.Contains(user))
                {
                    loadedUsers.Add(user);
                }
                return loadedHandle;
            }
            else
            {
                var handlePacker = new ResourceHandleBasePacker();
                handlePacker.Location = location;
                m_AllResourceHandle.Add(handlePacker, new HashSet<IResourceUser>() { user, });
                return handlePacker;
            }
        }
    }
}
