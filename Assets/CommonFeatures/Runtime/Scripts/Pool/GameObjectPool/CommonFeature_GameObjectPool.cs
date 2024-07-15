using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.Pool
{
    /// <summary>
    /// 通用功能_物体缓存池
    /// </summary>
    public class CommonFeature_GameObjectPool : CommonFeature
    {
        [Header("缓存的元素持有时间,超时自动释放")]
        [SerializeField] internal float HoldDuration = 60f;

        [Header("缓存元素清理时间间隔")]
        [SerializeField] private float ClearSpace = 10f;

        /// <summary>
        /// 自由池item
        /// </summary>
        private List<ReferencePoolItem> m_FreeItemList = new List<ReferencePoolItem>();

        /// <summary>
        /// 缓存池
        /// </summary>
        private Dictionary<(GameObject origin, Transform parent), (List<ReferencePoolItem> usingList, List<ReferencePoolItem> freeList)> m_PoolDic = new Dictionary<(GameObject origin, Transform parent), (List<ReferencePoolItem> usingList, List<ReferencePoolItem> freeList)>();

        /// <summary>
        /// 清理时间间隔计时器
        /// </summary>
        private float m_ClearSpaceTimer;

        private Transform m_PoolParent;

        public override UniTask Init()
        {
            //清理间隔时间不能小于持有时间
            ClearSpace = Mathf.Clamp(ClearSpace, 0, Mathf.Max(0, HoldDuration));

            m_ClearSpaceTimer = 0f;

            //初始化池父级
            if (null == m_PoolParent)
            {
                m_PoolParent = new GameObject("PoolParent").transform;
            }
            m_PoolParent.parent = this.transform;
            m_PoolParent.localPosition = Vector3.zero;
            m_PoolParent.localRotation = Quaternion.identity;
            m_PoolParent.localScale = Vector3.one;
            m_PoolParent.gameObject.SetActive(false);

            return base.Init();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (ClearSpace > 0)
            {
                var deltaTime = Time.unscaledDeltaTime;
                m_ClearSpaceTimer += deltaTime;
                if (m_ClearSpaceTimer >= ClearSpace)
                {
                    m_ClearSpaceTimer -= ClearSpace;

                    //清理长时间没有使用的item
                    var realTime = Time.unscaledTime;
                    foreach (var listPair in m_PoolDic.Values)
                    {
                        var list = listPair.freeList;
                        for (int i = list.Count - 1; i >= 0; i--)
                        {
                            if (list[i].IsDestroy(realTime))
                            {
                                var poolItem = list[i];
                                GameObject.Destroy(poolItem.Item);
                                poolItem.Item = null;
                                m_FreeItemList.Add(poolItem);
                                list.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从池中取用
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="parent"></param>
        /// <param name="setIdetityTransform">设置标准transform参数</param>
        /// <returns></returns>
        public GameObject Acquire(GameObject origin, Transform parent, bool setIdetityTransform = true)
        {
            if (null == origin || null == parent)
            {
                return null;
            }
            var pair = (origin, parent);
            if (!m_PoolDic.ContainsKey(pair))
            {
                m_PoolDic.Add(pair, (new List<ReferencePoolItem>(), new List<ReferencePoolItem>()));
            }

            var listPair = m_PoolDic[pair];
            //还有没有使用的item
            if (listPair.freeList.Count > 0)
            {
                var poolItem = listPair.freeList[listPair.freeList.Count - 1];
                listPair.freeList.RemoveAt(listPair.freeList.Count - 1);
                poolItem.Item.transform.SetParent(parent);
                if (setIdetityTransform)
                {
                    poolItem.Item.transform.localPosition = Vector3.zero;
                    poolItem.Item.transform.localRotation = Quaternion.identity;
                    poolItem.Item.transform.localScale = Vector3.one;
                }
                listPair.usingList.Add(poolItem);
                return poolItem.Item;
            }
            //所有item已使用
            else
            {
                var go = GameObject.Instantiate(origin);
                ReferencePoolItem poolItem = null;
                if (m_FreeItemList.Count > 0)
                {
                    poolItem = m_FreeItemList[m_FreeItemList.Count - 1];
                    m_FreeItemList.RemoveAt(m_FreeItemList.Count - 1);
                }
                else
                {
                    poolItem = new ReferencePoolItem();
                }
                poolItem.Item = go;
                go.transform.SetParent(parent);
                if (setIdetityTransform)
                {
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                }
                listPair.usingList.Add(poolItem);
                return go;
            }
        }

        /// <summary>
        /// 从池中取用
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="parent"></param>
        /// <param name="count">取用数量</param>
        /// <param name="backAll">回收所有已使用的,为true时会将所有正在使用的gameobject自动回收</param>
        /// <param name="setIdetityTransform">设置标准transform参数</param>
        /// <returns></returns>
        public GameObject[] Acquire(GameObject origin, Transform parent, int count = 1, bool backAll = false, bool setIdetityTransform = true)
        {
            if (null == origin || null == parent || count < 0)
            {
                return null;
            }
            var pair = (origin, parent);
            if (!m_PoolDic.ContainsKey(pair))
            {
                m_PoolDic.Add(pair, (new List<ReferencePoolItem>(), new List<ReferencePoolItem>()));
            }
            var listPair = m_PoolDic[pair];
            if (backAll)
            {
                //存储所有item
                var allPoolItem = new ReferencePoolItem[listPair.usingList.Count + listPair.freeList.Count];
                for (int i = 0; i < listPair.usingList.Count; i++)
                {
                    allPoolItem[i] = listPair.usingList[i];
                }
                for (int i = 0; i < listPair.freeList.Count; i++)
                {
                    allPoolItem[i + listPair.usingList.Count] = listPair.freeList[i];
                }
                listPair.usingList.Clear();
                listPair.freeList.Clear();

                //重新分配
                for (int i = 0; i < count; i++)
                {
                    //不足,需要新建
                    if (allPoolItem.Length <= i)
                    {
                        var go = GameObject.Instantiate(origin);
                        ReferencePoolItem item = null;
                        if (m_FreeItemList.Count > 0)
                        {
                            item = m_FreeItemList[m_FreeItemList.Count - 1];
                            m_FreeItemList.RemoveAt(m_FreeItemList.Count - 1);
                        }
                        else
                        {
                            item = new ReferencePoolItem();
                        }
                        go.transform.SetParent(parent);
                        if (setIdetityTransform)
                        {
                            go.transform.localPosition = Vector3.zero;
                            go.transform.localRotation = Quaternion.identity;
                            go.transform.localScale = Vector3.one;
                        }
                        item.Item = go;
                        listPair.usingList.Add(item);
                    }
                    else
                    {
                        var item = allPoolItem[i];
                        var go = item.Item;
                        go.transform.SetParent(parent);
                        if (setIdetityTransform)
                        {
                            go.transform.localPosition = Vector3.zero;
                            go.transform.localRotation = Quaternion.identity;
                            go.transform.localScale = Vector3.one;
                        }
                        listPair.usingList.Add(item);
                    }
                }
                var time = Time.unscaledTime;
                for (int i = count; i < allPoolItem.Length; i++)
                {
                    allPoolItem[i].AddTime = time;
                    allPoolItem[i].Item.transform.SetParent(m_PoolParent);
                    listPair.freeList.Add(allPoolItem[i]);
                }
                return listPair.usingList.Select(x => x.Item).ToArray();
            }
            else
            {
                var result = new GameObject[count];
                for (int i = 0; i < count; i++)
                {
                    //不足,需要新建
                    if (listPair.freeList.Count == 0)
                    {
                        var go = GameObject.Instantiate(origin);
                        ReferencePoolItem item = null;
                        if (m_FreeItemList.Count > 0)
                        {
                            item = m_FreeItemList[m_FreeItemList.Count - 1];
                            m_FreeItemList.RemoveAt(m_FreeItemList.Count - 1);
                        }
                        else
                        {
                            item = new ReferencePoolItem();
                        }
                        go.transform.SetParent(parent);
                        if (setIdetityTransform)
                        {
                            go.transform.localPosition = Vector3.zero;
                            go.transform.localRotation = Quaternion.identity;
                            go.transform.localScale = Vector3.one;
                        }
                        item.Item = go;
                        listPair.usingList.Add(item);
                        result[i] = go;
                    }
                    else
                    {
                        var item = listPair.freeList[listPair.freeList.Count - 1];
                        listPair.freeList.RemoveAt(listPair.freeList.Count - 1);
                        listPair.usingList.Add(item);
                        var go = item.Item;
                        go.transform.SetParent(parent);
                        if (setIdetityTransform)
                        {
                            go.transform.localPosition = Vector3.zero;
                            go.transform.localRotation = Quaternion.identity;
                            go.transform.localScale = Vector3.one;
                        }
                        result[i] = go;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="item"></param>
        public void Back(GameObject origin, Transform parent, GameObject item)
        {
            if (null == origin || null == parent)
            {
                return;
            }
            //元素没有持有时间,无需归还
            if (HoldDuration <= 0)
            {
                return;
            }
            var pair = (origin, parent);
            if (!m_PoolDic.ContainsKey(pair))
            {
                m_PoolDic.Add(pair, (new List<ReferencePoolItem>(), new List<ReferencePoolItem>()));
            }

            var listPair = m_PoolDic[pair];
            //查找已归还元素
            for (int i = 0; i < listPair.freeList.Count; i++)
            {
                if (listPair.freeList[i].Item.Equals(item))
                {
                    CommonLog.LogWarning($"物品 {item.name} 已经归还,不能重复归还");
                    return;
                }
            }

            var time = Time.unscaledTime;
            //查找要归还的元素
            for (int i = 0; i < listPair.usingList.Count; i++)
            {
                if (listPair.usingList[i].Item.Equals(item))
                {
                    var usingItem = listPair.usingList[i];
                    usingItem.AddTime = time;
                    usingItem.Item.transform.SetParent(m_PoolParent);
                    listPair.freeList.Add(usingItem);
                    return;
                }
            }

            //新元素
            ReferencePoolItem poolItem = null;
            if (m_FreeItemList.Count > 0)
            {
                poolItem = m_FreeItemList[m_FreeItemList.Count - 1];
                m_FreeItemList.RemoveAt(m_FreeItemList.Count - 1);
            }
            else
            {
                poolItem = new ReferencePoolItem();
            }
            poolItem.Item = item;
            poolItem.AddTime = time;
            item.transform.SetParent(m_PoolParent);
            listPair.freeList.Add(poolItem);
            return;
        }

        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="items"></param>
        public void Back(GameObject origin, Transform parent, GameObject[] items)
        {
            if (null == origin || null == parent)
            {
                return;
            }
            //元素没有持有时间,无需归还
            if (HoldDuration <= 0)
            {
                return;
            }
            var pair = (origin, parent);
            if (!m_PoolDic.ContainsKey(pair))
            {
                m_PoolDic.Add(pair, (new List<ReferencePoolItem>(), new List<ReferencePoolItem>()));
            }

            var listPair = m_PoolDic[pair];
            var time = Time.unscaledTime;
            for (int j = 0; j < items.Length; j++)
            {
                var item = items[j];
                //查找已归还元素
                for (int i = 0; i < listPair.freeList.Count; i++)
                {
                    if (listPair.freeList[i].Item.Equals(item))
                    {
                        CommonLog.LogWarning($"物品 {item.name} 已经归还,不能重复归还");
                        continue;
                    }
                }

                //查找要归还的元素
                for (int i = 0; i < listPair.usingList.Count; i++)
                {
                    if (listPair.usingList[i].Item.Equals(item))
                    {
                        var usingItem = listPair.usingList[i];
                        usingItem.AddTime = time;
                        usingItem.Item.transform.SetParent(m_PoolParent);
                        listPair.freeList.Add(usingItem);
                        continue;
                    }
                }

                //新元素
                ReferencePoolItem poolItem = null;
                if (m_FreeItemList.Count > 0)
                {
                    poolItem = m_FreeItemList[m_FreeItemList.Count - 1];
                    m_FreeItemList.RemoveAt(m_FreeItemList.Count - 1);
                }
                else
                {
                    poolItem = new ReferencePoolItem();
                }
                poolItem.Item = item;
                poolItem.AddTime = time;
                item.transform.SetParent(m_PoolParent);
                listPair.freeList.Add(poolItem);
                continue;
            }
        }

        /// <summary>
        /// 缓存池元素
        /// </summary>
        private class ReferencePoolItem
        {
            /// <summary>
            /// 添加时间
            /// </summary>
            public float AddTime { get; set; }

            /// <summary>
            /// 保存的元素
            /// </summary>
            public GameObject Item { get; set; }

            /// <summary>
            /// 是否销毁
            /// </summary>
            /// <param name="realTime"></param>
            /// <returns>是否回收</returns>
            public bool IsDestroy(float realTime)
            {
                return CFM.GameObjectPool.HoldDuration + AddTime <= realTime;
            }
        }
    }
}
