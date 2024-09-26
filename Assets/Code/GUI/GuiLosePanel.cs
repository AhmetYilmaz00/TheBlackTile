using System.Collections;
using AIMStudio.Scripts;
using Code.AIM_Studio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GUI
{
    public class GuiLosePanel : MonoBehaviour
    {
        public Button TryAgainButton;
        public TMP_Text ScoreText;
        public TMP_Text TotalMoveText;
        public TextMeshProUGUI respect_TMP;
        public GameObject YesWallet;
        public GameObject NoWallet;
        public GameObject loadingIcon;

        private GameManagerAim _gameManagerAim;
        private DisplayManager _displayManager;

        private void OnEnable()
        {
            _gameManagerAim = FindObjectOfType<GameManagerAim>();
            _displayManager = FindObjectOfType<DisplayManager>();
            TryAgainButton.onClick.AddListener(OnSubmit);
            ScoreText.text = "SCORE: " + _gameManagerAim.score.Value;
            TotalMoveText.text = "TOTAL MOVE: " + _gameManagerAim.totalMoveCount;
            if (ElympicsAuthenticationHandler.instance.IsGuest())
            {
                YesWallet.SetActive(false);
                NoWallet.SetActive(true);
            }
            else
            {
                _displayManager.DisplayRespect(respect_TMP);
                StartCoroutine(WaitGetRespect());
            }
            //  HighScoreText.text = GameManager.instance.Progress.highScore.ToString();
        }

        private IEnumerator WaitGetRespect()
        {
            yield return new WaitUntil(() => respect_TMP.text != "");
            loadingIcon.SetActive(false);
        }

        private void Restart()
        {
            _displayManager.ReturnToLobbyButtonOnClick();
            //GameManager.instance.StartLevel();
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