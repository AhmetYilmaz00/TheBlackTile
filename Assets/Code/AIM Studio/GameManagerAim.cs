using System;
using System.Collections.Generic;
using Elympics;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class GameManagerAim : ElympicsMonoBehaviour, IUpdatable, IInitializable
    {
        public int seedModifier;
        [SerializeField] private float timerDuration;
        [SerializeField] private DisplayManager displayManager;
        public ElympicsArray<ElympicsInt> seedArray = new();

        private ElympicsFloat timer = new ElympicsFloat();
        private int _seedCounter;
        private List<int> seedArrayClient = new();

        public void Initialize()
        {
            timer.Value = timerDuration;
            seedArray = new ElympicsArray<ElympicsInt>(300, () => new ElympicsInt());
        }


        public int GetElympicsSeed()
        {
            var generatedSeed = 0;
            if (Elympics.IsServer)
            {
                generatedSeed = _seedCounter + DateTime.Now.Millisecond;
                seedArray.Values[_seedCounter].Value = generatedSeed;
                Debug.Log("Sunucu tarafından belirlenen seed: " + generatedSeed);
            }
            else if (Elympics.IsClient)
            {
                  generatedSeed = seedArrayClient[_seedCounter];
                Debug.Log("Client tarafından belirlenen seed: " + generatedSeed);
            }

            _seedCounter++;
            return generatedSeed;
        }

        public bool IsServer()
        {
            return Elympics.IsServer;
        }

        public void ElympicsUpdate()
        {
            if (Elympics.IsClient && seedArrayClient.Count <= 0)
            {
                foreach (var value in seedArray.Values)
                {
                    seedArrayClient.Add(value);
                    Debug.Log(value);
                }

                GameManager.instance.StartLevel();
            }

            timer.Value -= Elympics.TickDuration;
            displayManager.DisplayTimer(timer.Value);
            if (timer.Value <= 0) EndGame();
        }

        private void EndGame()
        {
        }
    }
}