using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// MenuItem 元素priority 统一管理
    /// </summary>
    public class CMenuItemPriority
    {
        /// <summary>
        /// 数据表导表窗口
        /// </summary>
        public const int DataTableWindow = 100;

        /// <summary>
        /// 数据表一键导表
        /// </summary>
        public const int DataTableOneKey = 101;

        /// <summary>
        /// proto代码生成窗口
        /// </summary>
        public const int Proto2CSharpWindow = 200;

        /// <summary>
        /// proto代码一键生成
        /// </summary>
        public const int Proto2CSharpOneKey = 201;

        /// <summary>
        /// 资源清单文件生成窗口
        /// </summary>
        public const int ResourceManifestWindow = 300;

        /// <summary>
        /// 资源清单文件一键生成
        /// </summary>
        public const int ResourceManifestOneKey = 301;

        /// <summary>
        /// AB包资源打包窗口
        /// </summary>
        public const int AssetBundlePackerWindow = 400;

        /// <summary>
        /// AB包资源一键打包
        /// </summary>
        public const int AssetBundlePackerOneKey = 401;
    }
}
