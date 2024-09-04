using Array2DEditor;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialManager : SingletonBehaviour<TutorialManager>
{
    public TutorialGridManager GridManager;
    public CanvasGroup[] Tutorials;
    public Animation TutorialTextAnim;

    public List<TutorialPath> Paths;

    private bool _isTutorial;
    private BBTime _timer;

    public void StartTutorial()
    {
        _isTutorial = true;
        GridManager.GenerteGrid(0);

        ShowTutorialCanvas(Tutorials[0]);
    }

    public void OnTutorialStageDone(int stage)
    {
        HideTutorialCanvas(Tutorials[stage], 0);
        ShowTutorialCanvas(Tutorials[stage + 1]);

        if(stage == 2)
        {
            HideTutorialCanvas(Tutorials[3], 1.5f);
            this.Invoke(() => TutorialGridManager.instance.DestroyGrid(), 1.6f);
            Invoke(nameof(EndTutorial), 2f);
        }
    }

    private void EndTutorial()
    {
        GameManager.instance.Progress.tutorialDone = true;
        GameManager.instance.StartLevel();
    }

   
    private void ShowTutorialCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 0;
        canvas.gameObject.SetActive(true);
        canvas.DOFade(1, 0.2f);
    }

    private void HideTutorialCanvas(CanvasGroup canvas, float delay)
    {
        this.Invoke(() =>
            canvas.DOFade(0, 0.2f).OnComplete(() => canvas.gameObject.SetActive(false)), delay);
    }

    internal void OnGridGenerated(int gridNumber)
    {
        TutorialHandController.instance.StopAnimation();
        List<Vector3> worldPositions = new List<Vector3>();

        for (int i = 0; i < Paths[gridNumber].Points.Count; i++)
        {
            worldPositions.Add(GridManager.GetBlock(Paths[gridNumber].Points[i]).transform.position + Vector3.back);
        }

        TutorialHandController.instance.PressMoveSequenceAndReleaseAnim(worldPositions);
    }

    [Serializable]
    public struct TutorialPath
    {
        public List<Vector2Int> Points;
    }
}
