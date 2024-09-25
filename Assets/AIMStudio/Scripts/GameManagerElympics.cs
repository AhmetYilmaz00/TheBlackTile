using System;
using System.Collections;
using System.Collections.Generic;
using Elympics;
using UnityEngine;

namespace AIMStudio.Scripts
{
    public class GameManagerElympics : ElympicsMonoBehaviour, IServerHandlerGuid, IUpdatable
    {
        public enum GameOverReason
        {
            Moves,
            Time,
            Disconnect
        }

        public enum GameState
        {
            menu,
            game,
            gameover,
            shop
        }

        public readonly Dictionary<string, int> parameters = new Dictionary<string, int>
        {
            { "match_timer", 180 },
            { "place_bonus", 1 },
            { "points_per_block", 10 },
            { "clear_line_bonus", 50 },
            { "combo_bonus", 500 },
            { "streak_bonus", 200 },
            { "remove_amount", 3 },
        };

        public static GameManagerElympics instance;
        public event Action GameStarted;
        public event Action GameEnded;

        private float _elapsedMatchTime;

        public GameState gameState;

        public bool IsPlaying
        {
            get { return gameState == GameState.game; }
        }


        public bool canPointerDown { get; set; } = true;

        public bool WasLastTurnClearLine { get; set; }

        private ElympicsBool isGameStarted = new(false);
        private bool isReadyLocally = false;
        private readonly HashSet<ElympicsPlayer> playersConnected = new HashSet<ElympicsPlayer>();
        private int _playersNumber;
        private bool NotAllPlayersConnected => playersConnected.Count != _playersNumber;

        [SerializeField] private RandomManager randomManager;
        private ElympicsFloat timeToStart = new ElympicsFloat(3);
        private Coroutine countdownSound;

        private void Awake()
        {
            GameStarted += () => Debug.Log("All players ready");
            Application.targetFrameRate = 60;
            instance = this;
        }

        public void ElympicsUpdate()
        {
            if (isReadyLocally) return;

            if (!isGameStarted.Value) return;

            if (countdownSound == null && Elympics.IsClient)
            {
                countdownSound = StartCoroutine(CountDownSound());
            }

            randomManager.InitializeRandom();

            GUIManagerElympics.instance.gameOverGUI.Hide();
            GUIManagerElympics.instance.frontPage.SetActive(false);

            timeToStart.Value -= Elympics.TickDuration;

            if (timeToStart.Value > 0f)
            {
                GUIManagerElympics.instance.countDownText.text = Mathf.Ceil(timeToStart.Value).ToString();
            }

            if (timeToStart.Value <= 0f)
            {
                GUIManagerElympics.instance.countDownTextContainer.SetActive(false);

                StartGame();

                isReadyLocally = true;

                if (Elympics.IsClient && countdownSound != null)
                {
                    StopCoroutine(countdownSound);
                    SoundsManager.instance.PlayGameStartSound();
                    countdownSound = null;
                }
            }
        }

        IEnumerator CountDownSound()
        {
            while (!isReadyLocally)
            {
                SoundsManager.instance.PlayCountdown0();
                yield return new WaitForSecondsRealtime(1);
            }
        }

        public void OnServerInit(InitialMatchPlayerDatasGuid initialMatchPlayerDatas)
        {
            randomManager.SetSeed(UnityEngine.Random.Range(1, 100000));
            _playersNumber = initialMatchPlayerDatas.Count;
        }

        public void OnPlayerDisconnected(ElympicsPlayer player)
        {
            GameOver(GameOverReason.Disconnect);
            ElympicsAuthenticationHandler.instance.DisconnectMessageBox();
        }

        public void OnPlayerConnected(ElympicsPlayer player)
        {
            if (!IsEnabledAndActive) return;

            playersConnected.Add(player);

            if (NotAllPlayersConnected) return;

            isGameStarted.Value = true;
        }

        public void StartGame(bool resetScore = true, bool resetOneMoreChance = true)
        {
            ResetGame();

            gameState = GameState.game;
            GameStarted?.Invoke();

            _elapsedMatchTime = Time.time;
        }

        public void ResetGame(bool resetScore = true)
        {
            SoundsManager.instance.PlayGameStartSound();

            if (resetScore)
            {
                ScoreManager.instance.reset();
            }

            //   GameGrid.instance.ClearGrid();
            //   GameGrid.instance.RefreshGridCounters();

            //  TetrimoSpawner.instance.Reset();

            GUIManagerElympics.instance.gameOverGUI.Hide();
            GUIManagerElympics.instance.frontPage.SetActive(false);
        }

        public void GameOver(GameOverReason reason)
        {
            if (gameState != GameState.game)
                return;

            _elapsedMatchTime = Time.time - _elapsedMatchTime;

            gameState = GameState.gameover;
            SoundsManager.instance.PlayGameOverSound();
            GameEnded?.Invoke();

            if (Elympics.IsServer)
            {
                Elympics.EndGame(new ResultMatchPlayerDatas(new List<ResultMatchPlayerData>
                    { new ResultMatchPlayerData { MatchmakerData = new float[1] { ScoreManager.instance.score } } }));
            }

            if (reason != GameOverReason.Disconnect)
            {
                GUIManagerElympics.instance.gameOverGUI.Show(reason);
            }
        }

        public void Disconnect()
        {
            Elympics.Disconnect();
        }
    }
}