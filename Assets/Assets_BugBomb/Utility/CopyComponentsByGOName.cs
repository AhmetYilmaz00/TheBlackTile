using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyComponentsByGOName : MonoBehaviour
{
    public Transform fromGO;
    public Transform toGO;

    List<Transform> allFromTr;
    List<Transform> allToTr;

    public bool copyBoxColl = false;
    public bool copyCapsuleColl = false;
    public bool copyRigidbody = false;
    public bool copyHingeJoint = false;

    [Button]
    public void CopyComponents()
    {
        allFromTr = new List<Transform>();
        allToTr = new List<Transform>();

        allFromTr.AddRange(fromGO.GetComponentsInChildren<Transform>());
        allToTr.AddRange(toGO.GetComponentsInChildren<Transform>());


        for (int i = 0; i < allFromTr.Count; i++)
        {
            if (copyBoxColl)
            {
                BoxCollider bc = allFromTr[i].GetComponent<BoxCollider>();
                if (bc != null)
                    CopyComponents(allFromTr[i].gameObject.name, bc);
            }
            if (copyCapsuleColl)
            {
                CapsuleCollider cc = allFromTr[i].GetComponent<CapsuleCollider>();
                if (cc != null)
                    CopyComponents(allFromTr[i].gameObject.name, cc);
            }
            if (copyRigidbody)
            {
                Rigidbody r = allFromTr[i].GetComponent<Rigidbody>();
                if (r != null)
                    CopyComponents(allFromTr[i].gameObject.name, r);
            }
            if (copyHingeJoint)
            {
                HingeJoint hj = allFromTr[i].GetComponent<HingeJoint>();
                if (hj != null)
                    CopyComponents(allFromTr[i].gameObject.name, hj);
            }
        }

        Debug.Log("Copy done!");
    }

    private void CopyComponents(string name, HingeJoint fromHj)
    {
        for (int i = 0; i < allToTr.Count; i++)
        {
            if (name == allToTr[i].gameObject.name)
            {
                HingeJoint newHj = allToTr[i].gameObject.AddComponent<HingeJoint>();
                newHj.connectedBody = FindByName(fromHj.connectedBody.name);
                newHj.autoConfigureConnectedAnchor = true;
                newHj.axis = fromHj.axis;
                newHj.useLimits = fromHj.useLimits;
                newHj.limits = fromHj.limits;
            }
        }
    }

    private void CopyComponents(string name, Rigidbody fromR)
    {
        for (int i = 0; i < allToTr.Count; i++)
        {
            if (name == allToTr[i].gameObject.name)
            {
                Rigidbody newR = allToTr[i].gameObject.AddComponent<Rigidbody>();
                newR.isKinematic = fromR.isKinematic;
            }
        }
    }

    private void CopyComponents(string name, BoxCollider fromC)
    {
        for (int i = 0; i < allToTr.Count; i++)
        {
            if (name == allToTr[i].gameObject.name)
            {
                BoxCollider newColl = allToTr[i].gameObject.AddComponent<BoxCollider>();
                newColl.size = fromC.size;
                newColl.center = fromC.center;
            }
        }
    }
    private void CopyComponents(string name, CapsuleCollider fromC)
    {
        for (int i = 0; i < allToTr.Count; i++)
        {
            if (name == allToTr[i].gameObject.name)
            {
                CapsuleCollider newColl = allToTr[i].gameObject.AddComponent<CapsuleCollider>();
                newColl.radius = fromC.radius;
                newColl.center = fromC.center;
                newColl.height = fromC.height;
                newColl.direction = fromC.direction;
            }
        }
    }

    private Rigidbody FindByName(string name)
    {
        for (int i = 0; i < allToTr.Count; i++)
        {
            if (name == allToTr[i].gameObject.name)
            {
                return allToTr[i].GetComponent<Rigidbody>();
            }
        }
        Debug.LogError($"FindByName: Rigidbody {name} not found");
        return null;
    }
}
