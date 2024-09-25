using UnityEngine;

namespace AIMStudio.Scripts
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        #region Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static T _instance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    //if (instance == null)
                    //{
                    //    GameObject obj = new GameObject();
                    //    obj.name = typeof(T).Name;
                    //    instance = obj.AddComponent<T>();
                    //}
                }

                return _instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.Log("Generic Singleton Destroying : " + gameObject);
                Destroy(gameObject);
            }
        }

        #endregion
    }
}