using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.Pool
{
    /// <summary>
    /// ͨ�ù���_���û����
    /// </summary>
    public class CommonFeature_ReferencePool : CommonFeature
    {
        [Header("�����Ԫ�س���ʱ��,��ʱ�Զ��ͷ�")]
        [SerializeField] internal float HoldDuration = 60f;

        [Header("����Ԫ������ʱ����")]
        [SerializeField] private float ClearSpace = 10f;

        /// <summary>
        /// ���ɳ�item
        /// </summary>
        private List<ReferencePoolItem> m_FreeItemList = new List<ReferencePoolItem>();

        /// <summary>
        /// �����
        /// </summary>
        private Dictionary<System.Type, HashSet<ReferencePoolItem>> m_PoolDic = new Dictionary<System.Type, HashSet<ReferencePoolItem>>();

        /// <summary>
        /// ���б��������
        /// </summary>
        private HashSet<IReference> m_AllHoldReference = new HashSet<IReference>();

        /// <summary>
        /// ����ʱ������ʱ��
        /// </summary>
        private float m_ClearSpaceTimer;

        public override UniTask Init()
        {
            //������ʱ�䲻��С�ڳ���ʱ��
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

                    //����ʱ��û��ʹ�õ�item
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
        /// �ӳ���ȡ��
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
        /// �ӳ���ȡ��
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
        /// �黹
        /// </summary>
        /// <param name="item"></param>
        public void Back(IReference item)
        {
            if (m_AllHoldReference.Contains(item))
            {
                CommonLog.LogError("�򻺴�����ظ��黹Ԫ��");
                return;
            }
            //Ԫ��û�г���ʱ��,����黹
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
        /// �����Ԫ��
        /// </summary>
        private class ReferencePoolItem
        {
            /// <summary>
            /// ���ʱ��
            /// </summary>
            public float AddTime { get; set; }

            /// <summary>
            /// �����Ԫ��
            /// </summary>
            public IReference Item { get; set; }

            /// <summary>
            /// �Ƿ�����
            /// </summary>
            /// <param name="realTime"></param>
            /// <returns>�Ƿ����</returns>
            public bool IsDestroy(float realTime)
            {
                return CFM.ReferencePool.HoldDuration + AddTime <= realTime;
            }
        }
    }
}
