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
        private Dictionary<(GameObject origin, Transform parent), List<ReferencePoolItem>> m_PoolDic = new Dictionary<(GameObject origin, Transform parent), List<ReferencePoolItem>>();

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
            m_PoolParent.localScale = Vector3.zero;

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
                    foreach (var hashSet in m_PoolDic.Values)
                    {
                        var array = hashSet.ToArray();
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i].IsDestroy(realTime))
                            {
                                var poolItem = array[i];
                                poolItem.Item = null;
                                m_FreeItemList.Add(poolItem);
                                hashSet.Remove(poolItem);
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
                m_PoolDic.Add(pair, new List<ReferencePoolItem>());
            }
            var list = m_PoolDic[pair];
            GameObject[] result = new GameObject[count];
            var time = Time.unscaledTime;
            if (backAll)
            {
                //从下标0开始
                for (int i = 0; i < count; i++)
                {
                    GameObject go = null;
                    if (list.Count <= i)
                    {
                        go = GameObject.Instantiate(origin);
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
                        item.AddTime = time;
                        item.Item = go;
                        item.Using = true;
                        list.Add(item);
                    }
                    else
                    {
                        var item = list[i];
                        item.Using = true;
                        item.AddTime = time;
                        go = item.Item;
                    }
                    go.transform.SetParent(parent);
                    if (setIdetityTransform)
                    {
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;
                    }
                    result[i] = go;
                }
                //放回所有其他item
                for (int i = count; i < list.Count; i++)
                {
                    var item = list[i];
                    item.Using = false;
                    item.AddTime = time;
                }
            }
            else
            {
                int findedIndex = 0;
                if (findedIndex < result.Length)
                {
                    //遍历所有没有使用的item
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (!list[i].Using)
                        {
                            list[i].Using = true;
                            list[i].Item.transform.SetParent(parent);
                            result[findedIndex] = list[i].Item;
                            findedIndex++;
                        }
                        if (findedIndex >= result.Length)
                        {
                            break;
                        }
                    }
                }
                if (findedIndex < result.Length)
                {
                    //创建不足的item
                    for (int i = findedIndex; i < result.Length; i++)
                    {
                        var go = GameObject.Instantiate(origin);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="item"></param>
        public void Back(IReference item)
        {
            if (m_AllHoldReference.Contains(item))
            {
                CommonLog.LogError("向缓存池中重复归还元素");
                return;
            }
            //元素没有持有时间,无需归还
            if (HoldDuration <= 0)
            {
                return;
            }
            var type = item.GetType();
            if (!m_PoolDic.ContainsKey(type))
            {
                m_PoolDic.Add(type, new HashSet<ReferencePoolItem>());
            }
            var hashSet = m_PoolDic[type];
            ReferencePoolItem poolItem = null;
            if (m_FreeItemList.Count == 0)
            {
                poolItem = new ReferencePoolItem();
            }
            else
            {
                var index = m_FreeItemList.Count - 1;
                poolItem = m_FreeItemList[index];
                m_FreeItemList.RemoveAt(index);
            }
            poolItem.Item = item;
            poolItem.AddTime = Time.unscaledTime;
            hashSet.Add(poolItem);
            item.Reset();
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
            /// 是否正在使用中
            /// </summary>
            public bool Using { get; set; }

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
