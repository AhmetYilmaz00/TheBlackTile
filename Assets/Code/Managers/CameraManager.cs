using UnityEngine;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;

public class CameraManager : SingletonBehaviour<CameraManager>
{
    public Camera Camera;

    public float Multiplier;

    private Vector3 _defaultCameraPos;
    private Vector3 _cameraDir;
    [SerializeField]
    [ReadOnly]
    private float _defaultCameraDistance;

    private void OnEnable()
    {
        Messenger<GameState, GameState>.AddListener(Message.PostGameStateChange, OnGameStateChange);
    }

    private void OnDisable()
    {
        Messenger<GameState, GameState>.RemoveListener(Message.PostGameStateChange, OnGameStateChange);
    }

    private void Awake()
    {
        _defaultCameraPos = Camera.transform.position;
        _defaultCameraDistance = _defaultCameraPos.magnitude;
        _cameraDir = _defaultCameraPos.normalized;
    }

    private void Update()
    {
    }

    private void OnGameStateChange(GameState gameState, GameState previousGameState)
    {
        switch (gameState)
        {
            case GameState.SplashScreen:
                break;
            case GameState.Menu:
                break;
            case GameState.PreGameplay:
                break;
            case GameState.Gameplay:
                break;
            case GameState.GameOverWin:
                break;
            case GameState.GameOverLose:
                break;
            default:
                break;
        }
    }

    [Button]
    public void Debug_Size()
    {
        SetCameraSize(0);
    }

    public void SetCameraSize(float additionalSize)
    {
        var defualtSize = _defaultCameraDistance + additionalSize;
        float finalSize = defualtSize;

        var ratio = (float)Screen.width / Screen.height;
        if (ratio < 0.56f)
        {
            finalSize = ratio.Remap(0.486f, 0.562f, defualtSize * Multiplier, defualtSize);
        }

        Camera.transform.position = _cameraDir * finalSize;
    }

    private float GetExtraDistance(float size, float multiplier)
    {
        float distanceFor_16_9 = 0;

        if (size == 7)
            distanceFor_16_9 = 0;
        else if (size == 8)
            distanceFor_16_9 = 4f;
        else if (size == 9)
            distanceFor_16_9 = 10f;
        else if (size == 10)
            distanceFor_16_9 = 15f;
        if (size == 11)
            distanceFor_16_9 = 23f;

        return distanceFor_16_9 * multiplier;
    }
}
