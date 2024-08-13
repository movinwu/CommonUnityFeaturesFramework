using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrollViewEx
{
    /// <summary>
    /// 竖直滚动条
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class HorizontalScrollRect : MonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
    {
        [Header("所有item预制体")]
        [SerializeField] private HorizontalScrollRectViewItem[] m_ChildPrefab;
        [Header("所有item之间间隔")]
        [SerializeField] private float[] m_AllSpacing = new float[] { 0f, };
        [Header("左右边距")]
        [SerializeField] private float m_RightPadding;
        [SerializeField] private float m_LeftPadding;
        [Header("回收距离")]
        [SerializeField] private float m_PreAllocLength = 200;

        [Header("滚动方向")]
        [SerializeField] private EScrollDirection m_ScrollDirection;

        [Header("是否开启自动定位")]
        [SerializeField] private bool m_EnableAutoSnap;

        [Header("自动定位时间,开启自动定位后生效")]
        [SerializeField] private float m_AutoSnapTime = 0.3f;

        [Header("自动定位item对齐位置,开启自动定位后生效")]
        [SerializeField, Range(0, 1)] private float m_ItemSnapPivot;

        [Header("定位viewport对齐位置")]
        [SerializeField, Range(0, 1)] private float m_ViewportPivot;

        /// <summary>
        /// 是否是循环列表
        /// </summary>
        private bool m_IsLoop;

        /// <summary>
        /// 元素数量
        /// </summary>
        private int m_ItemCount;

        /// <summary>
        /// 当前元素位置
        /// </summary>
        private float m_CurItemPos;

        /// <summary>
        /// 当前viewport锚点位置的item数据位置
        /// </summary>
        public float CurItemPos 
        { 
            get 
            {
                if (m_IsLoop)
                {
                    return RegularPos(m_CurItemPos);
                }
                else
                {
                    return m_CurItemPos;
                }
            }
        }

        /// <summary>
        /// 当前viewport锚点位置的item数据下标
        /// </summary>
        public int CurItemIndex
        {
            get
            {
                if (m_IsLoop)
                {
                    return Mathf.FloorToInt(m_CurItemPos);
                }
                else
                {
                    return Mathf.Clamp(Mathf.FloorToInt(m_CurItemPos), 0, m_ItemCount - 1);
                }
            }
        }

        /// <summary>
        /// 当前显示的item开始下标
        /// </summary>
        private int m_CurShownStartIndex;

        /// <summary>
        /// 当前显示的item结束下标
        /// </summary>
        private int m_CurShownEndIndex;

        /// <summary>
        /// 刷新元素.
        /// </summary>
        private Action<HorizontalScrollRectViewItem> m_RefreshItemAction;

        /// <summary>
        /// 回收元素
        /// </summary>
        private Action<HorizontalScrollRectViewItem> m_RecycleItemAction;

        /// <summary>
        /// 获取子物体预制体下标
        /// </summary>
        private Func<int, int> m_GetChildItemPrefabIndex;

        /// <summary>
        /// 获取边距下标
        /// </summary>
        private Func<int, int, int> m_GetChildItemSpaceIndex;

        /// <summary>
        /// 获取item高度
        /// </summary>
        private Func<int, float> m_GetItemHeight;

        /// <summary>
        /// 当滚动条滚动时在每个元素上执行的回调
        /// </summary>
        private Action<HorizontalScrollRectViewItem, float> m_OnScrollRectValueChangeItemAction;

        /// <summary>
        /// 当滚动条滚动时执行的回调
        /// </summary>
        private Action<float> m_OnScrollRectValueChangeAction;

        private ScrollRect m_ScrollRect;

        /// <summary>
        /// 是否正在拖动
        /// </summary>
        private bool m_IsDraging;

        /// <summary>
        /// 缓存的点击数据
        /// </summary>
        PointerEventData m_CachedPointerEventData = null;

        /// <summary>
        /// 拖动时上一帧位置
        /// </summary>
        private Vector2 m_DragLastPosition;

        /// <summary>
        /// 调整后速度
        /// </summary>
        private Vector2 m_AdjustVelocity;

        /// <summary>
        /// 需要调整速度
        /// </summary>
        private bool mNeedAdjustVec = false;

        /// <summary>
        /// 上一帧content位置
        /// </summary>
        private Vector3 m_LastFrameContainerPos = Vector3.zero;

        /// <summary>
        /// 是否正在移动中
        /// </summary>
        private bool m_IsAutoMoving = false;

        /// <summary>
        /// 是否当前rect在使用中
        /// </summary>
        private bool m_IsScrollRectUsing;

        private LinkedList<HorizontalScrollRectViewItem> m_UsingItem = new LinkedList<HorizontalScrollRectViewItem>();
        private List<Queue<HorizontalScrollRectViewItem>> m_PoolingItem = new List<Queue<HorizontalScrollRectViewItem>>();

        /// <summary>
        /// 元素高度缓存
        /// </summary>
        private List<float> m_ItemHeightCache = new List<float>();

        /// <summary>
        /// 元素高度位置缓存
        /// </summary>
        private List<float> m_ItemHeightPosCache = new List<float>();

        /// <summary>
        /// content高度
        /// </summary>
        private float m_ContentHeight;

        /// <summary>
        /// viewport的rect
        /// </summary>
        private Rect m_ViewportRect;

        /// <summary>
        /// 自动滚动动画协程
        /// </summary>
        private Coroutine m_AutoScrollAnimation;

        /// <summary>
        /// 开始滚动条
        /// </summary>
        /// <param name="itemCount"></param>
        /// <param name="refreshItemAction"></param>
        /// <param name="recycleItemAction"></param>
        /// <param name="getchildItemPrefabIndex"></param>
        /// <param name="getChildItemSpacingIndex"></param>
        /// <param name="getItemHeight">获取item高度</param>
        /// <param name="initItemPos">初始化显示下标</param>
        /// <param name="onScrollRectValueChangeItemAction">当滚动条滚动时每个item上执行函数</param>
        /// <param name="onScrollRectValueChangeAction">当滚动条滚动时执行函数</param>
        /// <param name="isLoop">是否是循环列表</param>
        public void StartScrollView(
            int itemCount,
            Action<HorizontalScrollRectViewItem> refreshItemAction,
            Action<HorizontalScrollRectViewItem> recycleItemAction,
            Func<int, int> getchildItemPrefabIndex = null,
            Func<int, int, int> getChildItemSpacingIndex = null,
            Func<int, float> getItemHeight = null,
            Action<HorizontalScrollRectViewItem, float> onScrollRectValueChangeItemAction = null,
            Action<float> onScrollRectValueChangeAction = null,
            float initItemPos = 0,
            bool isLoop = false)
        {
            m_ScrollRect = GetComponent<ScrollRect>();

            m_IsLoop = isLoop;

            if (itemCount == 0)
            {
                Debug.LogError("元素数量不能为0");
                return;
            }

            //设置滑动方向
            m_ScrollRect.vertical = false;
            m_ScrollRect.horizontal = true;

            if (null != m_ScrollRect.verticalScrollbar)
            {
                m_ScrollRect.verticalScrollbar.gameObject.SetActive(false);
            }
            m_ScrollRect.verticalScrollbar = null;
            if (m_IsLoop)
            {
                if (null != m_ScrollRect.horizontalScrollbar)
                {
                    m_ScrollRect.horizontalScrollbar.gameObject.SetActive(false);
                }
                m_ScrollRect.horizontalScrollbar = null;
            }
            
            if (null == m_ChildPrefab || m_ChildPrefab.Length == 0)
            {
                Debug.LogError("没有指定预制体");
                return;
            }
            if (null == m_AllSpacing || m_AllSpacing.Length == 0)
            {
                Debug.LogError("没有指定边距");
                return;
            }

            StopAnimation();

            //第一次开启滚动条
            if (m_PoolingItem.Count == 0)
            {
                for (int i = 0; i < m_ChildPrefab.Length; i++)
                {
                    m_ChildPrefab[i].PrefabIndex = i;
                    m_PoolingItem.Add(new Queue<HorizontalScrollRectViewItem>());

                    if (m_ScrollDirection == EScrollDirection.Left2Right)
                    {
                        m_ChildPrefab[i].RectTransform.anchorMin = new Vector2(0f, 0.5f);
                        m_ChildPrefab[i].RectTransform.anchorMax = new Vector2(0f, 0.5f);
                        m_ChildPrefab[i].RectTransform.pivot = new Vector2(0f, 0.5f);
                    }
                    else
                    {
                        m_ChildPrefab[i].RectTransform.anchorMin = new Vector2(1f, 0.5f);
                        m_ChildPrefab[i].RectTransform.anchorMax = new Vector2(1f, 0.5f);
                        m_ChildPrefab[i].RectTransform.pivot = new Vector2(1f, 0.5f);
                    }
                }
            }
            else
            {
                //回收所有item
                StopScrollView();
            }

            //隐藏所有预制体
            for (int i = 0; i < m_ChildPrefab.Length; i++)
            {
                m_ChildPrefab[i].gameObject.SetActive(false);
            }

            m_RefreshItemAction = refreshItemAction;
            m_RecycleItemAction = recycleItemAction;
            m_GetChildItemPrefabIndex = getchildItemPrefabIndex;
            m_GetChildItemSpaceIndex = getChildItemSpacingIndex;
            m_GetItemHeight = getItemHeight;
            m_OnScrollRectValueChangeItemAction = onScrollRectValueChangeItemAction;
            m_OnScrollRectValueChangeAction = onScrollRectValueChangeAction;
            if (null == m_GetChildItemPrefabIndex)
            {
                m_GetChildItemPrefabIndex = DefaultGetItemIndex;
            }
            if (null == m_GetChildItemSpaceIndex)
            {
                m_GetChildItemSpaceIndex = DefaultGetSpacingIndex;
            }
            if (null == m_GetItemHeight)
            {
                m_GetItemHeight = DefaultGetItemHeight;
            }

            m_ItemCount = Mathf.Max(0, itemCount);
            if (m_IsLoop)
            {
                m_CurItemPos = initItemPos;
            }
            else
            {
                m_CurItemPos = Mathf.Clamp(initItemPos, 0, itemCount);
            }

            //更新content
            UpdateViewportRect();
            UpdateContentAnchor();
            ForceUpdateShownItems();

            m_IsScrollRectUsing = true;
        }

        /// <summary>
        /// 重置元素数量
        /// </summary>
        /// <param name="newItemCount"></param>
        public void ResetItemCount(int newItemCount)
        {
            StartScrollView(newItemCount, m_RefreshItemAction, m_RecycleItemAction, m_GetChildItemPrefabIndex, m_GetChildItemSpaceIndex, m_GetItemHeight, m_OnScrollRectValueChangeItemAction, m_OnScrollRectValueChangeAction, m_CurItemPos);
        }

        /// <summary>
        /// 默认获取元素下标函数
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private int DefaultGetItemIndex(int itemIndex) => 0;

        /// <summary>
        /// 默认获取间距下标函数
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private int DefaultGetSpacingIndex(int startIndex, int endIndex) => 0;

        /// <summary>
        /// 默认获取item高度
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private float DefaultGetItemHeight(int itemIndex)
        {
            var prefabIndex = m_GetChildItemPrefabIndex(itemIndex);
            var prefab = m_ChildPrefab[prefabIndex];
            return prefab.RectTransform.rect.width;
        }

        /// <summary>
        /// 更新viewport的Rect
        /// </summary>
        private void UpdateViewportRect()
        {
            //检查scrollrect中scrollbar的设置,不支持Visibility选项为AutoHideAndExpandViewport
            //这个选项会修改viewport的recttransform,导致第二帧才能获取到viewport的正确尺寸
            //这里可以想办法手动计算viewportRect,以解除对scrollbar的Visibility选项的限制
            if (null != m_ScrollRect.horizontalScrollbar && m_ScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport)
            {
                Debug.LogError("Vertical Scrollbar的Visibility选项不支持AutoHideAndExpandViewport");
            }

            m_ViewportRect = m_ScrollRect.viewport.rect;
        }

        /// <summary>
        /// 更新现在显示的item下标范围
        /// </summary>
        private void UpdateCurShowItemIndex()
        {
            var preStartIndex = m_CurShownStartIndex;
            var preEndIndex = m_CurShownEndIndex;

            if (m_IsLoop)
            {
                var index = Mathf.FloorToInt(m_CurItemPos);
                m_CurShownStartIndex = index;
                m_CurShownEndIndex = index;
            }
            else
            {
                var index = Mathf.Clamp(Mathf.FloorToInt(m_CurItemPos), 0, m_ItemCount - 1);
                m_CurShownStartIndex = index;
                m_CurShownEndIndex = index;
            }

            var curHeight = m_ViewportRect.width * m_ViewportPivot;

            var curItemPos = RegularPos(m_CurItemPos);
            var itemHeight = m_GetItemHeight(Mathf.FloorToInt(curItemPos));
            float limit;

            var curDown = curHeight - itemHeight * (curItemPos % 1f);
            limit = -m_PreAllocLength;
            while (curDown > limit)
            {
                //向下添加
                if (m_IsLoop)
                {
                    var endIndex = RegularIndex(m_CurShownStartIndex);
                    m_CurShownStartIndex--;
                    var startIndex = RegularIndex(m_CurShownStartIndex);
                    curDown -= m_AllSpacing[m_GetChildItemSpaceIndex(startIndex, endIndex)];
                    curDown -= m_GetItemHeight(startIndex);
                }
                else
                {
                    if (m_CurShownStartIndex > 0)
                    {
                        m_CurShownStartIndex--;
                        curDown -= m_AllSpacing[m_GetChildItemSpaceIndex(m_CurShownStartIndex, m_CurShownStartIndex + 1)];
                        curDown -= m_GetItemHeight(m_CurShownStartIndex);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (m_IsDraging || m_IsAutoMoving)
            {
                m_CurShownStartIndex = Mathf.Min(m_CurShownStartIndex, preStartIndex);
            }

            var curUp = curHeight + itemHeight * (1 - curItemPos % 1f);
            limit = m_PreAllocLength + m_ViewportRect.width;
            while (curUp < limit)
            {
                //向上添加
                if (m_IsLoop)
                {
                    var startIndex = RegularIndex(m_CurShownEndIndex);
                    m_CurShownEndIndex++;
                    var endIndex = RegularIndex(m_CurShownEndIndex);
                    curUp += m_AllSpacing[m_GetChildItemSpaceIndex(startIndex, endIndex)];
                    curUp += m_GetItemHeight(endIndex);
                }
                else
                {
                    if (m_CurShownEndIndex < m_ItemCount - 1)
                    {
                        m_CurShownEndIndex++;
                        curUp += m_AllSpacing[m_GetChildItemSpaceIndex(m_CurShownEndIndex - 1, m_CurShownEndIndex)];
                        curUp += m_GetItemHeight(m_CurShownEndIndex);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (m_IsDraging || m_IsAutoMoving)
            {
                m_CurShownEndIndex = Mathf.Max(m_CurShownEndIndex, preEndIndex);
            }
        }

        /// <summary>
        /// 规范化下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int RegularIndex(int index)
        {
            index = index % m_ItemCount;
            if (index < 0)
            {
                index += m_ItemCount;
            }
            return index;
        }

        /// <summary>
        /// 规范化位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private float RegularPos(float pos)
        {
            pos = pos % m_ItemCount;
            if (pos < 0)
            {
                pos += m_ItemCount;
            }
            return pos;
        }

        /// <summary>
        /// 更新content锚点
        /// </summary>
        private void UpdateContentAnchor()
        {
            //更新content尺寸
            var content = m_ScrollRect.content;
            if (m_ScrollDirection == EScrollDirection.Left2Right)
            {
                content.anchorMin = new Vector2(0f, 0.5f);
                content.anchorMax = new Vector2(0f, 0.5f);
                content.pivot = new Vector2(0f, 0.5f);
            }
            else
            {
                content.anchorMin = new Vector2(1f, 0.5f);
                content.anchorMax = new Vector2(1f, 0.5f);
                content.pivot = new Vector2(1f, 0.5f);
            }
        }

        /// <summary>
        /// 更新content大小
        /// </summary>
        private void UpdateContentSize()
        {
            var content = m_ScrollRect.content;

            if (m_IsLoop)
            {
                var height = 0f;
                for (int i = m_CurShownStartIndex; i < m_CurShownEndIndex; i++)
                {
                    var regularStartIndex = RegularIndex(i);
                    var regularEndIndex = RegularIndex(i + 1);
                    height += m_GetItemHeight(regularStartIndex);
                    height += m_AllSpacing[m_GetChildItemSpaceIndex(regularStartIndex, regularEndIndex)];
                }
                var regularIndex = RegularIndex(m_CurShownEndIndex);
                height += m_GetItemHeight(regularIndex);

                content.sizeDelta = new Vector2(height, m_ViewportRect.height);

                var anchoreHeight = 0f;
                var endIndex = Mathf.FloorToInt(m_CurItemPos);
                for (int i = m_CurShownStartIndex; i < endIndex; i++)
                {
                    var regularStartIndex = RegularIndex(i);
                    var regularEndIndex = RegularIndex(i + 1);
                    anchoreHeight += m_GetItemHeight(regularStartIndex);
                    anchoreHeight += m_AllSpacing[m_GetChildItemSpaceIndex(regularStartIndex, regularEndIndex)];
                }
                regularIndex = RegularIndex(endIndex);
                anchoreHeight += m_GetItemHeight(regularIndex) * (RegularPos(m_CurItemPos) - regularIndex);
                anchoreHeight -= m_ViewportRect.width * m_ViewportPivot;

                if (m_ScrollDirection == EScrollDirection.Left2Right)
                {
                    anchoreHeight = -anchoreHeight;
                }
                content.anchoredPosition = new Vector2(anchoreHeight, 0);
            }
            else
            {
                UpdateHeightCache();
                content.sizeDelta = new Vector2(m_ContentHeight, m_ViewportRect.height);
                var anchorPos = CalcContentHeightByItemPos(m_CurItemPos);
                if (anchorPos + m_ViewportRect.width > m_ContentHeight)
                {
                    anchorPos = m_ContentHeight - m_ViewportRect.width;
                }
                if (m_ScrollDirection == EScrollDirection.Left2Right)
                {
                    anchorPos = Mathf.Min(-anchorPos, 0f);
                }
                else
                {
                    anchorPos = Mathf.Max(anchorPos, 0f);
                }
                content.anchoredPosition = new Vector2(anchorPos, 0);
            }

            //更新后修正当前位置
            UpdateCurItemPos();
        }

        /// <summary>
        /// 计算item高度位置
        /// </summary>
        /// <param name="itemPos"></param>
        /// <returns></returns>
        private float CalcItemHeightByItemPos(float itemPos)
        {
            var curItemIndex = Mathf.Clamp(Mathf.FloorToInt(itemPos), 0, m_ItemCount - 1);
            var anchorPos = m_ItemHeightPosCache[curItemIndex];
            float percent = itemPos - curItemIndex;
            anchorPos += percent * m_ItemHeightCache[curItemIndex];
            return anchorPos;
        }

        private float CalcItemHeightByItemPos(float fromPos, float toPos)
        {
            if (m_IsLoop)
            {
                float height = 0f;
                float from;
                float to;
                if (fromPos > toPos)
                {
                    from = toPos;
                    to = fromPos;
                }
                else if (fromPos < toPos)
                {
                    from = fromPos;
                    to = toPos;
                }
                else
                {
                    return height;
                }

                var fromIndex = Mathf.CeilToInt(from);
                var toIndex = Mathf.FloorToInt(to);
                var firstIndex = Mathf.FloorToInt(from);
                var regularFirstIndex = RegularIndex(firstIndex);
                if (firstIndex == toIndex)
                {
                    height = m_GetItemHeight(regularFirstIndex) * (to - from);
                }
                else
                {
                    height += m_GetItemHeight(regularFirstIndex) * (fromIndex - from);
                    if (fromIndex != firstIndex)
                    {
                        var regularFromIndex = RegularIndex(fromIndex);
                        height += m_AllSpacing[m_GetChildItemSpaceIndex(regularFirstIndex, regularFromIndex)];
                    }
                    for (int i = fromIndex; i < toIndex; i++)
                    {
                        var regularIndex = RegularIndex(i);
                        var regularNextIndex = RegularIndex(i + 1);
                        height += m_GetItemHeight(regularIndex);
                        height += m_AllSpacing[m_GetChildItemSpaceIndex(regularIndex, regularNextIndex)];
                    }
                    var lastIndex = Mathf.CeilToInt(to);
                    if (toIndex == lastIndex)
                    {
                        var regularIndex = RegularIndex(toIndex - 1);
                        var regularNextIndex = RegularIndex(toIndex);
                        height -= m_AllSpacing[m_GetChildItemSpaceIndex(regularIndex, regularNextIndex)];
                    }
                    else
                    {
                        var regularIndex = RegularIndex(toIndex);
                        height += m_GetItemHeight(regularIndex) * (to - toIndex);
                    }
                }

                if (fromPos < toPos)
                {
                    height = -height;
                }

                return height;
            }
            else
            {
                var from = CalcItemHeightByItemPos(fromPos);
                var to = CalcItemHeightByItemPos(toPos);
                return to - from;
            }
        }

        /// <summary>
        /// 计算content高度位置
        /// </summary>
        /// <param name="itemPos"></param>
        /// <returns></returns>
        private float CalcContentHeightByItemPos(float itemPos)
        {
            var curItemIndex = Mathf.Clamp(Mathf.FloorToInt(itemPos), 0, m_ItemCount - 1);
            var anchorPos = m_ItemHeightPosCache[curItemIndex];
            float percent = itemPos - curItemIndex;
            anchorPos += percent * m_ItemHeightCache[curItemIndex];
            anchorPos -= m_ViewportRect.width * m_ViewportPivot;
            return anchorPos;
        }

        /// <summary>
        /// 更新显示的item集合
        /// </summary>
        private void UpdateChildItem()
        {
            if (m_IsLoop)
            {
                //没有显示的item
                if (m_UsingItem.Count == 0)
                {
                    float height = 0;
                    for (int i = m_CurShownStartIndex; i <= m_CurShownEndIndex; i++)
                    {
                        var regularCurIndex = RegularIndex(i);
                        var regularNextIndex = RegularIndex(i + 1);

                        var item = NewSingleItem(i);
                        var anchoredPos = item.RectTransform.anchoredPosition;
                        if (m_ScrollDirection == EScrollDirection.Left2Right)
                        {
                            anchoredPos.x = height;
                        }
                        else
                        {
                            anchoredPos.x = -height;
                        }
                        item.RectTransform.anchoredPosition = anchoredPos;
                        m_UsingItem.AddLast(item);

                        height += m_GetItemHeight(regularCurIndex);
                        height += m_AllSpacing[m_GetChildItemSpaceIndex(regularCurIndex, regularNextIndex)];
                    }
                }
                else
                {
                    //下方
                    var first = m_UsingItem.First;
                    while (null != first && first.Value.CurIndex > m_CurShownStartIndex)
                    {
                        var item = NewSingleItem(first.Value.CurIndex - 1);
                        m_UsingItem.AddFirst(item);
                        first = m_UsingItem.First;
                    }

                    first = m_UsingItem.First;
                    while (null != first && first.Value.CurIndex < m_CurShownStartIndex)
                    {
                        RecycleSingleItem(first.Value);
                        m_UsingItem.RemoveFirst();
                        first = m_UsingItem.First;
                    }

                    //上方
                    var last = m_UsingItem.Last;
                    while (null != last && last.Value.CurIndex < m_CurShownEndIndex)
                    {
                        var item = NewSingleItem(last.Value.CurIndex + 1);
                        m_UsingItem.AddLast(item);
                        last = m_UsingItem.Last;
                    }

                    last = m_UsingItem.Last;
                    while (null != last && last.Value.CurIndex > m_CurShownEndIndex)
                    {
                        RecycleSingleItem(last.Value);
                        m_UsingItem.RemoveLast();
                        last = m_UsingItem.Last;
                    }

                    //统一调整位置坐标
                    float height = 0;
                    var cur = m_UsingItem.First;
                    while (null != cur)
                    {
                        var item = cur.Value;
                        var anchoredPos = item.RectTransform.anchoredPosition;
                        if (m_ScrollDirection == EScrollDirection.Left2Right)
                        {
                            anchoredPos.x = height;
                        }
                        else
                        {
                            anchoredPos.x = -height;
                        }
                        item.RectTransform.anchoredPosition = anchoredPos;

                        var regularCurIndex = RegularIndex(cur.Value.CurIndex);
                        var regularNextIndex = RegularIndex(cur.Value.CurIndex + 1);
                        height += m_GetItemHeight(regularCurIndex);
                        height += m_AllSpacing[m_GetChildItemSpaceIndex(regularCurIndex, regularNextIndex)];

                        cur = cur.Next;
                    }
                }
            }
            else
            {

                //没有显示的item
                if (m_UsingItem.Count == 0)
                {
                    for (int i = m_CurShownStartIndex; i <= m_CurShownEndIndex; i++)
                    {
                        var item = NewSingleItem(i);
                        var anchoredPos = item.RectTransform.anchoredPosition;
                        var height = CalcItemHeightByItemPos(i);
                        if (m_ScrollDirection == EScrollDirection.Right2Left)
                        {
                            height = -height;
                        }
                        anchoredPos.x = height;
                        item.RectTransform.anchoredPosition = anchoredPos;
                        m_UsingItem.AddLast(item);
                    }
                }
                else
                {
                    //下方
                    var first = m_UsingItem.First;
                    while (null != first && first.Value.CurIndex > m_CurShownStartIndex)
                    {
                        var index = first.Value.CurIndex - 1;
                        var item = NewSingleItem(index);
                        var anchoredPos = item.RectTransform.anchoredPosition;
                        var height = CalcItemHeightByItemPos(index);
                        if (m_ScrollDirection == EScrollDirection.Right2Left)
                        {
                            height = -height;
                        }
                        anchoredPos.x = height;
                        item.RectTransform.anchoredPosition = anchoredPos;
                        m_UsingItem.AddFirst(item);
                        first = m_UsingItem.First;
                    }

                    first = m_UsingItem.First;
                    while (null != first && first.Value.CurIndex < m_CurShownStartIndex)
                    {
                        RecycleSingleItem(first.Value);
                        m_UsingItem.RemoveFirst();
                        first = m_UsingItem.First;
                    }

                    //上方
                    var last = m_UsingItem.Last;
                    while (null != last && last.Value.CurIndex < m_CurShownEndIndex)
                    {
                        var index = last.Value.CurIndex + 1;
                        var item = NewSingleItem(index);
                        var anchoredPos = item.RectTransform.anchoredPosition;
                        var height = CalcItemHeightByItemPos(index);
                        if (m_ScrollDirection == EScrollDirection.Right2Left)
                        {
                            height = -height;
                        }
                        anchoredPos.x = height;
                        item.RectTransform.anchoredPosition = anchoredPos;
                        m_UsingItem.AddLast(item);
                        last = m_UsingItem.Last;
                    }

                    last = m_UsingItem.Last;
                    while (null != last && last.Value.CurIndex > m_CurShownEndIndex)
                    {
                        RecycleSingleItem(last.Value);
                        m_UsingItem.RemoveLast();
                        last = m_UsingItem.Last;
                    }
                }
            }
        }

        private void UpdateHeightCache()
        {
            //更新高度缓存
            m_ItemHeightCache.Clear();
            m_ItemHeightPosCache.Clear();
            m_ContentHeight = 0;
            if (m_ScrollDirection == EScrollDirection.Left2Right)
            {
                m_ContentHeight += m_LeftPadding;
            }
            else
            {
                m_ContentHeight += m_RightPadding;
            }
            for (int i = 0; i < m_ItemCount; i++)
            {
                var height = m_GetItemHeight(i);

                m_ItemHeightCache.Add(height);
                m_ItemHeightPosCache.Add(m_ContentHeight);

                m_ContentHeight += height;
                if (i != m_ItemCount - 1)
                {
                    var paddingIndex = m_GetChildItemSpaceIndex(i, i + 1);
                    m_ContentHeight += m_AllSpacing[paddingIndex];
                }
            }
            if (m_ScrollDirection == EScrollDirection.Left2Right)
            {
                m_ContentHeight += m_RightPadding;
            }
            else
            {
                m_ContentHeight += m_LeftPadding;
            }
        }

        /// <summary>
        /// 更新当前item位置
        /// </summary>
        private void UpdateCurItemPos()
        {
            var contentPos = m_ScrollRect.content.anchoredPosition.x;
            if (m_ScrollDirection == EScrollDirection.Left2Right)
            {
                contentPos = -contentPos;
            }
            //当前高度
            var curHeight = contentPos + m_ViewportRect.width * m_ViewportPivot;

            if (m_IsLoop)
            {
                var curItem = m_UsingItem.First;
                while (curHeight > 0 && null != curItem)
                {
                    var regularItemIndex = RegularIndex(curItem.Value.CurIndex);
                    var nextRegularItemIndex = RegularIndex(curItem.Value.CurIndex + 1);
                    var itemHeight = m_GetItemHeight(regularItemIndex);
                    curHeight -= itemHeight;

                    if (curHeight <= 0)
                    {
                        curHeight += itemHeight;
                        m_CurItemPos = curItem.Value.CurIndex + curHeight / itemHeight;
                        break;
                    }
                    var spaceHeight = m_AllSpacing[m_GetChildItemSpaceIndex(regularItemIndex, nextRegularItemIndex)];
                    curHeight -= spaceHeight;
                    if (curHeight <= 0)
                    {
                        m_CurItemPos = curItem.Value.CurIndex + 1;
                        break;
                    }

                    curItem = curItem.Next;
                    if (null == curItem)
                    {
                        m_CurItemPos = curItem.Value.CurIndex + 1;
                    }
                }
            }
            else
            {
                if (curHeight < 0)
                {
                    m_CurItemPos = curHeight / m_ItemHeightCache[0];
                }
                else if (curHeight > m_ContentHeight)
                {
                    m_CurItemPos = m_ItemCount + (curHeight - m_ContentHeight) / m_ItemHeightCache[m_ItemHeightCache.Count - 1];
                }
                else
                {
                    for (int i = 0; i < m_ItemHeightPosCache.Count; i++)
                    {
                        //找到了当前位置
                        if (m_ItemHeightPosCache[i] > curHeight)
                        {
                            if (i == 0)
                            {
                                m_CurItemPos = 0f;
                            }
                            else
                            {
                                var itemUp = m_ItemHeightPosCache[i - 1] + m_ItemHeightCache[i - 1];
                                if (curHeight > itemUp)
                                {
                                    m_CurItemPos = i;
                                }
                                else
                                {
                                    m_CurItemPos = i - 1 + (curHeight - m_ItemHeightPosCache[i - 1]) / m_ItemHeightCache[i - 1];
                                }
                            }
                            return;
                        }
                    }
                    //都没有找到
                    var lastItemUp = m_ItemHeightPosCache[m_ItemCount - 1] + m_ItemHeightCache[m_ItemCount - 1];
                    if (curHeight > lastItemUp)
                    {
                        m_CurItemPos = m_ItemCount;
                    }
                    else
                    {
                        m_CurItemPos = m_ItemCount - 1 + (curHeight - m_ItemHeightPosCache[m_ItemCount - 1]) / m_ItemHeightCache[m_ItemCount - 1];
                    }
                }
            }
        }

        /// <summary>
        /// 停止滚动条
        /// </summary>
        public void StopScrollView()
        {
            while (m_UsingItem.Count > 0)
            {
                RecycleSingleItem(m_UsingItem.First.Value);
                m_UsingItem.RemoveFirst();
            }
            m_ItemCount = 0;
            m_RefreshItemAction = null;
            m_RecycleItemAction = null;
            m_GetChildItemSpaceIndex = null;
            m_GetChildItemPrefabIndex = null;

            m_IsScrollRectUsing = false;
        }

        /// <summary>
        /// 强制更新所有显示的元素
        /// </summary>
        public void ForceUpdateShownItems()
        {
            UpdateCurShowItemIndex();
            UpdateContentSize();
            UpdateChildItem();
        }

        /// <summary>
        /// 改变元素数量
        /// </summary>
        /// <param name="newItemCount"></param>
        public void ChangeItemCount(int newItemCount)
        {
            StartScrollView(newItemCount, m_RefreshItemAction, m_RecycleItemAction, m_GetChildItemPrefabIndex, m_GetChildItemSpaceIndex, m_GetItemHeight, m_OnScrollRectValueChangeItemAction, m_OnScrollRectValueChangeAction, m_CurItemPos);
        }

        /// <summary>
        /// 新创建单个元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private HorizontalScrollRectViewItem NewSingleItem(int index)
        {
            var regularIndex = RegularIndex(index);
            var prefabIndex = m_GetChildItemPrefabIndex(regularIndex);
            HorizontalScrollRectViewItem item = null;
            if (m_PoolingItem[prefabIndex].Count > 0)
            {
                item = m_PoolingItem[prefabIndex].Dequeue();
            }
            else
            {
                item = GameObject.Instantiate(m_ChildPrefab[prefabIndex].gameObject, m_ScrollRect.content).GetComponent<HorizontalScrollRectViewItem>();
            }
            item.PrefabIndex = prefabIndex;
            item.CurIndex = index;
            item.ParentRecyle = this;
            m_RefreshItemAction?.Invoke(item);
            item.gameObject.SetActive(true);
            return item;
        }

        /// <summary>
        /// 回收单个元素
        /// </summary>
        private void RecycleSingleItem(HorizontalScrollRectViewItem item)
        {
            item.gameObject.SetActive(false);
            m_RecycleItemAction?.Invoke(item);
            m_PoolingItem[item.PrefabIndex].Enqueue(item);
        }

        /// <summary>
        /// 瞬间跳跃到指定位置
        /// </summary>
        /// <param name="position">从 0-元素个数 取值</param>
        /// <param name="autoSnap">跳跃完成后是否自动定位</param>
        public void JumpTo(float position, bool autoSnap = false)
        {
            if (!m_IsLoop)
            {
                position = RegularPos(position);
            }

            if (!m_IsLoop && m_ContentHeight <= m_ViewportRect.width)
            {
                return;
            }

            //立即中断当前自动滚动的动画
            StopAnimation();
            m_ScrollRect.StopMovement();

            StartScrollView(m_ItemCount, m_RefreshItemAction, m_RecycleItemAction, m_GetChildItemPrefabIndex, m_GetChildItemSpaceIndex, m_GetItemHeight, m_OnScrollRectValueChangeItemAction, m_OnScrollRectValueChangeAction, position);
            if (autoSnap)
            {
                StartAutoSnap();
            }
        }

        /// <summary>
        /// 指定速度滚动到指定位置
        /// </summary>
        /// <param name="targetPos">滚动的下标位置</param>
        /// <param name="speed"></param>
        /// <param name="blockRaycasts">是否屏蔽点击</param>
        /// <param name="onScrollEnd">当滚动完毕回调</param>
        /// <param name="autoSnap">滚动完毕后是否自动定位</param>
        public void ScrollToBySpeed(float targetPos, float speed, bool blockRaycasts = true, Action onScrollEnd = null, bool autoSnap = false)
        {
            if (speed <= 0)
            {
                onScrollEnd?.Invoke();
                return;
            }

            if (!m_IsLoop && m_ContentHeight <= m_ViewportRect.width)
            {
                onScrollEnd?.Invoke();
                return;
            }

            StopAnimation();
            m_ScrollRect.StopMovement();

            //标准化位置
            var distance = CalcItemHeightByItemPos(m_CurItemPos, targetPos);

            //计算滚动
            if (Mathf.Abs(distance) > 0.1f)
            {
                distance = Mathf.Abs(distance);

                var time = distance / speed;
                StartAnimation(targetPos, time, onScrollEnd, autoSnap);
                if (blockRaycasts)
                {
                    BlockRaycasts();
                }
            }
            else
            {
                onScrollEnd?.Invoke();
            }
        }

        /// <summary>
        /// 指定时间滚动到指定位置
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="time"></param>
        /// <param name="blockRaycasts">是否屏蔽射线检测</param>
        /// <param name="onScrollEnd">当滚动完毕回调</param>
        /// <param name="autoSnap">滚动完毕后是否自动定位</param>
        public void ScrollToByTime(float targetPos, float time, bool blockRaycasts = false, Action onScrollEnd = null, bool autoSnap = false)
        {
            if (time <= 0)
            {
                onScrollEnd?.Invoke();
                return;
            }

            StopAnimation();
            m_ScrollRect.StopMovement();

            //标准化位置
            var distance = CalcItemHeightByItemPos(m_CurItemPos, targetPos);

            //计算滚动
            if (Mathf.Abs(distance) > 0.1f)
            {
                distance = Mathf.Abs(distance);

                StartAnimation(targetPos, time, onScrollEnd, autoSnap);
                if (!blockRaycasts)
                {
                    BlockRaycasts();
                }
            }
            else
            {
                onScrollEnd?.Invoke();
            }
        }

        /// <summary>
        /// 关闭点击检测
        /// </summary>
        private void BlockRaycasts()
        {
            var canvasGroup = this.GetComponent<CanvasGroup>();
            if (null == canvasGroup)
            {
                canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 开启点击检测
        /// </summary>
        private void UnblockRaycasts()
        {
            var canvasGroup = this.GetComponent<CanvasGroup>();
            if (null != canvasGroup)
            {
                canvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// 开启自动滚动动画
        /// </summary>
        /// <param name="target"></param>
        /// <param name="time"></param>
        /// <param name="onScrollEnd"></param>
        /// <param name="autoSnap"></param>
        private void StartAnimation(float target, float time, Action onScrollEnd, bool autoSnap)
        {
            m_AutoScrollAnimation = StartCoroutine(AutoMoveCoroutine(target, time, onScrollEnd, autoSnap));
        }

        /// <summary>
        /// 自动滚动协程
        /// </summary>
        /// <param name="target"></param>
        /// <param name="time"></param>
        /// <param name="onScrollEnd"></param>
        /// <param name="autoSnap"></param>
        /// <returns></returns>
        private IEnumerator AutoMoveCoroutine(float target, float time, Action onScrollEnd, bool autoSnap)
        {
            float timer = 0;
            m_IsAutoMoving = true;
            while(timer < time)
            {
                yield return new WaitForEndOfFrame();
                var deltaTime = Time.deltaTime;
                var curTimer = timer;
                timer += deltaTime;
                if (timer > time)
                {
                    timer = time;
                }
                UpdateCurItemPos();
                var cur = m_CurItemPos;
                var distance = CalcItemHeightByItemPos(cur, target);
                if ((m_IsLoop && m_ScrollDirection == EScrollDirection.Right2Left)
                    || (!m_IsLoop && m_ScrollDirection == EScrollDirection.Left2Right))
                {
                    distance = -distance;
                }
                var pos = m_ScrollRect.content.anchoredPosition;
                var moveDeltaDistance = distance * (timer - curTimer) / (time - curTimer);
                pos.x += moveDeltaDistance;
                m_ScrollRect.content.anchoredPosition = pos;
            }

            m_AutoScrollAnimation = null;
            UnblockRaycasts();
            m_IsAutoMoving = false;

            onScrollEnd?.Invoke();

            if (autoSnap)
            {
                StartAutoSnap();
            }
        }

        /// <summary>
        /// 关闭自动滚动动画
        /// </summary>
        private void StopAnimation()
        {
            if (null != m_AutoScrollAnimation)
            {
                StopCoroutine(m_AutoScrollAnimation);
                m_AutoScrollAnimation = null;
            }
            UnblockRaycasts();

            m_IsAutoMoving = false;
        }

        /// <summary>
        /// 开始自动定位
        /// </summary>
        private void StartAutoSnap()
        {
            if (!m_EnableAutoSnap)
            {
                return;
            }

            //计算是否回弹,如果回弹,不走自动定位
            if (IsInElastic())
            {
                return;
            }

            StopAnimation();
            m_ScrollRect.StopMovement(); 

            var itemPos = CurItemIndex + m_ItemSnapPivot;
            if (m_AutoSnapTime <= 0)
            {
                JumpTo(itemPos);
            }
            else
            {
                ScrollToByTime(
                targetPos: itemPos,
                time: m_AutoSnapTime,
                blockRaycasts: false,
                autoSnap: false
                );
            }
        }

        /// <summary>
        /// 是否正在回弹
        /// </summary>
        /// <returns></returns>
        private bool IsInElastic()
        {
            if (!m_IsLoop && m_ScrollRect.movementType == ScrollRect.MovementType.Elastic)
            {
                var contentPos = m_ScrollRect.content.anchoredPosition.x;
                if (m_ScrollDirection == EScrollDirection.Left2Right)
                {
                    contentPos = -contentPos;
                }
                if (contentPos + m_ViewportRect.width > m_ContentHeight || contentPos < 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            m_IsDraging = false;
            m_CachedPointerEventData = null;

            StartAutoSnap();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            m_IsDraging = true;

            CacheDragPointerEventData(eventData);

            m_DragLastPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            CacheDragPointerEventData(eventData);

            m_DragLastPosition = eventData.position;
        }

        private void CacheDragPointerEventData(PointerEventData eventData)
        {
            if (m_CachedPointerEventData == null)
            {
                m_CachedPointerEventData = new PointerEventData(EventSystem.current);
            }
            m_CachedPointerEventData.button = eventData.button;
            m_CachedPointerEventData.position = eventData.position;
            m_CachedPointerEventData.pointerPressRaycast = eventData.pointerPressRaycast;
            m_CachedPointerEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
        }

        private void Update()
        {
            if (!m_IsScrollRectUsing)
            {
                return;
            }
            if (mNeedAdjustVec)
            {
                mNeedAdjustVec = false;
                if (m_ScrollRect.velocity.x * m_AdjustVelocity.x > 0)
                {
                    m_ScrollRect.velocity = m_AdjustVelocity;
                }

            }

            var preItemPos = m_CurItemPos;
            //更新当前位置
            UpdateCurItemPos();

            m_AdjustVelocity = (m_ScrollRect.content.localPosition - m_LastFrameContainerPos) / Time.deltaTime;

            if (!IsInElastic())
            {
                UpdateCurShowItemIndex();
                if (m_IsLoop)
                {
                    UpdateContentSize();
                }
                UpdateChildItem();
                if (m_IsDraging)
                {
                    m_ScrollRect.OnBeginDrag(m_CachedPointerEventData);
                    m_ScrollRect.Rebuild(CanvasUpdate.PostLayout);
                    m_ScrollRect.velocity = m_AdjustVelocity;
                    mNeedAdjustVec = true;
                }
            }

            m_LastFrameContainerPos = m_ScrollRect.content.localPosition;

            if (preItemPos == m_CurItemPos)
            {
                return;
            }

            //执行回调
            m_OnScrollRectValueChangeAction?.Invoke(m_CurItemPos);
            if (m_UsingItem.Count > 0 && null != m_OnScrollRectValueChangeItemAction)
            {
                var cur = m_UsingItem.First;
                while (null != cur)
                {
                    m_OnScrollRectValueChangeItemAction(cur.Value, m_CurItemPos);
                    cur = cur.Next;
                }
            }
        }

        private enum EScrollDirection : byte
        {
            Left2Right,

            Right2Left,
        }
    }
}
