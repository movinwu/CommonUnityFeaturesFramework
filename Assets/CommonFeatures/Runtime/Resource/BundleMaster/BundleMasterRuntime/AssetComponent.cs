using UnityEngine;
using Cysharp.Threading.Tasks;
using CommonFeatures.Log;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BundleMaster
{
    public static partial class AssetComponent
    {
        /// <summary>
        /// 同步加载(泛型)
        /// </summary>
        public static T Load<T>(string assetPath, string bundlePackageName = null) where T : UnityEngine.Object
        {
            T asset = null;
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
#if UNITY_EDITOR
                asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
                AssetLogHelper.LogError("加载资源: " + assetPath + " 失败(资源加载Develop模式只能在编辑器下运行)");
#endif
                return asset;
            }
            LoadHandler loadHandler = null;
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError(bundlePackageName + "分包没有初始化");
                return null;
            }
            
            if (!bundleRuntimeInfo.AllAssetLoadHandler.TryGetValue(assetPath, out loadHandler))
            {
                loadHandler = LoadHandlerFactory.GetLoadHandler(assetPath, bundlePackageName, true, true);
                bundleRuntimeInfo.AllAssetLoadHandler.Add(assetPath, loadHandler);
                bundleRuntimeInfo.UnLoadHandler.Add(loadHandler.UniqueId, loadHandler);
            }
            if (loadHandler.LoadState == LoadState.NoLoad)
            {
                loadHandler.Load();
                asset = loadHandler.FileAssetBundle.LoadAsset<T>(assetPath);
                loadHandler.Asset = asset;
                return asset;
            }
            else if (loadHandler.LoadState == LoadState.Loading)
            {
                loadHandler.ForceAsyncLoadFinish();
                asset = loadHandler.FileAssetBundle.LoadAsset<T>(assetPath);
                loadHandler.Asset = asset;
            }
            if (loadHandler.LoadState == LoadState.Finish)
            {
                asset = (T)loadHandler.Asset;
            }
            return asset;
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        public static UnityEngine.Object Load(string assetPath, string bundlePackageName = null)
        {
           
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
#if UNITY_EDITOR
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
#else
                UnityEngine.Object asset = null;
                AssetLogHelper.LogError("加载资源: " + assetPath + " 失败(资源加载Develop模式只能在编辑器下运行)");
#endif
                return asset;
            }
            LoadHandler loadHandler = null;
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError(bundlePackageName + "分包没有初始化");
                return null;
            }
            if (!bundleRuntimeInfo.AllAssetLoadHandler.TryGetValue(assetPath, out loadHandler))
            {
                loadHandler = LoadHandlerFactory.GetLoadHandler(assetPath, bundlePackageName, true, true);
                bundleRuntimeInfo.AllAssetLoadHandler.Add(assetPath, loadHandler);
                bundleRuntimeInfo.UnLoadHandler.Add(loadHandler.UniqueId, loadHandler);
            }
            if (loadHandler.LoadState == LoadState.NoLoad)
            {
                loadHandler.Load();
                loadHandler.Asset = loadHandler.FileAssetBundle.LoadAsset(assetPath);
                return loadHandler.Asset;
            }
            else if (loadHandler.LoadState == LoadState.Loading)
            {
                loadHandler.ForceAsyncLoadFinish();
                loadHandler.Asset = loadHandler.FileAssetBundle.LoadAsset(assetPath);
            }
            return loadHandler.Asset;
        }

        /// <summary>
        /// 异步加载(泛型)
        /// </summary>
        public static async UniTask<T> LoadAsync<T>(string assetPath, string bundlePackageName = null) where T : UnityEngine.Object
        {
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
                AssetLogHelper.LogError("加载资源: " + assetPath + " 失败(资源加载Develop模式只能在编辑器下运行)");
                return null;
#endif
            }
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError(bundlePackageName + "分包没有初始化");
                return null;
            }
            if (!bundleRuntimeInfo.AllAssetLoadHandler.TryGetValue(assetPath, out LoadHandler loadHandler))
            {
                loadHandler = LoadHandlerFactory.GetLoadHandler(assetPath, bundlePackageName, true, true);
                bundleRuntimeInfo.AllAssetLoadHandler.Add(assetPath, loadHandler);
                bundleRuntimeInfo.UnLoadHandler.Add(loadHandler.UniqueId, loadHandler);
            }
            //加载资源
            if (loadHandler.LoadState == LoadState.NoLoad)
            {
                await loadHandler.LoadAsync();
                var loadAssetAsync = loadHandler.FileAssetBundle.LoadAssetAsync<T>(assetPath);
                await UniTask.WaitUntil(() => loadAssetAsync.isDone);
                loadHandler.Asset = loadAssetAsync.asset;
            }
            return (T)loadHandler.Asset;
        }
        
        /// <summary>
        /// 异步加载
        /// </summary>
        public static async UniTask<UnityEngine.Object> LoadAsync(string assetPath, string bundlePackageName = null)
        {
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
#else
                AssetLogHelper.LogError("加载资源: " + assetPath + " 失败(资源加载Develop模式只能在编辑器下运行)");
                return null;
#endif
            }
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError(bundlePackageName + "分包没有初始化");
                return null;
            }
            if (!bundleRuntimeInfo.AllAssetLoadHandler.TryGetValue(assetPath, out LoadHandler loadHandler))
            {
                loadHandler = LoadHandlerFactory.GetLoadHandler(assetPath, bundlePackageName, true, true);
                bundleRuntimeInfo.AllAssetLoadHandler.Add(assetPath, loadHandler);
                bundleRuntimeInfo.UnLoadHandler.Add(loadHandler.UniqueId, loadHandler);
            }
            //加载资源
            if (loadHandler.LoadState == LoadState.NoLoad)
            {
                await loadHandler.LoadAsync();
                AssetBundleRequest loadAssetAsync = loadHandler.FileAssetBundle.LoadAssetAsync(assetPath);
                await UniTask.WaitUntil(() => loadAssetAsync.isDone);
                loadHandler.Asset = loadAssetAsync.asset;
            }
            return loadHandler.Asset;
        }
        
        /// <summary>
        /// 同步加载场景的AssetBundle包
        /// </summary>
        public static LoadSceneHandler LoadScene(string scenePath, string bundlePackageName = null)
        {
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            LoadSceneHandler loadSceneHandler = new LoadSceneHandler(scenePath, bundlePackageName);
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
                //Develop模式可以直接加载场景
                return loadSceneHandler;
            }
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError(bundlePackageName + "分包没有初始化");
                return null;
            }
            bundleRuntimeInfo.UnLoadHandler.Add(loadSceneHandler.UniqueId, loadSceneHandler);
            loadSceneHandler.LoadSceneBundle();
            return loadSceneHandler;
        }
        
        /// <summary>
        /// 异步加载场景的AssetBundle包
        /// </summary>
        public static async UniTask<LoadSceneHandler> LoadSceneAsync(string scenePath, string bundlePackageName = null)
        {
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            LoadSceneHandler loadSceneHandler = new LoadSceneHandler(scenePath, bundlePackageName);
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
                //Develop模式不需要加载场景
                return loadSceneHandler;
            }
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError(bundlePackageName + "分包没有初始化");
                return null;
            }
            bundleRuntimeInfo.UnLoadHandler.Add(loadSceneHandler.UniqueId, loadSceneHandler);
            await loadSceneHandler.LoadSceneBundleAsync();
            return loadSceneHandler;
        }

        /// <summary>
        /// 从一个分包里加载shader
        /// </summary>
        public static Shader LoadShader(string shaderPath, string bundlePackageName = null)
        {
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
#else
                AssetLogHelper.LogError("资源加载Develop模式只能在编辑器下运行");
                return null;
#endif
            }
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError("加载Shader没有此分包: " + bundlePackageName);
                return null;
            }
            return bundleRuntimeInfo.Shader.LoadAsset<Shader>(shaderPath);
        }
        
        /// <summary>
        /// 从一个分包里异步加载shader
        /// </summary>
        public static async UniTask<Shader> LoadShaderAsync(string shaderPath, string bundlePackageName = null)
        {
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
#else
                AssetLogHelper.LogError("资源加载Develop模式只能在编辑器下运行");
                return null;
#endif
            }
            if (bundlePackageName == null)
            {
                bundlePackageName = AssetComponentConfig.DefaultBundlePackageName;
            }
            if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                CommonLog.ResourceError("加载Shader没有此分包: " + bundlePackageName);
                return null;
            }
            
            var bundleRequest = bundleRuntimeInfo.Shader.LoadAssetAsync<Shader>(shaderPath);
            await UniTask.WaitUntil(() => bundleRequest.isDone);
            return bundleRequest.asset as Shader;
        }

        /// <summary>
        /// 获取一个已经初始化完成的分包的信息
        /// </summary>
        public static BundleRuntimeInfo GetBundleRuntimeInfo(string bundlePackageName)
        {
            
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
#if UNITY_EDITOR
                BundleRuntimeInfo devBundleRuntimeInfo;
                if (!BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out devBundleRuntimeInfo))
                {
                    devBundleRuntimeInfo = new BundleRuntimeInfo(bundlePackageName);
                    BundleNameToRuntimeInfo.Add(bundlePackageName, devBundleRuntimeInfo);
                }
                return devBundleRuntimeInfo;
#else
                AssetLogHelper.LogError("资源加载Develop模式只能在编辑器下运行");
#endif
               
            }
            if (BundleNameToRuntimeInfo.TryGetValue(bundlePackageName, out BundleRuntimeInfo bundleRuntimeInfo))
            {
                return bundleRuntimeInfo;
            }
            else
            {
                CommonLog.ResourceError("初始化的分包里没有这个分包: " + bundlePackageName);
                return null;
            }
        }
    }
}


