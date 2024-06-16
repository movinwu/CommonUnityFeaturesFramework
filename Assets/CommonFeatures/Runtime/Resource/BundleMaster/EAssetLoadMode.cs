namespace BundleMaster
{
    /// <summary>
    /// 资源加载方式枚举
    /// </summary>
    public enum EAssetLoadMode : byte
    {
        /// <summary>
        /// 从编辑器中直接加载,用于开发
        /// </summary>
        Editor,

        /// <summary>
        /// 从本地直接读取AB包资源,用于非热更打包
        /// </summary>
        LocalAB,

        /// <summary>
        /// 从远端读取下载AB包资源,用于热更打包
        /// </summary>
        RemoteAB,
    }
}
