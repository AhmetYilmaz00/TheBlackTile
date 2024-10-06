using System;
using System.Collections.Generic;
using Elympics;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Session;
using TMPro;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class LobbyUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNickname;
        [SerializeField] private TextMeshProUGUI playerEthAddress;

        [SerializeField] private List<(TextMeshProUGUI, TextMeshProUGUI)> leaderBoardContent =
            new List<(TextMeshProUGUI, TextMeshProUGUI)>();

        [SerializeField] private TextMeshProUGUI playButtonText;
        [SerializeField] private GameObject connectWalletButton;
        [SerializeField] private GameObject authenticationInProgressScreen;
        [SerializeField] private GameObject matchmakingInProgressScreen;
        [SerializeField] private GameObject playerAvatar;

        private PersistentLobbyManager _persistentLobbyManager = null;
        private LeaderboardClient leaderboardClient;

        private string playQueue = "eth";
        private string leaderboardQueue;

        public void SetPersistantLobbyManager(PersistentLobbyManager newValue) => _persistentLobbyManager = newValue;

        public void SetAuthenticationScreenActive(bool newValue) => authenticationInProgressScreen.SetActive(newValue);

        public void SetLobbyVariantUI(SessionManager sessionManager)
        {
            // Capabilities capabilities = sessionManager.CurrentSession.Value.Capabilities;
            // var currentAuthType = ElympicsLobbyClient.Instance.AuthData.AuthType;
            // Debug.Log("currentAuthType=" + currentAuthType);
            // bool isGuest = currentAuthType is AuthType.ClientSecret or AuthType.None;
            //
            // playButtonText.text = isGuest ? "Train now" : "Play now";
            // playerAvatar.SetActive(!isGuest);
            // playerNickname.gameObject.SetActive(!isGuest);
            // if (!isGuest)
            // {
            //     playerNickname.text = sessionManager.CurrentSession.Value.AuthData.Nickname;
            //     if (!capabilities.IsTelegram())
            //     {
            //         playerEthAddress.text = sessionManager.CurrentSession.Value.SignWallet;
            //     }
            // }
            //
            // playerEthAddress.gameObject.SetActive(!isGuest && !capabilities.IsTelegram());
            //
            // Debug.Log("capabilities.IsEth() :" + capabilities.IsEth());
            // Debug.Log("capabilities.IsTon() :" + capabilities.IsTon());
            // Debug.Log("isGuest :" + isGuest);
            //
            // connectWalletButton.SetActive(capabilities.IsEth() || capabilities.IsTon() && isGuest);
            // playQueue = currentAuthType switch
            // {
            //     AuthType.Telegram => "telegram",
            //     AuthType.EthAddress => "eth",
            //     _ => "training",
            // };
            // leaderboardQueue = currentAuthType == AuthType.Telegram ? "telegram" : "eth";
            //
            // CreateLeaderboardClient();
            // FetchLeaderboardEntries();
        }

        public void CreateLeaderboardClient()
        {
            var pageSize = 5;
            var gameVersion = LeaderboardGameVersion.All;
            var leaderboardType = LeaderboardType.BestResult;
            var customTimeScopeFrom = "2024-8-10T12:00:00+02:00";
            var customTimeScopeTo = "2024-9-10T12:00:00+02:00";
            var timeScopeObject = new LeaderboardTimeScope(DateTimeOffset.Parse(customTimeScopeFrom),
                DateTimeOffset.Parse(customTimeScopeTo));

            leaderboardClient =
                new LeaderboardClient(pageSize, timeScopeObject, leaderboardQueue, gameVersion, leaderboardType);
        }

        public void FetchLeaderboardEntries() => leaderboardClient.FetchFirstPage(DisplayTop5Entries);

        private void DisplayTop5Entries(LeaderboardFetchResult result)
        {
            foreach (var leaderboardRow in leaderBoardContent)
            {
                leaderboardRow.Item1.text = "";
                leaderboardRow.Item2.text = "";
            }

            for (int i = 0; i < 5 && i > result.Entries.Count; i++)
            {
                leaderBoardContent[i].Item1.text = result.Entries[i].Nickname;
                leaderBoardContent[i].Item2.text = result.Entries[i].Score.ToString();
            }
        }

        public void ConnectToWallet()
        {
            _persistentLobbyManager.ConnectToWallet();
            authenticationInProgressScreen.SetActive(true);
        }

        public async void PlayGame()
        {
            _persistentLobbyManager.SetAppState(PersistentLobbyManager.AppState.Matchmaking);
            matchmakingInProgressScreen.SetActive(true);
            try
            {
                IRoom room = await ElympicsLobbyClient.Instance.RoomsManager.StartQuickMatch(playQueue);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        public void ShowAccountInfo()
        {
            ElympicsExternalCommunicator.Instance.WalletCommunicator.ExternalShowAccountInfo();
        }
    }
}