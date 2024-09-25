using System;
using System.Collections;
using Code.AIM_Studio;
using Code.GUI;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField] private GameManagerAim gameManagerAim;
    public PlayerProgress Progress;


    private GameState _previousGameState;

    public GameState PreviousGameState
    {
        get { return _previousGameState; }
    }

    [NaughtyAttributes.ReadOnly] [SerializeField] private GameState _gameState;

    public LevelData CurrentLevelData;


    public GameState GameState
    {
        get { return _gameState; }
        private set
        {
            _previousGameState = _gameState;
            Messenger<GameState, GameState>.Broadcast(Message.PreGameStateChange, value, _previousGameState);
            Debug.Log("_previousGameState: "+_previousGameState);
            _gameState = value;
            Messenger<GameState, GameState>.Broadcast(Message.PostGameStateChange, _gameState, _previousGameState);

            //Debug.Log("GameManager state change from " + _previousGameState + " to " + _gameState);
        }
    }

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Progress = PlayerProgressHelper.ReadPlayerProgress<PlayerProgress>();
        Progress.FillData();
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        _gameState = GameState.Menu;
        GameState = GameState.Menu;

        if (Progress.isLevelDataSaved && Progress.levelData.Score > 0)
        {
          //  StartCoroutine(StartLevelFromLevelData(Progress.levelData));
        }
        // else if (!Progress.tutorialDone)
        // {
        //     GameState = GameState.Tutorial;
        //     TutorialManager.instance.StartTutorial();
        // }
        else
        {
            if (gameManagerAim.IsServer())
            {
                StartLevel();
            }
        }
    }


    public void StartLevel()
    {
        StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine()
    {
        DestroyCurrentLevel();
        LoadLevel();

        GameState = GameState.PreGameplay;

        yield return null;

        GameState = GameState.Gameplay;
    }

    private IEnumerator StartLevelFromLevelData(LevelData levelData)
    {
        LevelManager.instance.GenerateLevelFromData(levelData);

        GameState = GameState.PreGameplay;

        yield return null;

        GameState = GameState.Gameplay;
    }

    private void LoadLevel()
    {
        LevelManager.instance.GenerateLevel();
    }

    private void DestroyCurrentLevel()
    {
        LevelManager.instance.DestroyLevel();
    }

    public void GoToMenu()
    {
        GameState = GameState.Menu;
    }

    public void OnLevelWin()
    {
        GameState = GameState.GameOverWin;

        Progress.globalLevelDoneCount++;
        Progress.Level++;

        UpdateProgress();
        GoToMenu();
    }

    public void OnLevelLose()       
    {
        Progress.isLevelDataSaved = false;
        Progress.levelData = null;

        // if (GuiGameplayPanel.instance.Score > Progress.highScore)
        //     Progress.highScore = GuiGameplayPanel.instance.Score;

        GameState = GameState.GameOverLose;
    }

    public void UpdateProgress()
    {
        PlayerProgressHelper.SavePlayerProgress(Progress);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLevelData();
            OnQuitTimeSave();
            UpdateProgress();
        }
    }

    private void OnApplicationQuit()
    {
        SaveLevelData();
        OnQuitTimeSave();
        UpdateProgress();
    }

    private void SaveLevelData()
    {
        if (GameState == GameState.Gameplay && GuiGameplayPanel.instance.Score > 0)
        {
            Progress.isLevelDataSaved = true;
            Progress.levelData = CurrentLevelData;
        }
        else
        {
            Progress.isLevelDataSaved = false;
            Progress.levelData = null;
        }

        Progress.playTime = LevelManager.instance._debugPlaytime;
    }

    public void SaveToLevelData(LevelData levelData)
    {
        CurrentLevelData = levelData;
    }

    private void OnQuitTimeSave()
    {
        Progress.appQuitTime = (DateTime.Now - new DateTime()).TotalMinutes;
    }
}