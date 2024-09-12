using TMPro;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class DisplayManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

        public void DisplayTimer(float timerValue)
        {
            var minute = (int)(timerValue / 60);
            var second = (int)(timerValue - minute * 60);

            timerText.text = minute + ":" + second;
            if (timerValue < 170)
            {
                timerText.color = Color.red;
            }
        }
    }
}