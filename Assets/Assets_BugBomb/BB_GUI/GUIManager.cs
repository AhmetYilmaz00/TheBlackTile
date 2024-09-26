using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class GUIManager : SingletonBehaviour<GUIManager>
{
    public List<GUIScreen> AllOpenedUIScreens;
    public SingletonGameEvent onLoadedGameSceneUI;
    public GameObject gameOverPanel;

    private GUIScreenType _currentOpenScreen;

    private void OnEnable()
    {
        Messenger<GameState, GameState>.AddListener(Message.PreGameStateChange, OnGameStatePreChange);
    }

    private void OnDisable()
    {
        Messenger<GameState, GameState>.RemoveListener(Message.PreGameStateChange, OnGameStatePreChange);
    }

    public void OnGameStatePreChange(GameState newGameState, GameState previousGameState)
    {
        switch (newGameState)
        {
            case GameState.Menu:
                break;
            case GameState.PreGameplay:
                break;
            case GameState.Gameplay:
                OpenOverlayScreen(GUIScreenType.Gameplay);
                onLoadedGameSceneUI.Raise();
                break;
            case GameState.Pause:
                break;
            case GameState.Settings:
                break;
            case GameState.GameOverWin:
                break;
            case GameState.GameOverLose:
                gameOverPanel.SetActive(true);
                // OpenOverlayScreen(GUIScreenType.GameoverLose);
                break;
        }
    }

    public void OpenOverlayScreen(GUIScreenType screenType)
    {
        RemoveGUIScreen(_currentOpenScreen);
        GetGUIScreen(screenType);
        _currentOpenScreen = screenType;
    }

    public GUIScreen GetGUIScreen(GUIScreenType _guiScreenType, bool autoSetup = true, bool autoOpen = true)
    {
        // if its open, return it
        for (int i = 0; i < AllOpenedUIScreens.Count; i++)
        {
            if (AllOpenedUIScreens[i].GUIScreenType == _guiScreenType)
                return AllOpenedUIScreens[i];
        }

        // spawn prefab and return
        GUIScreen gsPrefab = GUIConfiguration.instance.GetGUIScreen(_guiScreenType);
        if (gsPrefab == null)
        {
            Debug.LogError($"No GUIScreen prefab with _panelType: {_guiScreenType}");
            return null;
        }

        GUIScreen gs = Instantiate(gsPrefab);
        AllOpenedUIScreens.Add(gs);

        if (autoSetup)
            gs.SetupPopup();
        if (autoOpen)
            gs.OpenPopup();

        return gs;
    }

    internal void RemoveGUIScreen(GUIScreenType _guiScreenType)
    {
        for (int i = 0; i < AllOpenedUIScreens.Count; i++)
        {
            if (AllOpenedUIScreens[i].GUIScreenType == _guiScreenType)
            {
                AllOpenedUIScreens[i].ClosePopup();
                AllOpenedUIScreens.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// Method called by all buttons
    /// </summary>
    public void ButtonOnClick()
    {
        //BB_GUIEventSystemLock.instance.LockForTime();
        //FeedbackManager.instance.LightFeedback();

        // odtwórz dźwięk
        //SoundManager.instance.PlayOnClickSfx();

        // inne
    }
}