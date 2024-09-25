using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/Singleton Game Event")]
    public class SingletonGameEvent : ScriptableObject
    {
        private static SingletonGameEvent _instance;

        public static SingletonGameEvent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance<SingletonGameEvent>();
                }

                return _instance;
            }
        }

        private List<GameEventListener> listeners = new List<GameEventListener>();

        public void Raise()
        {
            for (var i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}