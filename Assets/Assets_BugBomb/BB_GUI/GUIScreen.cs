using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class GUIScreen : MonoBehaviour
{
    public GUIScreenType GUIScreenType;
    public int SortOrder = 100;
    private Canvas canvas;
    private CanvasGroup viewCG;
    public float ShowAnimTime = 0.3f;
    public float CloseAnimTime = 0.3f;

    [Header("Properties")]
    public bool LockInputWhileOpen = false;
    public bool ScreenIsInteractable = true;

    [Header("Events")]
    public UnityEvent PreOpenEvent;
    public UnityEvent PostOpenEvent;
    public UnityEvent PreCloseEvent;
    public UnityEvent PostCloseEvent;

    public void SetupPopup()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.sortingOrder = SortOrder;
        viewCG = GetComponent<CanvasGroup>();
        viewCG.alpha = 0;
        viewCG.interactable = false;
        viewCG.blocksRaycasts = false;
    }


    public void OpenPopup()
    {
        OnPreShow();

        if (ScreenIsInteractable)
        {
            viewCG.interactable = true;
            viewCG.blocksRaycasts = true;
        }
        viewCG.DOFade(1f, ShowAnimTime).OnComplete(OnPostShow);
    }

    public void ClosePopup(float delay = 0)
    {
        if (delay <= 0)
        {
            ClosePopup();
        }
        else
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                ClosePopup();
            });
        }
    }

    public void ClosePopup()
    {
        OnPreClose();

        viewCG.interactable = false;
        viewCG.blocksRaycasts = false;
        viewCG.DOFade(0f, CloseAnimTime).OnComplete(OnPostClose);
    }

    internal void ForceVisibility(bool guiVisible)
    {
        if (viewCG != null)
            viewCG.alpha = guiVisible ? 1 : 0;
    }

    private void OnPreShow()
    {
        PreOpenEvent.Invoke();

        if (LockInputWhileOpen)
            Messenger<bool>.Broadcast(Message.LockInput, true);
    }

    private void OnPostShow()
    {
        PostOpenEvent.Invoke();
    }

    private void OnPreClose()
    {
        PreCloseEvent.Invoke();
    }

    private void OnPostClose()
    {
        PostCloseEvent.Invoke();

        if (LockInputWhileOpen)
            Messenger<bool>.Broadcast(Message.LockInput, false);

        Destroy(gameObject);
    }

    public void LockInteractable()
    {
        viewCG.interactable = false;
        viewCG.blocksRaycasts = false;
    }
}
