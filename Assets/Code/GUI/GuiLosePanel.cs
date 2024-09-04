using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GuiLosePanel : MonoBehaviour
{
    public Button TryAgainButton;
    public TMP_Text ScoreText;
    public TMP_Text HighScoreText;

    private void Awake()
    {
        TryAgainButton.onClick.AddListener(Restart);
        ScoreText.text = LevelManager.instance.Score.ToString();
        HighScoreText.text = GameManager.instance.Progress.highScore.ToString();
    }

    private void Restart()
    {
        GameManager.instance.StartLevel();
    }
}