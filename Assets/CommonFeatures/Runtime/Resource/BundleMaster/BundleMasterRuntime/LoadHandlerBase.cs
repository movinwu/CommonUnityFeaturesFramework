using System.Collections.Generic;
using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BundleMaster
{
    public abstract class LoadHandlerBase
    {
        /// <summary>
        /// 是否是通过路径加载的
        /// </summary>
        internal bool HaveHandler = true;
        
        /// <summary>
        /// 对应的加载的资源的路径
        /// </summary>
        protected string AssetPath;

        /// <summary>
        /// 唯一ID
        /// </summary>
        public uint UniqueId = 0;
        
        /// <summary>
        /// 所属分包的名称
        /// </summary>
        protected string BundlePackageName;
        
        /// <summary>
        /// 是否卸载标记位
        /// </summary>
        protected bool UnloadFinish = false;
        
        /// <summary>
        /// File文件AssetBundle的引用
        /// </summary>
        public AssetBundle FileAssetBundle;
    
        /// <summary>
        /// 资源所在的LoadBase包
        /// </summary>
        protected LoadBase LoadBase = null;
    
        /// <summary>
        /// 依赖的Bundle包
        /// </summary>
        protected readonly List<LoadDepend> LoadDepends = new List<LoadDepend>();
    
        /// <summary>
        /// 依赖的其它File包
        /// </summary>
        protected readonly List<LoadFile> LoadDependFiles = new List<LoadFile>();
        
        /// <summary>
        /// 依赖的其它Group包
        /// </summary>
        protected readonly List<LoadGroup> LoadDependGroups = new List<LoadGroup>();
        
        /// <summary>
        /// 加载计数器(负责完成所有依赖的Bundle加载完成)
        /// </summary>
        protected async UniTask LoadAsyncLoader(LoadBase loadBase)
        {
            await loadBase.LoadAssetBundleAsync(BundlePackageName);
        }
        
        public void UnLoad()
        {
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
                return;
            }
            if (UnloadFinish)
            {
                CommonLog.ResourceError(AssetPath + "已经卸载完了");
                return;
            }
            AssetComponent.BundleNameToRuntimeInfo[BundlePackageName].UnLoadHandler.Remove(UniqueId);
            //减少引用数量
            ClearAsset();
            UnloadFinish = true;
        }
        
        /// <summary>
        /// 子类需要实现清理资源引用的逻辑
        /// </summary>
        protected abstract void ClearAsset();
    }
}