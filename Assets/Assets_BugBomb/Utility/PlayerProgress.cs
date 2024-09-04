using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerProgress
{
    public int Level;
    public int GUILevel { get { return Level + 1; } }
    public int NextGUILevel { get { return Level + 2; } }
    public int globalLevelDoneCount;
    public bool tutorialDone;

    public int currency;
    public int highScore;

    // settings
    public bool musicOn;
    public bool sfxOn;
    public bool hapticOn;

    // app behaviours
    public double appQuitTime;
    public DateTime appQuitDateTime;
    public float playTime;

    // tutorial
    public int finishedTutorials = 0;

    // data
    public bool isLevelDataSaved;
    public LevelData levelData;

    public PlayerProgress()
    {
        Level = 0;
        globalLevelDoneCount = 0;

        tutorialDone = false;

        currency = 0;

        musicOn = true;
        sfxOn = true;
        hapticOn = true;


        appQuitTime = 0;
        FillData();

        finishedTutorials = 0;
    }

    internal void FillData()
    {
        appQuitDateTime = new DateTime().AddMinutes(appQuitTime);
    }

    internal void AddCoins(int newCoins)
    {
        currency += newCoins;
    }

    public void SpendCoins(int c)
    {
        currency -= c;
        if (currency < 0)
            currency = 0;
    }
}
