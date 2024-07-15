using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// UI层级,枚举数字对应Canvas下渲染顺序Sorting In Order的值
    /// </summary>
    public enum EUILayer : short
    {
        /// <summary>
        /// 基础层,用于放置进入游戏登录界面\热更新界面等固定界面(非热更)
        /// </summary>
        Base = 1,

        /// <summary>
        /// 浮动层,游戏中的各种界面放置在浮动界面下(热更)
        /// </summary>
        Float = 2,

        /// <summary>
        /// 遮罩层,用于阻挡用户点击(非热更)
        /// </summary>
        Mask = 3,

        #region 引导层
        /// <summary>
        /// 引导物体层3,用于引导的界面(热更),低于引导层-3
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectBelowGuide3 = 4,

        /// <summary>
        /// 引导物体层2,用于引导的界面(热更),低于引导层-2
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectBelowGuide2 = 5,

        /// <summary>
        /// 引导物体层1,用于引导的界面(热更),低于引导层-1
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectBelowGuide1 = 6,

        /// <summary>
        /// 引导层,用于引导的界面(热更)
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        Guide = 7,

        /// <summary>
        /// 引导物体层1,用于引导的界面(热更),高于引导层1
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectUpGuide1 = 8,

        /// <summary>
        /// 引导物体层2,用于引导的界面(热更),高于引导层2
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectUpGuide2 = 9,

        /// <summary>
        /// 引导物体层3,用于引导的界面(热更),高于引导层3
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectUpGuide3 = 10,

        /// <summary>
        /// 引导物体层4,用于引导的界面(热更),高于引导层4
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectUpGuide4 = 11,

        /// <summary>
        /// 引导物体层5,用于引导的界面(热更),高于引导层5
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectUpGuide5 = 12,

        /// <summary>
        /// 引导物体层6,用于引导的界面(热更),高于引导层6
        /// <para>引导时浮动界面中单独元素可能有高亮需求,预留了4-11的层级数字用于设置引导高亮</para>
        /// </summary>
        GuideObjectUpGuide6 = 13,
        #endregion 引导层

        /// <summary>
        /// 提示层,文字提示或其他提示,这个层级下的提示会被加载界面遮挡(非热更)
        /// </summary>
        Tip = 14,

        /// <summary>
        /// 加载层,用于断网重连或下载时阻挡用户点击(非热更)
        /// </summary>
        Loading = 15,

        /// <summary>
        /// 提示层,文字提示或其他提示,在加载层上方(非热更)
        /// </summary>
        Notice = 16,

        /// <summary>
        /// 调试层,cmd窗口和调试窗口(非热更)
        /// </summary>
        Debug = 17,
    }
}
