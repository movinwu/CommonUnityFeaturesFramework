using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// 单例基类
    /// <para>实现类需要声明私有无参构造函数</para>
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
                    //反射加载,暂时不考虑多线程
                    var type = typeof(T);
                    ConstructorInfo privateConstructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new System.Type[0], null);
                    if (null == privateConstructor)
                    {
                        throw new System.NotImplementedException($"单例类 {type} 需要提供私有无参构造函数");
                    }
                    _instance = privateConstructor.Invoke(null) as T;
                }
                return _instance;
            }
        }
    }
}
