using UnityEngine;

namespace OOPS
{
    /// <summary>
    /// 继承mono的单例
    /// <para>所有Mono单例都在加载场景时不可销毁,不能单独自行挂载</para>
    /// </summary>
    public class MonoSingletonBase<T> : MonoBehaviour where T : MonoSingletonBase<T>
    {
        /// <summary>
        /// 所有单例挂载父物体名称
        /// </summary>
        private const string SingletonParentName = "Singletons";

        private static T _instance = null;
        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    var obj = GameObject.Find(SingletonParentName);
                    if (null == obj)
                    {
                        obj = new GameObject(SingletonParentName);
                        GameObject.DontDestroyOnLoad(obj);
                    }
                    var name = typeof(T).Name;
                    var childObj = obj.transform.Find(name);
                    if (null == childObj)
                    {
                        childObj = new GameObject(name).transform;
                        childObj.SetParent(obj.transform);
                    }
                    _instance = childObj.GetComponent<T>();
                    if (null == _instance)
                    {
                        _instance = childObj.gameObject.AddComponent<T>();
                        _instance.Init();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake() { _instance = this as T; }

        protected virtual void Init() { }
    }
}
