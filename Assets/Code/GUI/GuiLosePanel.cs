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
        private GameManagerAim _gameManagerAim;

        private void Awake()
        {
            _gameManagerAim = FindObjectOfType<GameManagerAim>();
            TryAgainButton.onClick.AddListener(Restart);
            ScoreText.text = _gameManagerAim.score.Value.ToString();
            //  HighScoreText.text = GameManager.instance.Progress.highScore.ToString();
        }

        private void Restart()
        {
            SceneManager.LoadScene(0);
            //GameManager.instance.StartLevel();
        }
    }
}