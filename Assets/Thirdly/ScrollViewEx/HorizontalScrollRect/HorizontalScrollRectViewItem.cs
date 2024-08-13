using System;
using UnityEngine;

namespace ScrollViewEx
{
    /// <summary>
    /// You should subclass this to provide fast access to any data you need to populate
    /// this item on demand.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalScrollRectViewItem : MonoBehaviour
    {
        public HorizontalScrollRect ParentRecyle { get; internal set; }

        /// <summary>
        /// 当前显示下标
        /// </summary>
        public int CurIndex { get; internal set; }

        /// <summary>
        /// 对应预制体下标
        /// </summary>
        public int PrefabIndex { get; internal set; }

        internal RectTransform RectTransform { get => this.transform as RectTransform; }
    }
}
