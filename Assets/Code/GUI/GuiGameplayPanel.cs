using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GuiGameplayPanel : SingletonBehaviour<GuiGameplayPanel>
{
    public TMP_Text ScoreLabel;
    public TMP_Text ScoreText;
    public TMP_Text BestScoreLabel;
    public TMP_Text BestScoreText;
    public Image Line;

    public TMP_Text Debug_PlayTime;

    public RectTransform[] SelectionLengthFeedbacks;
    public float FeedbackObjectWidth;
    public float FeedbackAnimTime;

    public bool AnimsInProgress { get; private set; }
    public int Score { get; private set; }

    private int _bestScore;
    private int _multiplier;

    private void OnEnable()
    {
        Messenger<Block>.AddListener(Message.OnBlockMerged, OnBlockMerged);
        Messenger<List<Block>>.AddListener(Message.OnMergeMoveStarted, OnMergeMoveStarted);

        _bestScore = GameManager.instance.Progress.highScore;
        if (GameManager.instance.Progress.isLevelDataSaved)
        {
            Score = GameManager.instance.Progress.levelData.Score;
            ScoreText.text = Score.ToString();
            if(Score > _bestScore)
            {
                BestScoreText.text = ScoreText.text;
            }
            else
            {
                BestScoreText.text = _bestScore.ToString();
            }
        }
        else
        {
            ScoreText.text = "0";
            BestScoreText.text = _bestScore.ToString();
        }

        ScoreLabel.color = AssetsConfiguration.instance.ColorTheme.GuiColor_1;
        ScoreText.color = AssetsConfiguration.instance.ColorTheme.GuiColor_1;
        BestScoreLabel.color = AssetsConfiguration.instance.ColorTheme.GuiColor_2;
        BestScoreText.color = AssetsConfiguration.instance.ColorTheme.GuiColor_2;
        Line.color = AssetsConfiguration.instance.ColorTheme.GuiColor_3;
    }

    private void OnDisable()
    {
        Messenger<Block>.RemoveListener(Message.OnBlockMerged, OnBlockMerged);
        Messenger<List<Block>>.RemoveListener(Message.OnMergeMoveStarted, OnMergeMoveStarted);
    }

    private void Update()
    {
        Debug_PlayTime.text = LevelManager.instance._debugPlaytime.ToString("#");
    }

    private void OnMergeMoveStarted(List<Block> selectedBlocks)
    {
        _multiplier = 1;

        int selectionFeedbackIndex = -1;
        if(selectedBlocks.Count > 7)
            selectionFeedbackIndex = 0;
        if (selectedBlocks.Count >= 12)
            selectionFeedbackIndex = 1;
        if (selectedBlocks.Count >= 18)
            selectionFeedbackIndex = 2;
        if (selectedBlocks.Count >= 30)
            selectionFeedbackIndex = 3;

        if (selectionFeedbackIndex == -1)
            return;

        var feedbackObject = SelectionLengthFeedbacks[selectionFeedbackIndex];
        feedbackObject.gameObject.SetActive(true);
        feedbackObject.position = Camera.main.WorldToScreenPoint(selectedBlocks.Last().transform.position);
        if(feedbackObject.position.x - (FeedbackObjectWidth / 2f) < 0)
        {
            var pos = feedbackObject.position;
            pos.x = FeedbackObjectWidth / 2f;
            feedbackObject.position = pos;
        }
        else if(feedbackObject.position.x + (FeedbackObjectWidth / 2) > Screen.width)
        {
            var pos = feedbackObject.position;
            pos.x = Screen.width - (FeedbackObjectWidth / 2f);
            feedbackObject.position = pos;
        }

        this.Invoke(() => feedbackObject.gameObject.SetActive(false), FeedbackAnimTime);
    }

    private void OnBlockMerged(Block mergedBlock)
    {
        AddToScore(Mathf.Abs(mergedBlock.Number) * _multiplier);
        _multiplier++;
    }

    private void AddToScore(int additionalScore)
    {
        Score += additionalScore;
        ScoreText.text = Score.ToString();
        if(Score > _bestScore)
        {
            BestScoreText.text = Score.ToString();
        }
    }
}