using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.Pool
{
    /// <summary>
    /// 通用功能_引用缓存池
    /// </summary>
    public class CommonFeature_ReferencePool : CommonFeature
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
        private Dictionary<System.Type, HashSet<ReferencePoolItem>> m_PoolDic = new Dictionary<System.Type, HashSet<ReferencePoolItem>>();

        /// <summary>
        /// 所有保存的引用
        /// </summary>
        private HashSet<IReference> m_AllHoldReference = new HashSet<IReference>();

        /// <summary>
        /// 清理时间间隔计时器
        /// </summary>
        private float m_ClearSpaceTimer;

        public override UniTask Init()
        {
            //清理间隔时间不能小于持有时间
            ClearSpace = Mathf.Clamp(ClearSpace, 0, Mathf.Max(0, HoldDuration));

            m_ClearSpaceTimer = 0f;

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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Acquire<T>() where T : IReference
        {
            var type = typeof(T);
            if (!m_PoolDic.ContainsKey(type))
            {
                m_PoolDic.Add(type, new HashSet<ReferencePoolItem>());
            }
            var hashSet = m_PoolDic[type];
            if (hashSet.Count > 0)
            {
                var poolItem = hashSet.Last();
                var item = (T)poolItem.Item;
                poolItem.Item = null;
                hashSet.Remove(poolItem);
                m_FreeItemList.Add(poolItem);
                m_AllHoldReference.Remove(item);
                return item;
            }
            T t = (T)System.Activator.CreateInstance(type);
            t.Reset();
            return t;
        }

        /// <summary>
        /// 从池中取用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IReference Acquire(System.Type type)
        {
            if (!m_PoolDic.ContainsKey(type))
            {
                m_PoolDic.Add(type, new HashSet<ReferencePoolItem>());
            }
            var hashSet = m_PoolDic[type];
            if (hashSet.Count > 0)
            {
                var poolItem = hashSet.Last();
                var item = poolItem.Item;
                poolItem.Item = null;
                hashSet.Remove(poolItem);
                m_FreeItemList.Add(poolItem);
                m_AllHoldReference.Remove(item);
                return item;
            }
            IReference reference = (IReference)System.Activator.CreateInstance(type);
            reference.Reset();
            return reference;
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
            public IReference Item { get; set; }

            /// <summary>
            /// 是否销毁
            /// </summary>
            /// <param name="realTime"></param>
            /// <returns>是否回收</returns>
            public bool IsDestroy(float realTime)
            {
                return CFM.ReferencePool.HoldDuration + AddTime <= realTime;
            }
        }
    }
}
