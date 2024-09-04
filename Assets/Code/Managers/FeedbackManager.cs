using System;
using System.Collections;
using UnityEngine;

public class FeedbackManager : SingletonBehaviour<FeedbackManager>
{
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void LightFeedback()
    {
        Taptic.Light();
    }

    public void GoodFeeback()
    {
        Taptic.Success();
    }

    public void BadFeedback()
    {
        Taptic.Failure();
    }
}