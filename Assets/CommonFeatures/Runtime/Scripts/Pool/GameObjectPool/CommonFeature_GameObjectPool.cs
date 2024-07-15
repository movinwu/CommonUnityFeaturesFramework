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
        private Dictionary<(GameObject origin, Transform parent), (List<ReferencePoolItem> usingList, List<ReferencePoolItem> freeList)> m_PoolDic = new Dictionary<(GameObject origin, Transform parent), (List<ReferencePoolItem> usingList, List<ReferencePoolItem> freeList)>();

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

                    //����ʱ��û��ʹ�õ�item
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
        /// �ӳ���ȡ��
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="parent"></param>
        /// <param name="setIdetityTransform">���ñ�׼transform����</param>
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
            //����û��ʹ�õ�item
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
            //����item��ʹ��
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
                m_PoolDic.Add(pair, (new List<ReferencePoolItem>(), new List<ReferencePoolItem>()));
            }
            var listPair = m_PoolDic[pair];
            if (backAll)
            {
                //�洢����item
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

                //���·���
                for (int i = 0; i < count; i++)
                {
                    //����,��Ҫ�½�
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
                    //����,��Ҫ�½�
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
        /// �黹
        /// </summary>
        /// <param name="item"></param>
        public void Back(GameObject origin, Transform parent, GameObject item)
        {
            if (null == origin || null == parent)
            {
                return;
            }
            //Ԫ��û�г���ʱ��,����黹
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
            //�����ѹ黹Ԫ��
            for (int i = 0; i < listPair.freeList.Count; i++)
            {
                if (listPair.freeList[i].Item.Equals(item))
                {
                    CommonLog.LogWarning($"��Ʒ {item.name} �Ѿ��黹,�����ظ��黹");
                    return;
                }
            }

            var time = Time.unscaledTime;
            //����Ҫ�黹��Ԫ��
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

            //��Ԫ��
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
        /// �黹
        /// </summary>
        /// <param name="items"></param>
        public void Back(GameObject origin, Transform parent, GameObject[] items)
        {
            if (null == origin || null == parent)
            {
                return;
            }
            //Ԫ��û�г���ʱ��,����黹
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
                //�����ѹ黹Ԫ��
                for (int i = 0; i < listPair.freeList.Count; i++)
                {
                    if (listPair.freeList[i].Item.Equals(item))
                    {
                        CommonLog.LogWarning($"��Ʒ {item.name} �Ѿ��黹,�����ظ��黹");
                        continue;
                    }
                }

                //����Ҫ�黹��Ԫ��
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

                //��Ԫ��
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
