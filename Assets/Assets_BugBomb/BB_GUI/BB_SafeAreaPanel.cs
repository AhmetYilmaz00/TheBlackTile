using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BB_SafeAreaPanel : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    private void Start()
    {
        ApplySafeArea();

#if UNITY_EDITOR
        if (GUIConfiguration.instance.ForceSafeAreaForIphone)
        {
            _rectTransform.sizeDelta -= new Vector2(0, GUIConfiguration.instance.SafeAreaPadding.top + GUIConfiguration.instance.SafeAreaPadding.bottom);
            _rectTransform.anchoredPosition = new Vector2(0, (GUIConfiguration.instance.SafeAreaPadding.bottom - GUIConfiguration.instance.SafeAreaPadding.top) * 0.5f);
        }
#endif
    }

    private void ApplySafeArea()
    {
        var anchorMin = Screen.safeArea.position;
        var anchorMax = Screen.safeArea.position + Screen.safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}