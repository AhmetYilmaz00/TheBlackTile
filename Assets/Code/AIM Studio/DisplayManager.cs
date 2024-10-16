using System.Linq;
using AIMStudio.Scripts;
using ChocDino.UIFX;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Elympics;
using ElympicsLobbyPackage;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.AIM_Studio
{
    public class DisplayManager : ElympicsMonoBehaviour
    {
        [SerializeField] private Color textRedColor;
        [SerializeField] private TextMeshProUGUI _respectValueLabel;

        private TMP_Text _timerText;
        private bool _isStartRedTime;
        private long respectPoints;
        public GameObject YesWallet;
        public GameObject NoWallet;
        public GameObject loadingIcon;

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

        public void ReturnToLobbyButtonOnClick()
        {
            ReturnToLobby().Forget();
        }

        private async UniTask ReturnToLobby()
        {
            await SceneManager.LoadSceneAsync(0);
            ElympicsExternalCommunicator.Instance.gameObject.GetComponent<PersistentLobbyManager>()
                .SetAppState(PersistentLobbyManager.AppState.Lobby);
        }

        public async void DisplayRespect(TextMeshProUGUI respectText)
        {
            if (!ElympicsLobbyClient.Instance.IsAuthenticated)
            {
                respectText.text = "-1";
            }

            RespectService respectService = new RespectService(ElympicsLobbyClient.Instance, ElympicsConfig.Load());
            var matchID = ElympicsLobbyClient.Instance.RoomsManager.ListJoinedRooms()[0].State.MatchmakingData!
                .MatchData!.MatchId;
            var respectValue = await respectService.GetRespectForMatch(matchID);
            respectText.text = respectValue.Respect.ToString();
        }

        public async void GetRespect()
        {
            // we assume here that the game allows for being in only one match at time


            if (!ElympicsAuthenticationHandler.instance.IsGuest())
            {
                var respectService = new RespectService(ElympicsLobbyClient.Instance, ElympicsConfig.Load());
                var matchId =
                    ElympicsLobbyClient.Instance.RoomsManager.ListAvailableRooms().First().State.MatchmakingData!
                        .MatchData!.MatchId;

                var respectValue = await respectService.GetRespectForMatch(matchId);
                loadingIcon.SetActive(false);
                respectPoints = respectValue.Respect;
                _respectValueLabel.text = respectPoints.ToString();

                YesWallet.SetActive(true);
                NoWallet.SetActive(false);
            }
            else
            {
                YesWallet.SetActive(false);
                NoWallet.SetActive(true);
            }
        }
    }
}