using System;
using UnityEngine;
using UnityEngine.Events;

namespace AIMStudio.Scripts
{
    public class ScoreManager : MonoBehaviour
    {
        /// <summary>
        /// Check if this variable is used when all scripts are implemented
        /// </summary>
        private const string LIFETIME_SCORE_PLAYER_PREFS_KEY = "LIFETIMESCORE";

        private const string HIGHSCORE_PLAYER_PREFS_KEY = "HIGHSCORE";
        private const string TODAY_HIGHSCORE_PLAYER_PREFS_KEY = "TODAYHIGHSCORE";
        private const string TODAY_PLAYER_PREFS_KEY = "TODAY";

        public event Action Increased;
        public event Action Reset;

        public int score;
        public int highScore;
        public int todayHighScore;
        public string today => DateTime.Today.ToShortDateString();
        public int specialPoints;
        public int numberOfGames;

        public bool highscoreScoredInThisSession;
        public bool todayHighscoreScoredInThisSession;

        public delegate void ScoredHighscore();

        public UnityIntEvent OnScoredHighscore;

        public static ScoreManager instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            highScore = LoadHighScoreFromPrefs();

            if (today == LoadTodayFromPrefs())
            {
                todayHighScore = LoadTodayHighScoreFromPrefs();
            }
            else
            {
                todayHighScore = 0;
                SaveTodayHighScoreToPrefs();
                SaveTodayToPrefs();
            }
        }

        public void IncreaseScore(int valueToAdd)
        {
            Debug.Log("Add Points: " + valueToAdd);

            score += valueToAdd;

#if !UNITY_EDITOR
        //OJO / NOTE: ADD CHECK FROM ELYMPICS SDK HERE

        //if(SkillzCrossPlatform.IsMatchInProgress())
        //    SkillzCrossPlatform.UpdatePlayersCurrentScore(score);
#endif

            if (score > highScore)
            {
                if (!highscoreScoredInThisSession)
                    OnScoredHighscore?.Invoke(score);

                highscoreScoredInThisSession = true;
                highScore = score;
                SaveHighScoreToPrefs();
            }

            if (score > todayHighScore)
            {
                todayHighscoreScoredInThisSession = true;
                todayHighScore = score;
                SaveTodayHighScoreToPrefs();
            }

            Increased?.Invoke();
        }

        private void SaveHighScoreToPrefs()
        {
            PlayerPrefs.SetInt(HIGHSCORE_PLAYER_PREFS_KEY, highScore);
        }

        private void SaveTodayHighScoreToPrefs()
        {
            PlayerPrefs.SetInt(TODAY_HIGHSCORE_PLAYER_PREFS_KEY, highScore);
        }

        private void SaveTodayToPrefs()
        {
            PlayerPrefs.SetString(TODAY_PLAYER_PREFS_KEY, today);
        }

        private int LoadHighScoreFromPrefs()
        {
            return PlayerPrefs.GetInt(HIGHSCORE_PLAYER_PREFS_KEY, 0);
        }

        private int LoadTodayHighScoreFromPrefs()
        {
            return PlayerPrefs.GetInt(TODAY_HIGHSCORE_PLAYER_PREFS_KEY, 0);
        }

        private string LoadTodayFromPrefs()
        {
            return PlayerPrefs.GetString(HIGHSCORE_PLAYER_PREFS_KEY, string.Empty);
        }

        public void reset()
        {
            score = 0;
            highscoreScoredInThisSession = false;
            todayHighscoreScoredInThisSession = false;
            Reset?.Invoke();
        }

        [System.Serializable]
        public class UnityIntEvent : UnityEvent<int>
        {
        }
    }
}