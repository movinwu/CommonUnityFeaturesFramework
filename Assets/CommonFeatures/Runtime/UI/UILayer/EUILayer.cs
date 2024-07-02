using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// UI层级,层级数字对应Canvas下渲染顺序Sorting In Order的值
    /// </summary>
    public enum EUILayer : sbyte
    {
        /// <summary>
        /// 基础层,用于放置进入游戏登录界面\热更新界面等固定界面(非热更)
        /// </summary>
        Base = -3,

        /// <summary>
        /// 浮动层,游戏中的各种界面放置在浮动界面下(热更)
        /// </summary>
        Float = -2,

        /// <summary>
        /// 遮罩层,用于阻挡用户点击(非热更)
        /// </summary>
        Mask = -1,

        /// <summary>
        /// 引导层,用于引导的界面(热更)
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了1-9的层级数字用于设置引导高亮</para>
        /// </summary>
        Guide = 0,

        /// <summary>
        /// 提示层,文字提示或其他提示,这个层级下的提示会被加载界面遮挡(非热更)
        /// </summary>
        Tip = 10,

        /// <summary>
        /// 加载层,用于断网重连或下载时阻挡用户点击(非热更)
        /// </summary>
        Loading = 11,

        /// <summary>
        /// 提示层,文字提示或其他提示,在加载层上方(非热更)
        /// </summary>
        Notice = 12,

        /// <summary>
        /// 调试层,cmd窗口和调试窗口(非热更)
        /// </summary>
        Debug = 13,
    }
}
