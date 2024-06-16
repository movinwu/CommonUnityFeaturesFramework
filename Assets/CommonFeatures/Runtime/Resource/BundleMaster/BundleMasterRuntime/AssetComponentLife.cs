using System;
using System.Collections.Generic;
using UnityEngine;

namespace BundleMaster
{
    public static partial class AssetComponent
    {
        /// <summary>
        /// 计时
        /// </summary>
        private static float _timer = 0;

        /// <summary>
        /// 下载进度更新器
        /// </summary>
        internal static Action<float> DownLoadAction = null;

        /// <summary>
        /// 卸载周期计时循环
        /// </summary>
        public static void Update()
        {
            
        }

        /// <summary>
        /// 游戏关闭时调用
        /// </summary>
        public static void Destroy()
        {
#if !BMWebGL
            LMTD.ThreadFactory.Destroy();
#endif
        }
    }
}