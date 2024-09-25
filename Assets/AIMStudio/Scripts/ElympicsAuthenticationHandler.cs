using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Elympics;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.Wallet;
using ElympicsLobbyPackage.Session;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIMStudio.Scripts
{
    public class ElympicsAuthenticationHandler : Singleton<ElympicsAuthenticationHandler>
    {
        [SerializeField] private Web3Wallet _web3Wallet;
        [SerializeField] private SessionManager _sessionManager;

        private ExternalAuthData _authData;
        private string _web3Address;

        private bool _startingMatch = false;
        private bool _initialized = false;
        private bool _initializationInProgress = false;
        private bool _connectingInProgress = false;
        private bool _walletConnectionInProgress = false;
        private bool _externalInitSent = false;
        public Action OnStatusChanged;

        public Action OnStatusCheckEverySecond;
        private float timeToCheck = 0;

        public static bool ReturningBack = false;
        public static bool InMatch = false;

        private void Start()
        {
            if (ElympicsLobbyClient.Instance != null)
            {
                ElympicsLobbyClient.Instance.AuthenticationSucceeded += OnAuthenticationSucceeded;
                ElympicsLobbyClient.Instance.AuthenticationFailed += AuthenticationFailed;

                ElympicsLobbyClient.Instance.WebSocketSession.Disconnected += WebSocketDisconnected;
                ElympicsLobbyClient.Instance.WebSocketSession.Connected += WebSocketConnected;
            }

            if (_web3Wallet != null)
            {
                _web3Wallet.WalletConnectionUpdated += WalletConnectionUpdated;
            }

            if (ElympicsExternalCommunicator.Instance)
            {
                ElympicsExternalCommunicator.Instance.WalletCommunicator.WalletConnected += OnWalletConnected;
                ElympicsExternalCommunicator.Instance.WalletCommunicator.WalletDisconnected += OnWalletDisconnected;
            }

            StartCoroutine(UIUpdaterProcess());
        }

        protected void OnDestroy()
        {
            if (ElympicsLobbyClient.Instance != null)
            {
                ElympicsLobbyClient.Instance.AuthenticationSucceeded -= OnAuthenticationSucceeded;
                ElympicsLobbyClient.Instance.AuthenticationFailed -= AuthenticationFailed;

                ElympicsLobbyClient.Instance.WebSocketSession.Disconnected -= WebSocketDisconnected;
                ElympicsLobbyClient.Instance.WebSocketSession.Connected -= WebSocketConnected;
            }

            if (_web3Wallet != null)
            {
                _web3Wallet.WalletConnectionUpdated -= WalletConnectionUpdated;
            }

            if (ElympicsExternalCommunicator.Instance)
            {
                ElympicsExternalCommunicator.Instance.WalletCommunicator.WalletConnected -= OnWalletConnected;
                ElympicsExternalCommunicator.Instance.WalletCommunicator.WalletDisconnected -= OnWalletDisconnected;
            }
        }

        IEnumerator UIUpdaterProcess()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (_initialized)
                {
                    OnStatusCheckEverySecond?.Invoke();
                }
            }
        }

        public async UniTask Initialize()
        {
            LoadingAnimationManager.instance.StartLoadingAnimation();
            if (_initialized)
            {
                Debug.Log("Already inited");
                if (ReturningBack)
                {
                    if (!_authData.Capabilities.IsTelegram())
                    {
                        bool refreshResult = await _sessionManager.TryReAuthenticateIfWalletChanged();
                        Debug.Log("refresh result : " + refreshResult);
                    }

                    ReturningBack = false;
                }

                LoadingAnimationManager.instance.StopLoadingAnimation();
                return;
            }

            _initializationInProgress = true;
            var config = ElympicsConfig.LoadCurrentElympicsGameConfig();
            if (config == null)
            {
                Debug.Log("config is null");
                return;
            }

            try
            {
                Debug.Log("Game Id : " + config.GameId);
                Debug.Log("Game Name : " + config.GameName);
                Debug.Log("Game Version : " + config.GameVersion);

                await _sessionManager.AuthenticateFromExternalAndConnect();
                Debug.Log(_sessionManager.CurrentSession);
                _authData = new ExternalAuthData(_sessionManager.CurrentSession.Value.AuthData,
                    _sessionManager.CurrentSession.Value.IsMobile, _sessionManager.CurrentSession.Value.Capabilities,
                    _sessionManager.CurrentSession.Value.Environment);
                SendExternalInit();
                Debug.Log("External Game Status Application Initialized");
                Debug.Log("Initialization finishing");
                _initialized = true;
                OnStatusChanged?.Invoke();
                LoadingAnimationManager.instance.StopLoadingAnimation();
                Debug.Log("Capabilities : " + _authData.Capabilities);
            }
            catch (System.Exception ex)
            {
                MessageBox.instance?.Show(ex.Message);
                Debug.Log("Initialization error : " + ex.Message);
                Debug.Log(ex.StackTrace);
            }

            _initializationInProgress = false;
        }

        private void SendExternalInit()
        {
            if (_externalInitSent)
            {
                return;
            }

            ElympicsExternalCommunicator.Instance.GameStatusCommunicator.ApplicationInitialized();
            _externalInitSent = true;
        }

        public async void ConnectWallet()
        {
            if (_walletConnectionInProgress)
            {
                Debug.Log("Wallet connecting in progress...");
                return;
            }

            LoadingAnimationManager.instance.StartLoadingAnimation();
            _walletConnectionInProgress = true;
            try
            {
                Debug.Log("Connecting wallet");
                await _sessionManager.ConnectToWallet();
            }
            catch (System.Exception ex)
            {
                Debug.Log("Exception while connecting wallet : " + ex.Message);
            }

            LoadingAnimationManager.instance.StopLoadingAnimation();
            _walletConnectionInProgress = false;
            OnStatusChanged?.Invoke();
        }

        public async void StartQuickMatch()
        {
            _startingMatch = true;
            OnStatusChanged?.Invoke();
            LoadingAnimationManager.instance.StartLoadingAnimation();
            try
            {
                IRoom room = await ElympicsLobbyClient.Instance.RoomsManager.StartQuickMatch(GetPlayQueue());
                Debug.Log(room);
                _startingMatch = false;
                Debug.Log("_startingMatch: " + _startingMatch);

                OnStatusChanged?.Invoke();
                LoadingAnimationManager.instance?.StopLoadingAnimation();
                Debug.Log("StopLoadingAnimation");

                InMatch = true;
                Debug.Log("InMatch: " + InMatch);
            }
            catch (System.Exception ex)
            {
                MessageBox.instance.Show(ex.Message);
                Debug.LogError(ex.Message);

                _startingMatch = false;
                OnStatusChanged?.Invoke();
                LoadingAnimationManager.instance?.StopLoadingAnimation();
            }
        }

        #region Elympics Callbacks

        private void OnAuthenticationSucceeded(AuthData authData)
        {
            Debug.Log("Authentication succeeded.");
            OnStatusChanged?.Invoke();
        }

        private void AuthenticationFailed(string error)
        {
            Debug.Log("Authentication failed with error : " + error);
            OnStatusChanged?.Invoke();
        }

        private void WalletConnectionUpdated(WalletConnectionStatus connectionStatus)
        {
            if (connectionStatus == WalletConnectionStatus.Connected)
            {
                Debug.Log("Wallet connected");
                if (InMatch)
                {
                    ReusableMethodForIsInMatch();
                    return;
                }

                TryToReauthenticate();
            }
            else if (connectionStatus == WalletConnectionStatus.Disconnected)
            {
                Debug.Log("Wallet disconnected.");
                if (InMatch)
                {
                    ReusableMethodForIsInMatch();
                    return;
                }

                TryToReauthenticate();
            }

            OnStatusChanged?.Invoke();
        }

        private void OnWalletConnected(string address, string chainId)
        {
            Debug.LogError("Wallet connected callback");
            if (InMatch)
            {
                ReusableMethodForIsInMatch();
                return;
            }

            _sessionManager.TryReAuthenticateIfWalletChanged();
            OnStatusChanged?.Invoke();
        }

        private void OnWalletDisconnected()
        {
            Debug.LogError("Wallet disconnected callback");
            if (InMatch)
            {
                ReusableMethodForIsInMatch();
                return;
            }

            _sessionManager.TryReAuthenticateIfWalletChanged();
            OnStatusChanged?.Invoke();
        }

        private void ReusableMethodForIsInMatch()
        {
            //Add message box for players here
            GameManagerElympics.instance?.Disconnect();
            GameManagerElympics.instance?.GameOver(GameManagerElympics.GameOverReason.Disconnect);

            ElympicsAuthenticationHandler.ReturningBack = true;
            ElympicsAuthenticationHandler.InMatch = false;

            MessageBox.instance?.Register(() => SceneManager.LoadScene(0));
            MessageBox.instance.Show("Wallet changed in-game. Please go back to main menu and refresh.");
        }

        public void WebSocketDisconnected(DisconnectionData disconnectionData)
        {
            if (disconnectionData.Reason == DisconnectionReason.ApplicationShutdown ||
                disconnectionData.Reason == DisconnectionReason.ClientRequest)
                return;

            if (InMatch)
            {
                GameManagerElympics.instance?.GameOver(GameManagerElympics.GameOverReason.Disconnect);
            }

            DisconnectMessageBox();
            OnStatusChanged?.Invoke();
        }

        public void WebSocketConnected()
        {
            MessageBox.instance?.HideDisconnectPannel();
            OnStatusChanged?.Invoke();
        }

        #endregion

        #region Util Methods

        public void DisconnectMessageBox()
        {
            MessageBox.instance?.ShowDisconnectPannel();
        }

        public bool IsWalletConnectionInProgress()
        {
            return _walletConnectionInProgress;
        }

        public bool IsAuthenticated()
        {
            return IsWebSocketConnected() && ElympicsLobbyClient.Instance.IsAuthenticated;
        }

        public bool IsInitialized()
        {
            return IsWebSocketConnected() && _initialized;
        }

        public bool IsGuest()
        {
            return (ElympicsLobbyClient.Instance.AuthData.AuthType == AuthType.ClientSecret ||
                    ElympicsLobbyClient.Instance.AuthData.AuthType == AuthType.None);
        }

        public bool IsEthAddressShowable()
        {
            return IsAuthenticated() && !IsGuest();
        }

        public bool IsConnectWalletShowable()
        {
            return IsAuthenticated() && IsGuest() && (_authData.Capabilities.IsTon() || _authData.Capabilities.IsEth());
        }

        public bool IsWebSocketConnected()
        {
            return ElympicsLobbyClient.Instance.WebSocketSession.IsConnected;
        }

        public string GetLeaderboardQueue()
        {
            return IsTelegram() ? "telegram" : "eth";
        }

        public string GetPlayerName()
        {
            Debug.Log("Nickname : " + ElympicsLobbyClient.Instance.AuthData.Nickname);
            return ElympicsLobbyClient.Instance.AuthData.Nickname;
        }

        public string GetPlayerEthAddress()
        {
            return _sessionManager.CurrentSession.Value.AccountWallet;
        }

        public string GetPlayQueue()
        {
            //return "solo";
            AuthType authType = ElympicsLobbyClient.Instance.AuthData.AuthType;
            return authType == AuthType.Telegram ? "telegram" : authType == AuthType.EthAddress ? "eth" : "training";
        }

        public void TryToReauthenticate()
        {
            if (_sessionManager != null && _sessionManager.CurrentSession != null)
            {
                Debug.Log("trying to reauthenticate");
                _sessionManager.TryReAuthenticateIfWalletChanged();
            }
        }

        public bool IsTelegram()
        {
            return _authData.Capabilities.IsTelegram();
        }

        public void SignOut()
        {
            if (!IsAuthenticated())
            {
                Debug.Log("Not authenticated yet.");
                return;
            }

            ElympicsLobbyClient.Instance.SignOut();
            TryToReauthenticate();
            OnStatusChanged?.Invoke();
        }

        public bool IsStartingMatch()
        {
            return _startingMatch;
        }

        #endregion
    }
}