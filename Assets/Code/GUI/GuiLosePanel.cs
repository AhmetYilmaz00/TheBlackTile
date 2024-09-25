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
        public TMP_Text HighScoreText;
        public TextMeshProUGUI respect_TMP;

        private GameManagerAim _gameManagerAim;
        private DisplayManager _displayManager;

        private void Awake()
        {
            _gameManagerAim = FindObjectOfType<GameManagerAim>();
            _displayManager = FindObjectOfType<DisplayManager>();
            TryAgainButton.onClick.AddListener(OnSubmit);
            ScoreText.text = _gameManagerAim.score.Value.ToString();
            if (ElympicsAuthenticationHandler.instance.IsGuest())
            {
                respect_TMP.text = "Please connect wallet to earn respect.";
            }
            else
            {
                _displayManager.DisplayRespect(respect_TMP);
            }
            //  HighScoreText.text = GameManager.instance.Progress.highScore.ToString();
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