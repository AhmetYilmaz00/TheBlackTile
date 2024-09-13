using Elympics;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class PlayerInputControllerAim : ElympicsMonoBehaviour, IInputHandler, IUpdatable, IInitializable
    {
        // Input'lar
        public bool inputGetMouseDown;
        public bool inputGetMouse;
        public bool inputGetMouseUp;
        public float inputMousePositionX;
        public float inputMousePositionY;

        public bool serverInputGetMouseDown;
        public bool serverInputGetMouse;
        public bool serverInputGetMouseUp;
        public float serverInputMousePositionX;
        public float serverInputMousePositionY;

        public bool isServer;

        public void Initialize()
        {
            isServer = Elympics.IsServer;
        }

        public void OnInputForClient(IInputWriter inputSerializer)
        {
            // Mouse tıklaması, yatay hareket ve ateş etme input'larını yazıyoruz
            inputSerializer.Write(inputGetMouseDown); // Mouse tıklaması
            inputSerializer.Write(inputGetMouse); // Yatay hareket
            inputSerializer.Write(inputGetMouseUp); // Ateş etme
            inputSerializer.Write(inputMousePositionX);
            inputSerializer.Write(inputMousePositionY);

            inputGetMouseDown = false;
            inputGetMouse = false;
            inputGetMouseUp = false;
            inputMousePositionX = 0;
            inputMousePositionY = 0;
        }

        public void OnInputForBot(IInputWriter inputSerializer)
        {
            // Bot için input verileri burada yazılabilir
        }


        public void ElympicsUpdate()
        {
            if (Elympics.IsServer)
            {
                // Verileri okuma
                bool readInputMouseDown;
                bool readInputMouse;
                bool readInputMouseUp;
                float readInputMousePositionX;
                float readInputMousePositionY;

                if (ElympicsBehaviour.TryGetInput(ElympicsPlayer.FromIndex(0), out var inputReader))
                {
                    // Verileri sırasıyla okuyoruz
                    inputReader.Read(out readInputMouseDown); // Mouse tıklaması
                    inputReader.Read(out readInputMouse); // Yatay hareket
                    inputReader.Read(out readInputMouseUp); // Ateş etme
                    inputReader.Read(out readInputMousePositionX); // Ateş etme
                    inputReader.Read(out readInputMousePositionY); // Ateş etme

                    serverInputGetMouseDown = readInputMouseDown;
                    serverInputGetMouse = readInputMouse;
                    serverInputGetMouseUp = readInputMouseUp;
                    serverInputMousePositionX = readInputMousePositionX;
                    serverInputMousePositionY = readInputMousePositionY;

                    Debug.Log("serverInputGetMouseDown: " + serverInputGetMouseDown);
                    Debug.Log("readInputMousePositionX: " + readInputMousePositionX);
                    Debug.Log("readInputMousePositionY: " + readInputMousePositionY);
                    Debug.Log("serverInputGetMouseUp: " + serverInputGetMouseUp);
                }
            }
        }
    }
}