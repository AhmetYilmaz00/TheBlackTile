using System;
using Code.AIM_Studio;
using Elympics;
using UnityEngine;

public class PlayerInputControllerAim : ElympicsMonoBehaviour, IInputHandler, IUpdatable
{
    public static PlayerInputControllerAim instance;


    /// <summary>
    /// 0 = none, 1 = down, 2 = Up
    /// </summary>
    public int mouseButtonState;

    public float mousePositionX;
    public float mousePositionY;
    public float screenPositionX;
    public float screenPositionY;
    public bool isDraggingState;

    private GameManagerAim _gameManagerAim;
    public bool IsDragging => isDraggingState;
    public Vector3 MousePosition => new Vector3(mousePositionX, mousePositionY, 0);
    public Vector3 ScreenPosition => new Vector3(screenPositionX, screenPositionY, 0);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseButtonState = 1;
            //Log("Mouse Button Down Detected");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseButtonState = 2;
            isDraggingState = false;

            //Log("Mouse Button Up Detected");
        }

        if (Input.GetMouseButton(0))
        {
            isDraggingState = true;
        }

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePositionX = Input.mousePosition.x;
        mousePositionY = Input.mousePosition.y;
        screenPositionX = screenPos.x;
        screenPositionY = screenPos.y;
    }

    private void Start()
    {
        _gameManagerAim = FindObjectOfType<GameManagerAim>();
    }

    public void ElympicsUpdate()
    {
        if (ElympicsBehaviour.TryGetInput(PredictableFor, out IInputReader inputReader))
        {
            inputReader.Read(out mouseButtonState);
            inputReader.Read(out mousePositionX);
            inputReader.Read(out mousePositionX);
            inputReader.Read(out screenPositionX);
            inputReader.Read(out screenPositionY);
            inputReader.Read(out isDraggingState);

            //Log($"Elympics Update - MouseButtonState: {mouseButtonState}, MousePosition: {mousePositionX}, {mousePositionY}, IsDragging: {isDraggingState}");
        }

        if (_gameManagerAim.IsServer())
        {
            if (mouseButtonState == 1 || mouseButtonState == 2)
            {
                _gameManagerAim.DebugString.Values[11].Value = mouseButtonState.ToString();
            }

            if (mousePositionX > 0)
            {
                _gameManagerAim.DebugString.Values[12].Value = mousePositionX.ToString();
            }

            if (mousePositionY > 0)
            {
                _gameManagerAim.DebugString.Values[13].Value = mousePositionY.ToString();
            }

            if (isDraggingState)
            {
                _gameManagerAim.DebugString.Values[13].Value = isDraggingState.ToString();
            }
        }
    }

    public void OnInputForClient(IInputWriter inputWriter)
    {
        inputWriter.Write(mouseButtonState);
        inputWriter.Write(mousePositionX);
        inputWriter.Write(mousePositionY);
        inputWriter.Write(screenPositionX);
        inputWriter.Write(screenPositionY);
        inputWriter.Write(isDraggingState);

        //Log($"Input For Client - MouseButtonState: {mouseButtonState}, MousePosition: {mousePositionX}, {mousePositionY}, IsDragging: {isDraggingState}");

        mouseButtonState = 0;
    }

    public void StartDrag()
    {
        isDraggingState = true;
        //Log("Start Drag");
    }

    public void StopDrag()
    {
        isDraggingState = false;
        mouseButtonState = 0;
        //Log("Stop Drag");
    }

    public void ResetMouseState()
    {
        mouseButtonState = 0;
    }

    public void OnInputForBot(IInputWriter inputSerializer)
    {
        //Do nothing, game is single player.
    }

    void Log(string text)
    {
        Debug.LogWarning(text);
    }
}