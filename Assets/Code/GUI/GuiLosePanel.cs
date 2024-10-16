﻿using System.Collections;
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


        private GameManagerAim _gameManagerAim;
        private DisplayManager _displayManager;

        private void OnEnable()
        {
            _gameManagerAim = FindObjectOfType<GameManagerAim>();
            _displayManager = FindObjectOfType<DisplayManager>();
            TryAgainButton.onClick.AddListener(OnSubmit);
            ScoreText.text = "SCORE: " + _gameManagerAim.score.Value;
            TotalMoveText.text = "TOTAL MOVE: " + _gameManagerAim.totalMoveCount;
            Debug.Log("GUILOSEPANEL");
            _displayManager.GetRespect();


            //  HighScoreText.text = GameManager.instance.Progress.highScore.ToString();
        }

        private IEnumerator WaitGetRespect()
        {
            yield return new WaitUntil(() => respect_TMP.text != "");
        }

        private void Restart()
        {
            _displayManager.ReturnToLobbyButtonOnClick();
            //GameManager.instance.StartLevel();
        }

        public void OnSubmit()
        {
            SceneManager.LoadScene(0);

            // //not sure if loading animation manager will work.
            // LoadingAnimationManager.instance.StartLoadingAnimation();
            //
            // //Go back to main menu
            // ElympicsAuthenticationHandler.ReturningBack = true;
            // ElympicsAuthenticationHandler.InMatch = false;
            // ClearAllDontDestroyOnLoadObjects();
            // SceneManager.LoadScene(0);
        }

        public void ClearAllDontDestroyOnLoadObjects()
        {
            // DontDestroyOnLoad sahnesine ulaş
            Scene dontDestroyOnLoadScene = GetDontDestroyOnLoadScene();

            if (dontDestroyOnLoadScene.IsValid())
            {
                // Sahnedeki tüm root GameObject'leri bul ve yok et
                GameObject[] rootObjects = dontDestroyOnLoadScene.GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    Destroy(obj);
                }
            }
        }

        private Scene GetDontDestroyOnLoadScene()
        {
            // Tüm sahneleri dolaş ve DontDestroyOnLoad sahnesini bul
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == "DontDestroyOnLoad")
                {
                    return scene;
                }
            }

            return default;
        }
    }
}