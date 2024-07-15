using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFeatures.Pool
{
    /// <summary>
    /// ͨ�ù���_���建���
    /// </summary>
    public class CommonFeature_GameObjectPool : CommonFeature
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
        private Dictionary<(GameObject origin, Transform parent), List<ReferencePoolItem>> m_PoolDic = new Dictionary<(GameObject origin, Transform parent), List<ReferencePoolItem>>();

        /// <summary>
        /// ����ʱ������ʱ��
        /// </summary>
        private float m_ClearSpaceTimer;

        private Transform m_PoolParent;

        public override UniTask Init()
        {
            //������ʱ�䲻��С�ڳ���ʱ��
            ClearSpace = Mathf.Clamp(ClearSpace, 0, Mathf.Max(0, HoldDuration));

            m_ClearSpaceTimer = 0f;

            //��ʼ���ظ���
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
        /// <param name="origin"></param>
        /// <param name="parent"></param>
        /// <param name="count">ȡ������</param>
        /// <param name="backAll">����������ʹ�õ�,Ϊtrueʱ�Ὣ��������ʹ�õ�gameobject�Զ�����</param>
        /// <param name="setIdetityTransform">���ñ�׼transform����</param>
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
                //���±�0��ʼ
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
                //�Ż���������item
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
                    //��������û��ʹ�õ�item
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
                    //���������item
                    for (int i = findedIndex; i < result.Length; i++)
                    {
                        var go = GameObject.Instantiate(origin);
                    }
                }
            }
            return result;
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
            public GameObject Item { get; set; }

            /// <summary>
            /// �Ƿ�����ʹ����
            /// </summary>
            public bool Using { get; set; }

            /// <summary>
            /// �Ƿ�����
            /// </summary>
            /// <param name="realTime"></param>
            /// <returns>�Ƿ����</returns>
            public bool IsDestroy(float realTime)
            {
                return CFM.GameObjectPool.HoldDuration + AddTime <= realTime;
            }
        }
    }
}
