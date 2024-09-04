using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BB_GUIEventSystemLock : MonoBehaviour
{
    public static BB_GUIEventSystemLock instance;
    public float lockTimer = 0.2f;
    public bool locked = false;
    EventSystem es;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        es = GetComponent<EventSystem>();
    }

    public void LockForTime()
    {
        //Debug.Log("EventSystem LockForTime");
        if (locked == false)
        {
            locked = true;
            es.enabled = false;
            Invoke("Unlock", lockTimer);
        }
    }

    public void Unlock()
    {
        //Debug.Log("EventSystem Unlock");
        locked = false;
        es.enabled = true;
    }

    internal void Lock()
    {
        locked = true;
        es.enabled = false;
    }
}
