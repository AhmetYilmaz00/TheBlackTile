#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_HideForAds : MonoBehaviour
{
    public bool MakeInvisible = true;
    public bool MakeUnclickable = true;

    void Start()
    {
        if (GUIConfiguration.instance.AdsEnabled)
        {
            CanvasGroup cg = gameObject.AddComponent<CanvasGroup>();
            if (MakeInvisible)
                cg.alpha = 0;
            if (MakeUnclickable)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }
}
#endif

