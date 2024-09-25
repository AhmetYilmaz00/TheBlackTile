using System;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.Wallet;
using ElympicsLobbyPackage.Session;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class PersistentLobbyManager : MonoBehaviour
    {
        private SessionManager sessionManager;
        private Web3Wallet web3Wallet;
        private LobbyUIManager lobbyUIManager;
        private ExternalAuthData _authData;

        public enum AppState
        {
            Lobby,
            Matchmaking,
            Gameplay
        }

        private AppState _appState = AppState.Lobby;

        public void SetAppState(AppState newState)
        {
            _appState = newState;
            if (_appState == AppState.Lobby)
            {
                SetLobbyUIManager();
                AttemptReAuthenticate();
            }
        }

        private void SetLobbyUIManager()
        {
            lobbyUIManager = FindObjectOfType<LobbyUIManager>();
            lobbyUIManager.SetPersistantLobbyManager(this);
        }

        private async void Start()
        {
            GameObject elympicsExternalCommunicater = ElympicsExternalCommunicator.Instance.gameObject;
            sessionManager = elympicsExternalCommunicater.GetComponent<SessionManager>();
            web3Wallet = elympicsExternalCommunicater.GetComponent<Web3Wallet>();

            SetLobbyUIManager();


            await Initialize();
        }

        public async UniTask Initialize()
        {
            var config = ElympicsConfig.LoadCurrentElympicsGameConfig();
            if (config == null)
            {
                Debug.Log("config is null");
                return;
            }

            try
            {
                await sessionManager.AuthenticateFromExternalAndConnect();
                _authData = new ExternalAuthData(sessionManager.CurrentSession.Value.AuthData,
                    sessionManager.CurrentSession.Value.IsMobile, sessionManager.CurrentSession.Value.Capabilities,
                    sessionManager.CurrentSession.Value.Environment);
                ElympicsExternalCommunicator.Instance.GameStatusCommunicator.ApplicationInitialized();
            }
            catch (System.Exception ex)
            {
                Debug.Log("Initialization error : " + ex.Message);
                Debug.Log(ex.StackTrace);
            }
        }

        private async UniTask AttemptStartAuthenticate()
        {
            lobbyUIManager.SetAuthenticationScreenActive(true);
            if (!ElympicsLobbyClient.Instance.IsAuthenticated ||
                !ElympicsLobbyClient.Instance.WebSocketSession.IsConnected)
            {
                await sessionManager.AuthenticateFromExternalAndConnect();
            }

            lobbyUIManager.SetAuthenticationScreenActive(false);

            lobbyUIManager.SetLobbyVariantUI(sessionManager);
            web3Wallet.WalletConnectionUpdated += ReactToAuthenticationChange;
        }

        private async void AttemptReAuthenticate()
        {
            await sessionManager.TryReAuthenticateIfWalletChanged();
            lobbyUIManager.SetLobbyVariantUI(sessionManager);
            lobbyUIManager.SetAuthenticationScreenActive(false);
        }

        public void ReactToAuthenticationChange(WalletConnectionStatus status)
        {
            if (_appState == AppState.Lobby)
            {
                AttemptReAuthenticate();
            }
        }

        public async void ConnectToWallet()
        {
            await sessionManager.ConnectToWallet();
        }
    }
}