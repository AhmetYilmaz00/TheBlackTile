using Elympics;
using TMPro;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class GameManagerAim : ElympicsMonoBehaviour, IUpdatable, IInitializable
    {
        [SerializeField] private float timerDuration;
        [SerializeField] private DisplayManager displayManager;

        private ElympicsFloat timer = new ElympicsFloat();

        public void Initialize()
        {
            timer.Value = timerDuration;
        }

        public void ElympicsUpdate()
        {
            timer.Value -= Elympics.TickDuration;
            displayManager.DisplayTimer(timer.Value);
            if (timer.Value <= 0) EndGame();
        }

        private void EndGame()
        {
            
        }
    }
}