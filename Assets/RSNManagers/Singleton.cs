using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace RSNManagers
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        #region Fields

        [CanBeNull] private static T _instance;

        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new();

        [FormerlySerializedAs("_persistent")] [SerializeField]
        private bool persistent = true;

        #endregion

        #region Properties

        [NotNull]
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    Debug.LogWarning(
                        $"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }

                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;
                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    if (count > 0)
                    {
                        if (count == 1)
                            return _instance = instances[0];
                        Debug.LogWarning(
                            $"[{nameof(Singleton)}<{typeof(T)}>] There should never be more than one {nameof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                        for (var i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                        return _instance = instances[0];
                    }

                    Debug.Log(
                        $"[{nameof(Singleton)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return _instance = new GameObject($"({nameof(Singleton)}){typeof(T)}")
                        .AddComponent<T>();
                }
            }
        }

        #endregion

        #region Methods

        protected virtual void Awake()
        {
            if (transform.parent) transform.SetParent(null,true);
            if (persistent) DontDestroyOnLoad(gameObject);
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        #endregion
    }

    public abstract class Singleton : MonoBehaviour
    {
        #region Properties

        protected static bool Quitting { get; private set; }

        #endregion

        #region Methods

        private void OnApplicationQuit()
        {
            Quitting = true;
        }

        #endregion
    }
}