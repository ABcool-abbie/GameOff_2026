using System;
using UnityEngine;

namespace Utility
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Activator.CreateInstance(typeof(T), true) as T;
                }

                return instance;
            }
        }
    }

    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        [SerializeField] private bool dontDestroyOnLoad = false;
        private static T instance;
        protected static bool InstanceExists => instance != null;

    protected static T GetInstance()
    {
        if (instance == null)
        {
            instance = FindAnyObjectByType<T>();
            if (instance == null)
            {
                throw new SingletonException(typeof(T).Name);
             }
         }
            return instance;
        }

        protected void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"Multiple singletons of type {typeof(T).Name} detected. Destroying duplicate on '{name}'.", this);
                Destroy(gameObject);
                return;
            }

            instance = this as T;

            if (dontDestroyOnLoad)
            {
                if (transform.parent != null)
                {
                    Debug.LogWarning($"{typeof(T).Name}: DontDestroyOnLoad only works on root objects.", this);
                }
                DontDestroyOnLoad(gameObject);
            }

            Init();
        }
        protected virtual void Init() { }
    }

    public abstract class SingletonBehaviour<T> : SingletonMono<T> where T : SingletonBehaviour<T>
    {
        public static T Instance => GetInstance();
        public static bool HasInstance => InstanceExists;
    }

    public abstract class ProtectedSingletonBehaviour<T> : SingletonMono<T> where T : ProtectedSingletonBehaviour<T>
    {
        protected static T Instance => GetInstance();
        protected static bool HasInstance => InstanceExists;
    }

    public class SingletonException : Exception
    {
        public SingletonException(string className)
        : base($"Singleton not found in scene: {className}. Add it to the scene before accessing Instance.") { }
    }
}



