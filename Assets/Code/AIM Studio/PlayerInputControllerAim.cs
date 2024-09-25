using System;
using Elympics;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class PlayerInputControllerAim : ElympicsMonoBehaviour, IInputHandler, IUpdatable, IInitializable
    {
        // Input'lar
        /// <summary>
        /// 0 = none, 1 = down, 2 = Up
        /// </summary>
        public int mouseButtonState;

        public bool inputGetMouse;
        public float inputMousePositionX;
        public float inputMousePositionY;

        public int serverMouseButtonState;
        public bool serverInputGetMouse;
        public float serverInputMousePositionX;
        public float serverInputMousePositionY;

        public bool isServer;
        private InputManager _inputManager;

        private void Start()
        {
            _inputManager = InputManager.instance;
        }

        public void Initialize()
        {
            isServer = Elympics.IsServer;
            _inputManager = InputManager.instance;
        }

        public void OnInputForClient(IInputWriter inputSerializer)
        {
            // Mouse tıklaması, yatay hareket ve ateş etme input'larını yazıyoruz
            inputSerializer.Write(mouseButtonState); // Mouse tıklaması
            inputSerializer.Write(inputGetMouse); // Yatay hareket
            inputSerializer.Write(inputMousePositionX);
            inputSerializer.Write(inputMousePositionY);
            mouseButtonState = 0;
        }

        public void OnInputForBot(IInputWriter inputSerializer)
        {
        }

        private void Update()
        {
            if (_inputManager.inputWait)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseButtonState = 1;
                //Log("Mouse Button Down Detected");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseButtonState = 2;
                //Log("Mouse Button Up Detected");
            }
        }


        public void ElympicsUpdate()
        {
            if (Elympics.IsServer)
            {
                // Verileri okuma
                int readMouseButtonState;
                bool readInputMouse;
                float readInputMousePositionX;
                float readInputMousePositionY;

                if (ElympicsBehaviour.TryGetInput(ElympicsPlayer.FromIndex(0), out var inputReader))
                {
                    // Verileri sırasıyla okuyoruz
                    inputReader.Read(out readMouseButtonState); // Mouse tıklaması
                    inputReader.Read(out readInputMouse); // Yatay hareket
                    inputReader.Read(out readInputMousePositionX); // Ateş etme
                    inputReader.Read(out readInputMousePositionY); // Ateş etme

                    serverMouseButtonState = readMouseButtonState;
                    Debug.Log("serverMouseButtonState: " + serverMouseButtonState);
                    serverInputGetMouse = readInputMouse;
                    serverInputMousePositionX = readInputMousePositionX;
                    serverInputMousePositionY = readInputMousePositionY;
                }
            }
        }
    }
}