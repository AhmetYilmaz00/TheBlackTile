using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_GUICollection : MonoBehaviour
{
    public RectTransform HolderPanel;
    private Camera camera;

    public int AnimsInProgress { get; private set; }
    private int PrefabCount;

    [Header("Aniamtions")]
    public float StarPunchScaleAnimTime = 0.2f;
    public float StarToPanelAnimTime = 0.8f;

    private void Awake()
    {
        camera = Camera.main;
    }

    public void AddPrefabOnWorldPositionWithDelay(GameObject uiPrefab, Action<int> callbackAction, Vector3 fromWordlPos, RectTransform toRT, float delay, int uiPrefabCount)
    {
        Vector2 uiPos = camera.WorldToScreenPoint(fromWordlPos);
        Vector2 toPos = SwitchToRectTransform(toRT, HolderPanel);
        AddPrefabOnScreenPositionWithDelay(uiPrefab, callbackAction, uiPos - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), toPos, delay, uiPrefabCount);
    }

    public void AddPrefabOnRectTransformPositionWithDelay(GameObject uiPrefab, Action<int> callbackAction, RectTransform fromRT, RectTransform toRT, float delay, int starsCount)
    {
        Vector2 toPos = SwitchToRectTransform(toRT, HolderPanel);
        AddPrefabOnScreenPositionWithDelay(uiPrefab, callbackAction, GetRectTransformPositionOnHolderPanel(fromRT), toPos, delay, starsCount);
    }

    public void AddPrefabOnScreenPositionWithDelay(GameObject uiPrefab, Action<int> callbackAction, Vector2 fromScreenPos, Vector2 toScreenPos, float delay, int uiPrefabCount)
    {
        PrefabCount = uiPrefabCount;
        for (int i = 0; i < uiPrefabCount; i++)
        {
            AnimsInProgress++;
            if (i == 0)
            {
                DOVirtual.DelayedCall(delay, () => AddPrefabOnScreenPosition(uiPrefab, callbackAction, fromScreenPos, toScreenPos));
            }
            else
            {
                var rndPos = UnityEngine.Random.insideUnitSphere * (uiPrefabCount > 10 ? 150f : 100f);
                var pos2 = fromScreenPos + new Vector2(rndPos.x, rndPos.z);
                DOVirtual.DelayedCall(delay, () => AddPrefabOnScreenPosition(uiPrefab, callbackAction, pos2, toScreenPos));
            }
        }
    }

    private void AddPrefabOnScreenPosition(GameObject uiPrefab, Action<int> callbackAction, Vector2 fromScreenPos, Vector2 toScreenPos)
    {
        GameObject go = Instantiate(uiPrefab, HolderPanel);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = fromScreenPos;
        rt.DOAnchorPos(rt.anchoredPosition + new Vector2(0, 50), StarPunchScaleAnimTime).OnComplete(() =>
        {
            rt.DOSizeDelta(new Vector2(50, 50), StarToPanelAnimTime).SetEase(Ease.InQuart);
            rt.DOAnchorPos(toScreenPos, StarToPanelAnimTime).SetEase(Ease.InQuart).OnComplete(() =>
            {
                AnimsInProgress--;

                if (AnimsInProgress == 0)
                {
                    if (callbackAction != null)
                        callbackAction.Invoke(PrefabCount);
                }

                Destroy(go, 0.01f);
            });
        });
    }

    public void AddPrefabInSeriesOnWorldPositionWithDelay(GameObject uiPrefab, Action<int> callbackAction, Vector3 fromWordlPos, RectTransform toRT, float delay, int uiPrefabCount)
    {
        Vector2 uiPos = camera.WorldToScreenPoint(fromWordlPos);
        uiPos -= new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 toPos = SwitchToRectTransform(toRT, HolderPanel);
        PrefabCount = uiPrefabCount;
        for (int i = 0; i < uiPrefabCount; i++)
        {
            AnimsInProgress++;
            DOVirtual.DelayedCall(i * delay, () => AddSinglePrefabOnScreenPosition(uiPrefab, callbackAction, uiPos, toPos));
        }
    }


    private void AddSinglePrefabOnScreenPosition(GameObject uiPrefab, Action<int> callbackAction, Vector2 fromScreenPos, Vector2 toScreenPos)
    {
        GameObject go = Instantiate(uiPrefab, HolderPanel);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = fromScreenPos;

        float animTime = 0.4f;
        float scaleTime = 0.3f;

        rt.DOSizeDelta(new Vector2(50, 50), scaleTime).SetEase(Ease.InQuart);
        rt.DOAnchorPos(toScreenPos, animTime).SetEase(Ease.InQuart).OnComplete(() =>
        {
            AnimsInProgress--;

            if (AnimsInProgress == 0)
            {
                if (callbackAction != null)
                    callbackAction.Invoke(PrefabCount);
            }

            Destroy(go, 0.01f);
        });
    }

    private void AddSinglePrefabOnScreenPositionWithHop(GameObject uiPrefab, Action<int> callbackAction, Vector2 fromScreenPos, Vector2 toScreenPos)
    {
        GameObject go = Instantiate(uiPrefab, HolderPanel);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = fromScreenPos;
        rt.DOAnchorPos(rt.anchoredPosition + new Vector2(0, 50), StarPunchScaleAnimTime).OnComplete(() =>
        {
            rt.DOSizeDelta(new Vector2(50, 50), StarToPanelAnimTime).SetEase(Ease.InQuart);
            rt.DOAnchorPos(toScreenPos, StarToPanelAnimTime).SetEase(Ease.InQuart).OnComplete(() =>
            {
                AnimsInProgress--;

                if (AnimsInProgress == 0)
                {
                    if (callbackAction != null)
                        callbackAction.Invoke(PrefabCount);
                }

                Destroy(go, 0.01f);
            });
        });
    }




    public Vector2 GetRectTransformPositionOnHolderPanel(RectTransform rt)
    {
        return SwitchToRectTransform(rt, HolderPanel);
    }

    /// <summary>
    /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
    /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
    /// </summary>
    public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }
}
