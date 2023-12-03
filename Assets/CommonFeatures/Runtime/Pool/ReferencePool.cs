using CommonFeatures.Log;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.Pool
{
    /// <summary>
    /// 引用缓存池
    /// </summary>
    public class ReferencePool
    {
        /// <summary>
        /// 缓存池
        /// </summary>
        private static Dictionary<System.Type, HashSet<IReference>> m_PoolDic = new Dictionary<System.Type, HashSet<IReference>>();

        /// <summary>
        /// 从池中取用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Acquire<T>() where T : IReference
        {
            var type = typeof(T);
            if (!m_PoolDic.ContainsKey(type))
            {
                m_PoolDic.Add(type, new HashSet<IReference>());
            }
            var hashSet = m_PoolDic[type];
            if (hashSet.Count > 0)
            {
                var item = (T)hashSet.Last();
                hashSet.Remove(item);
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
        public static IReference Acquire(System.Type type)
        {
            if (!m_PoolDic.ContainsKey(type))
            {
                m_PoolDic.Add(type, new HashSet<IReference>());
            }
            var hashSet = m_PoolDic[type];
            if (hashSet.Count > 0)
            {
                var item = hashSet.Last();
                hashSet.Remove(item);
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
        public static void Back(IReference item)
        {
            var type = item.GetType();
            if (!m_PoolDic.ContainsKey(type))
            {
                m_PoolDic.Add(type, new HashSet<IReference>());
            }
            var hashSet = m_PoolDic[type];
            if (hashSet.Contains(item))
            {
                CommonLog.TraceError("向缓存池中重复归还元素");
                return;
            }
            hashSet.Add(item);
            item.Reset();
        }
    }
}
