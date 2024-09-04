using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentDestroyerScript : MonoBehaviour
{
    public Transform myObject;
    Collider[] childColliders;

    [Button]
    public void DestroyColliders()
    {
        childColliders = myObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in childColliders)
        {
            DestroyImmediate(collider);
        }
    }
}

