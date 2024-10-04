using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.AIM_Studio;
using Code.GUI;
using Code.Managers;
using UnityEngine;

public class LevelManager : SingletonBehaviour<LevelManager>
{
    public bool CanSpawnMultiplier { get; private set; }

    private BBTime _multiplierTimer;

    public float _debugPlaytime;

    private void OnEnable()
    {
        Messenger<GameState, GameState>.AddListener(Message.PostGameStateChange, OnGameStatePreChange);

        Camera.main.backgroundColor = AssetsConfiguration.instance.ColorTheme.BackgroundColor;

        _debugPlaytime = GameManager.instance.Progress.playTime;
    }

    private void OnDisable()
    {
        Messenger<GameState, GameState>.RemoveListener(Message.PostGameStateChange, OnGameStatePreChange);
    }

    public void OnGameStatePreChange(GameState newGameState, GameState previousGameState)
    {
        if (newGameState == GameState.Gameplay)
        {
            OnMultiplierSpawned();
        }
    }

    private void Update()
    {
        _debugPlaytime += Time.deltaTime;

        if (GameManager.instance.GameState != GameState.Gameplay)
            return;

        CanSpawnMultiplier = _multiplierTimer.Passed();

        if (GuiGameplayPanel.instance == null)
            return;

    }

    internal void OnMultiplierSpawned()
    {
        _multiplierTimer.Set(Time.time * 5 * 60);
    }

    #region Level Generation

    public void GenerateLevel()
    {
        FindObjectOfType<GridManager>().GenerteGridRandom();
        //GridManager.instance.GenerteGridRandom();
    }

    public void GenerateLevelFromData(LevelData levelData)
    {
        GridManager.instance.GenerteGridFromData(levelData);
    }

    public void DestroyLevel()
    {
        var childen = transform.BB_GetFirstLevelChildren(true);
        for (int i = 0; i < childen.Length; i++)
        {
            Destroy(childen[i].gameObject);
        }

        GridManager.instance.DestroyCurrentLevel();
    }

    #endregion
}