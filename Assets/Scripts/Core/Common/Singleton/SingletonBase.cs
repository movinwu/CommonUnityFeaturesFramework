using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// ��������
    /// <para>ʵ������Ҫ����˽���޲ι��캯��</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBase<T> where T : SingletonBase<T>
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    //�������,��ʱ�����Ƕ��߳�
                    var type = typeof(T);
                    ConstructorInfo privateConstructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(int) }, null);
                    if (null == privateConstructor)
                    {
                        throw new System.NotImplementedException("��������Ҫ�ṩ˽���޲ι��캯��");
                    }
                    _instance = privateConstructor.Invoke(null) as T;
                }
                return _instance;
            }
        }
    }
}
