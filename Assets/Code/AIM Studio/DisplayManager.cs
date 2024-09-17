using System;
using ChocDino.UIFX;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class DisplayManager : MonoBehaviour
    {
        [SerializeField] private Color textRedColor;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private GameObject gameOverScreen;

        private TMP_Text _timerText;
        private bool _isStartRedTime;


        public void FindTimerText()
        {
            _timerText = GameObject.FindWithTag("TimerText").GetComponent<TMP_Text>();
        }

        public void DisplayTimer(float timerValue)
        {
            var minute = (int)(timerValue / 60);
            var second = (int)(timerValue - minute * 60);

            if (_timerText != null)
            {
                var secondText = second.ToString();
                if (second < 10)
                {
                    secondText = "0" + second;
                }

                _timerText.text = "0" + minute + ":" + secondText;
                if (timerValue < 10 && !_isStartRedTime)
                {
                    _isStartRedTime = true;
                    var dropShadowFilter = _timerText.GetComponent<DropShadowFilter>();

                    _timerText.color = textRedColor;
                    dropShadowFilter.enabled = true;
                    DOTween.To(() => dropShadowFilter.Blur, x => dropShadowFilter.Blur = x, 30, 0.5f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                }
            }
        }
    }
}