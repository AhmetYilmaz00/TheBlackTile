using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIMStudio.Scripts
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button connectWalletButton;

        private TMP_Text playButtonText;
        private const string PLAY_BUTTON_PLAY_NOW_TEXT = "Play now";
        private const string PLAY_BUTTON_TRAIN_NOW_TEXT = "Train now";
        private const string WALLET_NOT_AVAILABLE_TEXT = "Wallet not available";

        private bool _availableToStart = false;

        #region Monobehaviours

        private void Awake()
        {
            Application.targetFrameRate = 60;

            playButtonText = playButton.GetComponentInChildren<TMP_Text>();

            playButton.onClick.AddListener(PlayButtonClick);
            connectWalletButton.onClick.AddListener(ConnectToWalletButtonClick);
        }

        private async void Start()
        {
            UpdateUI();

            LoadingAnimationManager.instance.StopLoadingAnimation();

            ElympicsAuthenticationHandler.instance.OnStatusChanged += Elympics_OnStatusChanged;
            ElympicsAuthenticationHandler.instance.OnStatusCheckEverySecond += Elympics_OnStatusChanged;

            await ElympicsAuthenticationHandler.instance.Initialize();
            if (ElympicsAuthenticationHandler.instance.IsInitialized())
            {
                _availableToStart = true;
            }
        }

        void OnDestroy()
        {
            if (ElympicsAuthenticationHandler.instance)
            {
                ElympicsAuthenticationHandler.instance.OnStatusChanged -= Elympics_OnStatusChanged;
                ElympicsAuthenticationHandler.instance.OnStatusCheckEverySecond -= Elympics_OnStatusChanged;
            }
        }

        #endregion

        #region Button Click Callbacks

        public void PlayButtonClick()
        {
            if (!_availableToStart || ElympicsAuthenticationHandler.instance.IsStartingMatch())
            {
                return;
            }
            ElympicsAuthenticationHandler.instance.StartQuickMatch();
        }

        private void ConnectToWalletButtonClick()
        {
            ElympicsAuthenticationHandler.instance.ConnectWallet();
        }

        #endregion

        private void Elympics_OnStatusChanged()
        {
            _availableToStart = ElympicsAuthenticationHandler.instance.IsAuthenticated();
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (ElympicsAuthenticationHandler.instance.IsAuthenticated())
            {
                connectWalletButton.gameObject.SetActive(ElympicsAuthenticationHandler.instance.IsConnectWalletShowable());
                playButtonText.text = ElympicsAuthenticationHandler.instance.IsGuest() ? PLAY_BUTTON_TRAIN_NOW_TEXT : PLAY_BUTTON_PLAY_NOW_TEXT;
                playButton.interactable = true;
            }
            else
            {
                connectWalletButton.gameObject.SetActive(ElympicsAuthenticationHandler.instance.IsConnectWalletShowable());
                playButton.interactable = false;
                playButtonText.text = "Loading...";
            }

            if (ElympicsAuthenticationHandler.instance.IsStartingMatch())
            {
                playButtonText.text = "Starting...";
                playButton.interactable = false;
            }
        }
    }
}
