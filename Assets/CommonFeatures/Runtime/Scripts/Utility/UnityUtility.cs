using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.Utility
{
    /// <summary>
    /// Unity������
    /// </summary>
    public static class UnityUtility
    {
        /// <summary>
        /// ��ȡ���������������,�����Ƿ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static List<T> FindComponents<T>(this GameObject go) where T : Component
        {
            return FindComponents<T>(go.transform);
        }

        /// <summary>
        /// ��ȡ���������������,�����Ƿ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static List<T> FindComponents<T>(this Transform trans) where T : Component
        {
            List<T> result = new List<T>();
            FindComponentsRecursive<T>(trans, result);
            return result;
        }

        /// <summary>
        /// �ݹ��ȡ�����������,�����Ƿ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <param name="finds"></param>
        private static void FindComponentsRecursive<T>(Transform trans, List<T> finds) where T : Component
        {
            var t = trans.GetComponent<T>();
            if (null != t)
            {
                finds.Add(t);
            }
            for (int i = 0; i < trans.childCount; i++)
            {
                FindComponentsRecursive<T>(trans.GetChild(i), finds);
            }
        }
    }
}
