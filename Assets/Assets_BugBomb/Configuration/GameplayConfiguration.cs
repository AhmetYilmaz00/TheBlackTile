using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Gameplay", menuName = "Configuration/Gameplay", order = 2)]
[ExecuteInEditMode]
public class GameplayConfiguration : Configuration
{
    [Header("Base")]
    public bool PreloadLevelOnStartup;

    [Header("Grid")]
    public Vector2Int GridSize;
    public Vector2 CellSize;

    [Header("Input")]
    public Vector3 LineOffset;

    [Header("Timings")]
    public float BlocksMergeSpeed;
    public float BlocksFallTime;
    public AnimationCurve BlocksFallEase;
    [Space(5)]
    public bool FractureNormalBlocks;
    public float FractureExplosionForce;

    [Header("Difficulty")]
    public int EarlyGameMin;
    public int EarlyGameMax;
    public int EarlyGameLength;
    public AnimationCurve StartingDifficultyCurve;
    public int LateGameMin;
    public int LateGameMax;
    public int LateGameLoop;
    public AnimationCurve LateGateDiffucultyCurve;
    public Vector2 DifficultyVariationPercentage;

    [Button]
    public void ClearAllGameFiles()
    {
        var saveFilePath = PlayerProgressHelper.SaveFilePath;
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        PlayerPrefs.DeleteAll();
        Debug.Log("All data cleared!");
    }


    private static GameplayConfiguration _instance;

    public static GameplayConfiguration instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Load<GameplayConfiguration>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}
