using System;
using UnityEngine;

namespace ScrollViewEx
{
    /// <summary>
    /// You should subclass this to provide fast access to any data you need to populate
    /// this item on demand.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class VerticalScrollRectViewItem : MonoBehaviour
    {
        public VerticalScrollRect ParentRecyle { get; internal set; }

        /// <summary>
        /// ��ǰ��ʾ�±�
        /// </summary>
        public int CurIndex { get; internal set; }

        /// <summary>
        /// ��ӦԤ�����±�
        /// </summary>
        public int PrefabIndex { get; internal set; }

        internal RectTransform RectTransform { get => this.transform as RectTransform; }
    }
}
