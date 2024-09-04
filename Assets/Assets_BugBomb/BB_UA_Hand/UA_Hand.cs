using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UA_Hand : MonoBehaviour
{
    [Header("Position")]
    public RectTransform handPositionHolder;
    public Vector2 posOffset;

    [Header("Rotation")]
    public bool Rotate = true;
    private RectTransform handRotationHolder;
    public float normalZRotation = -5f;
    public float downZRotation = 15f;
    public float rotationAnimTime = 0.4f;

    [Header("Scale")]
    public bool Scale = false;
    private RectTransform handScaleHolder;
    public float normalScale = 1f;
    public float downScale = 0.75f;
    public float scaleAnimTime = 0.4f;

    [Header("Show-Hide")]
    public CanvasGroup handHolderCanvasGroup;
    public bool startVisible = false;
    public KeyCode showHideKey;
    public float showHideAnimTime = 0.2f;

    [Header("Stop mouse follow")]
    public KeyCode stopStartMouseFollowKey;
    private bool follow = true;

    [Header("Hands")]
    [Range(0, 4)]
    public int currentHandIndex = 0;
    public List<RectTransform> HandsTR;

    Vector3 currentMousePos;
    Vector2 currentHandPos;

    float halfWidth;
    float halfHeight;

    bool isHandVisible;

    private void OnValidate()
    {
        SetActiveHand();
    }
    private void Start()
    {
        handScaleHolder = handPositionHolder;
        SetActiveHand();
        halfHeight = Screen.height / 2f;
        halfWidth = Screen.width / 2f;
        
        if (Rotate)
            handRotationHolder.localRotation = Quaternion.Euler(0, 0, normalZRotation);

        if (Scale)
            handScaleHolder.localScale = Vector3.one * normalScale;

        if (startVisible)
        {
            isHandVisible = true;
            handHolderCanvasGroup.alpha = 1;
        }
        else
        {
            isHandVisible = false;
            handHolderCanvasGroup.alpha = 0;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(showHideKey))
        {
            if (isHandVisible)
            {
                handHolderCanvasGroup.DOFade(0, showHideAnimTime);
                isHandVisible = false;
            }
            else
            {
                handHolderCanvasGroup.DOFade(1, showHideAnimTime);
                isHandVisible = true;
            }
        }

        if (Input.GetKeyDown(stopStartMouseFollowKey))
            follow = !follow;

        if (!follow)
            return;

        currentMousePos = Input.mousePosition;
        currentHandPos = new Vector2(currentMousePos.x - halfWidth, currentMousePos.y - halfHeight);
        handPositionHolder.anchoredPosition = currentHandPos + posOffset;

        if (Input.GetMouseButtonDown(0))
        {
            if (Rotate)
                handRotationHolder.DOLocalRotate(new Vector3(0, 0, downZRotation), rotationAnimTime);
            if (Scale)
                handScaleHolder.DOScale(Vector3.one* downScale, scaleAnimTime);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Rotate)
                handRotationHolder.DOLocalRotate(new Vector3(0, 0, normalZRotation), rotationAnimTime);
            if (Scale)
                handScaleHolder.DOScale(Vector3.one * normalScale, scaleAnimTime);
        }
    }

    private void SetActiveHand()
    {
        for (int i = 0; i < HandsTR.Count; i++)
        {
            if (i == currentHandIndex)
            {
                HandsTR[i].gameObject.SetActive(true);
                handRotationHolder = HandsTR[i];
            }
            else
                HandsTR[i].gameObject.SetActive(false);
        }
    }
}
