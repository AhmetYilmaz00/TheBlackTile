using System.Collections;
using System.Linq;
using Elympics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AIMStudio.Scripts
{
    public class GameOverGUI : ElympicsMonoBehaviour
    {
        [SerializeField] private GameObject _outOfMoves;
        [SerializeField] private GameObject _outOfTime;

        [SerializeField] private TextMeshProUGUI _placedBlocksLabel;
        [SerializeField] private TextMeshProUGUI _placedBlocks;
        [SerializeField] private TextMeshProUGUI _clearedBlocksBonusLabel;
        [SerializeField] private TextMeshProUGUI _clearedBlocksBonus;
        [SerializeField] private TextMeshProUGUI _clearedLinesLabel;
        [SerializeField] private TextMeshProUGUI _clearedLines;
        [SerializeField] private TextMeshProUGUI _combosLabel;
        [SerializeField] private TextMeshProUGUI _combos;
        [SerializeField] private TextMeshProUGUI _streaksLabel;
        [SerializeField] private TextMeshProUGUI _streaks;
        [SerializeField] private TextMeshProUGUI _currentScore;
        [SerializeField] private TextMeshProUGUI _respectValueLabel;

        [SerializeField] private GameObject walletConnectedPanel;
        [SerializeField] private GameObject walletDisconnectedPanel;

        [SerializeField] private Image respectLoadingBar;
        [SerializeField] private Button submitButton;

        private Color highScoreColor = new Color32(165, 214, 167, 255);
        private Color normalColor = Color.white;
        private bool startCounting = false;
        private Animator _animator;
        private long respectPoints;

        private void Awake()
        {
            submitButton.onClick.AddListener(OnSubmit);
        }

        private void OnDestroy()
        {
            ScoreManager.instance.Increased -= UpdateValues;
        }

        public void Show(GameManagerElympics.GameOverReason endReason)
        {
            _outOfMoves.SetActive(endReason == GameManagerElympics.GameOverReason.Moves);
            _outOfTime.SetActive(endReason == GameManagerElympics.GameOverReason.Time);

            UpdateValues();

            gameObject.SetActive(true);
            GetRespect();
        }

        private async void GetRespect()
        {
            RespectService respectService = new RespectService(ElympicsLobbyClient.Instance, ElympicsConfig.Load());
            // we assume here that the game allows for being in only one match at time
            var matchId = ElympicsLobbyClient.Instance.RoomsManager.ListJoinedRooms().First().State.MatchmakingData!.MatchData!.MatchId;

            if (!ElympicsAuthenticationHandler.instance.IsGuest())
            {
                StartRespectLoadingAnimation();
                var respectValue = await respectService.GetRespectForMatch(matchId);
                StopRespectLoadingAnimation();
                respectPoints = respectValue.Respect;
                _respectValueLabel.text = respectPoints.ToString();

                walletConnectedPanel.SetActive(true);
                walletDisconnectedPanel.SetActive(false);
            }
            else
            {
                walletConnectedPanel.SetActive(false);
                walletDisconnectedPanel.SetActive(true);
            }

        }

        #region Respect Loading Animation

        Coroutine RespectLoadingCoroutine;

        void StartRespectLoadingAnimation()
        {
            respectLoadingBar.gameObject.SetActive(true);
            _respectValueLabel.gameObject.SetActive(false);
            RespectLoadingCoroutine = StartCoroutine(AddLoadingAnimationToRespectText());
        }

        void StopRespectLoadingAnimation()
        {
            StopCoroutine(RespectLoadingCoroutine);
            respectLoadingBar.gameObject.SetActive(false);
            _respectValueLabel.gameObject.SetActive(true);
        }

        IEnumerator AddLoadingAnimationToRespectText()
        {
            while (true)
            {
                if (respectLoadingBar != null)
                {
                    respectLoadingBar.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
                }
                yield return null;
            }
        }

        #endregion

        private void UpdateValues()
        {
            // _placedBlocksLabel.text = string.Format(_placedBlocksLabel.text, StatisticsManager.instance.PlacedBlocks.ToString("N0"));
            // _placedBlocks.text = (StatisticsManager.instance.PlacedBlocks * GameManager.instance.parameters["place_bonus"]).ToString("N0");
            //
            // _clearedBlocksBonusLabel.text = string.Format(_clearedBlocksBonusLabel.text, StatisticsManager.instance.ClearedBlocks.Count.ToString("N0"));
            // _clearedBlocksBonus.text = (StatisticsManager.instance.ClearedBlocks.Count * GameManager.instance.parameters["points_per_block"]).ToString("N0");
            //
            // _clearedLinesLabel.text = string.Format(_clearedLinesLabel.text, StatisticsManager.instance.ClearedLines.ToString("N0"));
            // _clearedLines.text = (StatisticsManager.instance.ClearedLines * GameManager.instance.parameters["clear_line_bonus"]).ToString("N0");
            //
            // _combosLabel.text = string.Format(_combosLabel.text, StatisticsManager.instance.Combos.ToString("N0"));
            // _combos.text = (StatisticsManager.instance.Combos * GameManager.instance.parameters["combo_bonus"]).ToString("N0");
            //
            // _streaksLabel.text = string.Format(_streaksLabel.text, StatisticsManager.instance.Streaks.ToString("N0"));
            // _streaks.text = (StatisticsManager.instance.Streaks * GameManager.instance.parameters["streak_bonus"]).ToString("N0");
            //
            // _currentScore.text = ScoreManager.instance.score.ToString("N0");
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnSubmit()
        {
            //not sure if loading animation manager will work.
            LoadingAnimationManager.instance.StartLoadingAnimation();

            //Go back to main menu
            ElympicsAuthenticationHandler.ReturningBack = true;
            ElympicsAuthenticationHandler.InMatch = false;

            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}