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
            TryAgainButton.onClick.AddListener(Restart);
            ScoreText.text = _gameManagerAim.score.Value.ToString();
            _displayManager.DisplayRespect(respect_TMP);
            //  HighScoreText.text = GameManager.instance.Progress.highScore.ToString();
        }
        
        private void Restart()
        {
            _displayManager.ReturnToLobbyButtonOnClick();
            //GameManager.instance.StartLevel();
        }
    }
}