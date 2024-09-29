using System;
using System.Collections.Generic;
using System.Linq;
using Elympics;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class GameManagerAim : ElympicsMonoBehaviour, IUpdatable, IInitializable
    {
        public ElympicsInt score = new();
        public int totalMoveCount;


        [SerializeField] private float timerDuration;
        public ElympicsArray<ElympicsInt> seedArray = new();
        public bool updateGridControlClient;
        public ElympicsArray<ElympicsString> DebugString = new();
        private DisplayManager displayManager;


        public int currentHandBlocks = 0;
        private ElympicsFloat timer = new();
        private int _seedCounter;

        public List<int> seedArrayClient = new();
        private bool _isServer;
        private bool _isFinishGame;

        public Guid matchID;


        private void OnMatchDataReceived(MatchDataReceivedArgs args)
        {
            matchID = args.MatchId;
            Debug.Log("Match data received: " + matchID); // args içindeki verilere göre işlemler yapılır
        }

        private void Start()
        {
            displayManager = FindObjectOfType<DisplayManager>();
            ElympicsLobbyClient.Instance.RoomsManager.MatchDataReceived += OnMatchDataReceived;
        }

        public void Initialize()
        {
            timer.Value = timerDuration;
            seedArray = new ElympicsArray<ElympicsInt>(50, () => new ElympicsInt());
            DebugString = new ElympicsArray<ElympicsString>(100, () => new ElympicsString());
            _isServer = Elympics.IsServer;
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
            if (_isServer)
            {
                generatedSeed = _seedCounter + DateTime.Now.Millisecond;

                seedArray.Values[_seedCounter].Value = generatedSeed;
                Debug.Log("Sunucu tarafından belirlenen seed: " + generatedSeed);
            }
            else
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

        bool AllElementsAreZero(ElympicsArray<ElympicsInt> list)
        {
            var tempList = list.Values.Select(obj => (int)obj).ToList();

            return !(tempList.All(element => element == 0));
        }

        public void ElympicsUpdate()
        {
            if (!IsServer() && seedArrayClient.Count <= 0 && AllElementsAreZero(seedArray))
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

            if (_isServer)
            {
                DebugString.Values[15].Value = "Score: " + score.Value;
            }
            else
            {
//                Debug.Log("timer: " + timer.Value);
            }

            if (timer.Value <= 0 && !_isFinishGame)
            {
                _isFinishGame = true;

                if (_isServer)
                {
                    timer.Value = 0;
                    EndGame();
                }
                else
                {
                    GameManager.instance.OnLevelLose();
                }
            }

            if (timer.Value > 0)
            {
                if (_isServer)
                {
                    timer.Value -= Elympics.TickDuration;
                }

                if (!_isServer)
                {
                    displayManager.DisplayTimer(timer.Value);
                }
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

        public void EndGame()
        {
            Elympics.EndGame(new ResultMatchPlayerDatas(new List<ResultMatchPlayerData>
                { new ResultMatchPlayerData { MatchmakerData = new float[1] { score.Value } } }));


            // if (Elympics.IsClient)
            // {
            //     ElympicsExternalCommunicator.Instance.GameStatusCommunicator.GameFinished(scoreLocal);
            // }
        }
    }
}