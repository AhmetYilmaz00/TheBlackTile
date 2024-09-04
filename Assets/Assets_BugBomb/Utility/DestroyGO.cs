using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGO : MonoBehaviour
{
    public bool PooledObject;
    public bool Particle;
    public float time = 1;
    
    private ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        if (!Particle)
        {
            Invoke("DestroyGONow", time);
        }
        else
        {
            Invoke("DestroyParticle", time);
        }
    }

    private void DestroyGONow()
    {
        Destroy(gameObject);
    }

    public void DestroyParticle()
    {
        var emission = _particle.emission;
        emission.enabled = false;
        Invoke("DestroyGONow", time);
    }
}
