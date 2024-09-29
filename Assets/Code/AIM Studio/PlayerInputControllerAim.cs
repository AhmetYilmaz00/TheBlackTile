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


    public int serverMouseButtonState;
    public float serverMousePositionX;
    public float serverMousePositionY;
    public float serverScreenPositionX;
    public float serverScreenPositionY;
    public bool serverIsDraggingState;

    float oldXMin = 123f, oldXMax = 956f;
    float newXMin = 216f, newXMax = 424f;

    float oldYMin = 102f, oldYMax = 1637f;
    float newYMin = 27f, newYMax = 407f;


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

    float RescaleValue(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
    }


    public void ElympicsUpdate()
    {
        if (ElympicsBehaviour.TryGetInput(PredictableFor, out var inputReader))
        {
            inputReader.Read(out serverMouseButtonState);
            inputReader.Read(out serverMousePositionX);
            inputReader.Read(out serverMousePositionY);
            inputReader.Read(out serverScreenPositionX);
            inputReader.Read(out serverScreenPositionY);
            inputReader.Read(out serverIsDraggingState);

            //Log($"Elympics Update - MouseButtonState: {mouseButtonState}, MousePosition: {mousePositionX}, {mousePositionY}, IsDragging: {isDraggingState}");
        }
    }

    public void OnInputForClient(IInputWriter inputWriter)
    {
        if (Elympics.Player != PredictableFor)
            return;


        // X ve Y değerlerini dönüştürme
        float newX = RescaleValue(mousePositionX, oldXMin, oldXMax, newXMin, newXMax);
        float newY = RescaleValue(mousePositionY, oldYMin, oldYMax, newYMin, newYMax);


        inputWriter.Write(mouseButtonState);
        inputWriter.Write(newX);
        inputWriter.Write(newY);
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