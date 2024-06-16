using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace BundleMaster
{
    public partial class LoadBase
    {
#if BMWebGL
        internal void LoadAssetBundle(string bundlePackageName)
        {
            AddRefCount();
            if (_loadState == LoadState.Finish)
            {
                return;
            }
            //资源没有加载过也没有正在加载就同步加载出来
            string assetBundlePath = AssetComponent.BundleFileExistPath_WebGL(bundlePackageName, AssetBundleName);
            
            if (_loadState == LoadState.Loading)
            {
                AssetLogHelper.LogError("同步加载了正在异步加载的资源, WebGL出现此错误请检查资源加载是否冲突, 否则会引起异步加载失败。资源名: " + FilePath + 
                                        "\nAssetBundle包名: " + AssetBundleName);
                using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath))
                {
                    webRequest.SendWebRequest();
                    while (!webRequest.isDone) { }
#if UNITY_2020_1_OR_NEWER
                    if (webRequest.result == UnityWebRequest.Result.Success)
#else
                    if (string.IsNullOrEmpty(webRequest.error))
#endif
                    {
                        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                        if (AssetBundle == null)
                        {
                            AssetBundle = assetBundle;
                        }
                        else
                        {
                            assetBundle.Unload(true);
                        }
                    }
                    else
                    {
                        AssetLogHelper.LogError("UnityWebRequest同步加载正在异步加载的资源 AssetBundle失败: \t" + assetBundlePath);
                    }
                }
                return;
            }

            using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath))
            {
                webRequest.SendWebRequest();
                while (!webRequest.isDone) { }
#if UNITY_2020_1_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.Success)
#else
                if (string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                    if (AssetBundle == null)
                    {
                        AssetBundle = assetBundle;
                    }
                    else
                    {
                        assetBundle.Unload(true);
                    }
                }
                else
                {
                    AssetLogHelper.LogError("UnityWebRequest加载AssetBundle失败: \t" + assetBundlePath);
                }
            }
            _loadState = LoadState.Finish;
            if (_loadProgress != null)
            {
                _loadProgress.WeqOperation = null;
            }
        }
        
        /// <summary>
        /// 异步加载LoadBase的AssetBundle
        /// </summary>
        internal async UniTask LoadAssetBundleAsync(string bundlePackageName)
        {
            AddRefCount();
            if (_loadState == LoadState.Finish)
            {
                return;
            }
            BundleRuntimeInfo bundleRuntimeInfo = AssetComponent.BundleNameToRuntimeInfo[bundlePackageName];
            string assetBundlePath = AssetComponent.BundleFileExistPath_WebGL(bundlePackageName, AssetBundleName);

            if (_loadState == LoadState.NoLoad)
            {
                _loadState = LoadState.Loading;
                await LoadBundleFinish(assetBundlePath);
                _loadState = LoadState.Finish;
                if (_loadProgress != null)
                {
                    _loadProgress.WeqOperation = null;
                }
            }
        }
        
        /// <summary>
        /// 通过路径直接加载硬盘上的AssetBundle
        /// </summary>
        private async UniTask LoadBundleFinish(string assetBundlePath)
        {
            if (_loadState == LoadState.Finish)
            {
                return;
            }
            
            using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath))
            {
                await webRequest.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.Success)
#else
                if (string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                    //判断是否还需要
                    if (_refCount > 0)
                    {
                        
                        if (AssetBundle == null)
                        {
                            AssetBundle = assetBundle;
                        }
                        else
                        {
                            assetBundle.Unload(true);
                        }
                    }
                    else
                    {
                        assetBundle.Unload(true);
                        AssetComponent.AddPreUnLoadPool(this);
                    }
                }
                else
                {
                    AssetLogHelper.LogError("UnityWebRequest加载AssetBundle失败: \t" + assetBundlePath);
                }
            }
            
        }
        
        /// <summary>
        /// 强制加载完成
        /// </summary>
        internal void ForceLoadFinish(string bundlePackageName)
        {
            if (_loadState == LoadState.Finish)
            {
                return;
            }
            string assetBundlePath = AssetComponent.BundleFileExistPath_WebGL(bundlePackageName, AssetBundleName);
            if (AssetBundle == null)
            {
                using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundlePath))
                {
                    webRequest.SendWebRequest();
                    while (!webRequest.isDone){}
#if UNITY_2020_1_OR_NEWER
                    if (webRequest.result == UnityWebRequest.Result.Success)
#else
                    if (string.IsNullOrEmpty(webRequest.error))
#endif
                    {
                        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                        if (AssetBundle == null)
                        {
                            AssetBundle = assetBundle;
                            _loadState = LoadState.Finish;
                            if (_loadProgress != null)
                            {
                                _loadProgress.WeqOperation = null;
                            }
                            if (_refCount <= 0)
                            {
                                AssetComponent.AddPreUnLoadPool(this);
                            }
                        }
                        else
                        {
                            AssetLogHelper.LogError("同步加载了正在异步加载的资源, WebGL出现此错误请检查资源加载是否冲突, 否则会引起异步加载失败。资源名: " + FilePath + 
                                                    "\nAssetBundle包名: " + AssetBundleName);
                            assetBundle.Unload(true);
                        }
                    }
                    else
                    {
                        AssetLogHelper.LogError("UnityWebRequest同步加载正在异步加载的资源 AssetBundle失败: \t" + assetBundlePath);
                    }
                }
                return;
            }
            
        }
        
        /// <summary>
        /// 打开进度统计
        /// </summary>
        internal void OpenProgress()
        {
            _loadProgress = new WebLoadProgress();
        }
        
        /// <summary>
        /// 获取当前资源加载进度
        /// </summary>
        internal float GetProgress()
        {
            if (_loadProgress == null)
            {
                AssetLogHelper.LogError("未打开进度统计无法获取进度");
                return 0;
            }
            if (_loadState == LoadState.Finish)
            {
                return 1;
            }
            if (_loadState == LoadState.NoLoad)
            {
                return 0;
            }
            return _loadProgress.GetWebProgress();
        }
        
#endif
        
    }
}
