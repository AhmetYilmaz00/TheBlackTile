using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;


public class FracturedBlock : MonoBehaviour
{
    public Rigidbody[] Rigidbodies;

    private bool _destroyed;

    private void Update()
    {
        if (!_destroyed)
            return;

        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].transform.localScale = Vector3.MoveTowards(Rigidbodies[i].transform.localScale, Vector3.zero, Time.deltaTime * 3);
        }

        if (Rigidbodies[0].transform.localScale.x < 0.01f)
        {
            DestroyAll();
            enabled = false;
        }
    }

    public void DestroyBlock()
    {
        if (_destroyed)
            return;

        _destroyed = true;
        //Debug.Log(velocity);
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].gameObject.SetActive(true);
            Rigidbodies[i].isKinematic = false;
            Rigidbodies[i].AddExplosionForce(GameplayConfiguration.instance.FractureExplosionForce, transform.position, 1f);
        };
    }

    private void DestroyAll()
    {
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Destroy(Rigidbodies[i].gameObject);
        }
    }

    [Button]
    public void GetRigidbodies()
    {
        Rigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    internal void SetMaterial(Material sharedMaterial)
    {
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].GetComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
        }
    }
}