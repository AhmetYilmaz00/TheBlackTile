using System;
using System.Collections.Generic;
using System.Linq;
using Elympics;
using ElympicsLobbyPackage;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class GameManagerAim : ElympicsMonoBehaviour, IUpdatable, IInitializable
    {
        public ElympicsInt score;


        [SerializeField] private float timerDuration;
        [SerializeField] private DisplayManager displayManager;
        public ElympicsArray<ElympicsInt> seedArray = new();
        public bool updateGridControlClient;

        public int currentHandBlocks = 0;
        private ElympicsFloat timer = new();
        private int _seedCounter;

        public List<int> seedArrayClient = new();
        private bool _isServer;
        private bool _isFinishGame;


        private void Start()
        {
            Debug.Log("GameManagerAimStart");
            Debug.Log("Start Elympics.IsClient=" + Elympics.IsClient);
        }

        public void Initialize()
        {
            timer.Value = timerDuration;
            seedArray = new ElympicsArray<ElympicsInt>(50, () => new ElympicsInt());
            _isServer = Elympics.IsServer;
            Debug.Log("Elympics.IsClient=" + Elympics.IsClient);
            // if (Elympics.IsClient)
            // {
            //     if (ElympicsExternalCommunicator.Instance != null)
            //         ElympicsExternalCommunicator.Instance.gameObject.GetComponent<PersistentLobbyManager>()
            //             .SetAppState(PersistentLobbyManager.AppState.Gameplay);
            // }
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
            return _isServer;
        }

        public void ClearAllData()
        {
            _seedCounter = 0;
            if (IsServer())
            {
                for (int i = 0; i < seedArray.Values.Count; i++)
                {
                    seedArray.Values[i].Value = 0;
                }
            }
            else
            {
                for (int i = 0; i < seedArray.Values.Count; i++)
                {
                    seedArrayClient[i] = 0;
                }
            }
        }

        public void ElympicsUpdate()
        {
            if (Elympics.IsClient && seedArrayClient.Count <= 0)
            {
                foreach (var value in seedArray.Values)
                {
                    seedArrayClient.Add(value);
                    Debug.Log("value: " + value);
                    Debug.Log(value);
                }

                GameManager.instance.StartLevel();
                ClearAllData();
            }

            if (timer.Value <= 0 && !_isFinishGame)
            {
                timer.Value = 0;
                _isFinishGame = true;
                EndGame();
            }
            else if (timer.Value > 0)
            {
                timer.Value -= Elympics.TickDuration;
                displayManager.DisplayTimer(timer.Value);
            }
        }

        public void RepairSeedArray()
        {
            seedArrayClient = new List<int>();
            foreach (var value in seedArray.Values)
            {
                seedArrayClient.Add(value);
                Debug.Log(value);
            }

            currentHandBlocks = 0;
        }

        private void EndGame()
        {
            if (Elympics.IsServer)
            {
                Elympics.EndGame(new ResultMatchPlayerDatas(new List<ResultMatchPlayerData>
                    { new ResultMatchPlayerData { MatchmakerData = new float[1] { score.Value } } }));
                
            }

            GameManager.instance.OnLevelLose();
            if (Elympics.IsClient)
            {
                ElympicsExternalCommunicator.Instance.GameStatusCommunicator.GameFinished(score.Value);
            }
        }
    }
}