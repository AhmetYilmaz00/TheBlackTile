using Elympics;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace AIMStudio.Scripts
{
    public class LeaderboardEntryUI : MonoBehaviour
    {
        [Header("UI references")] [SerializeField]
        private TextMeshProUGUI positionText;

        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI scoreText;

        // [SerializeField] private GameObject goldCoin;
        // [SerializeField] private GameObject silverCoin;
        // [SerializeField] private GameObject bronzeCoin;

        [SerializeField] private Color highlightColor;
        [SerializeField] private Color defaultColor;

        public void SetValues(LeaderboardEntry entry)
        {
            gameObject.SetActive(true);

            if (entry == null)
                return;

            if (entry.Position == 4 || entry.Position == 5)
            {
                positionText.text = entry.Position.ToString();
            }

            // if (entry.Position == 1)
            // {
            //     SetGold();
            // }
            // else if (entry.Position == 2)
            // {
            //     SetSilver();
            // }
            // else if (entry.Position == 3)
            // {
            //     SetBronze();
            // }
            // else
            // {
            //     SetNone();
            // }

            nicknameText.text = entry.Nickname.IsNullOrEmpty() ? entry.UserId : entry.Nickname;
            scoreText.text = entry.Score.ToString("0");

            ChangeEntryToDefaultStyle();
        }

        private void ChangeEntryToDefaultStyle()
        {
            if (positionText != null)
            {
                ChangeTextToDefault(positionText);
            }

            ChangeTextToDefault(nicknameText);
            ChangeTextToDefault(scoreText);
        }

        public void HighlightEntry()
        {
            if (positionText != null)
            {
                ChangeTextToHighlight(positionText);
            }

            ChangeTextToHighlight(nicknameText);
            ChangeTextToHighlight(scoreText);
        }

        private void ChangeTextToHighlight(TextMeshProUGUI text)
        {
            text.color = highlightColor;
            text.fontStyle = FontStyles.Bold;
        }

        private void ChangeTextToDefault(TextMeshProUGUI text)
        {
            text.color = defaultColor;
            text.fontStyle = FontStyles.Normal;
        }

        // public void SetGold()
        // {
        //     goldCoin.SetActive(true);
        // }
        // public void SetSilver()
        // {
        //     silverCoin.SetActive(true);
        // }
        // public void SetBronze()
        // {
        //     bronzeCoin.SetActive(true);
        // }
        // public void SetNone()
        // {
        //     goldCoin.SetActive(true);
        //     silverCoin.SetActive(true);
        //     bronzeCoin.SetActive(true);
        // }
    }
}