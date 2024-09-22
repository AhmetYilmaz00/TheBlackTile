using System;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage;
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

        private void Start()
        {
            GameObject elympicsExternalCommunicater = ElympicsExternalCommunicator.Instance.gameObject;
            sessionManager = elympicsExternalCommunicater.GetComponent<SessionManager>();
            web3Wallet = elympicsExternalCommunicater.GetComponent<Web3Wallet>();

            SetLobbyUIManager();

            ElympicsExternalCommunicator.Instance.GameStatusCommunicator.ApplicationInitialized();
            AttemptStartAuthenticate().Forget();
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