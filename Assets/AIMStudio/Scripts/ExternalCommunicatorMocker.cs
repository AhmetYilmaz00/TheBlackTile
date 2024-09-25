using ElympicsLobbyPackage;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using ElympicsLobbyPackage.ExternalCommunication;
using SCS;
using UnityEngine;

namespace AIMStudio.Scripts
{
    public class ExternalCommunicatorMocker : MonoBehaviour
    {
        [SerializeField] private ElympicsExternalCommunicator _externalCommunicator;
        [SerializeField] private StandaloneExternalAuthorizerConfig _authConfig;
        [SerializeField] private StandaloneBrowserJsConfig _browserJsConfig;
        [SerializeField] private SmartContractService _smartContractService;

        private void Awake()
        {
#if !UNITY_EDITOR
        Destroy(gameObject);
        Debug.LogError("External Communication mocker Destroyed");
#else
            Mock();
#endif
        }

        void Mock()
        {
#if UNITY_EDITOR

            PlayerPrefs.SetString("Elympics/EthPrivateKey",
                "a22854ac3a3c59b2a33a4c8f6384239be151e145f89fcf6e0bfd67f70a3351c7"); //0x9F65fa43Ab6120f6DA0F4D07C11FE180C0F4191b
            if (_externalCommunicator == null)
            {
                Debug.Log("There is no external communicator");
                return;
            }

            GameStatusCommunicator bermudaExternalGameStatusCommunicator = new GameStatusCommunicator();
            StandaloneExternalAuthenticator bermudaExternalAuthorizer =
                new StandaloneExternalAuthenticator(_authConfig);
            StandaloneExternalWalletCommunicator bermudaWalletCommunicator =
                new StandaloneExternalWalletCommunicator(_browserJsConfig, _smartContractService);
            _externalCommunicator.SetCustomExternalAuthenticator(bermudaExternalAuthorizer);
            _externalCommunicator.SetCustomExternalGameStatusCommunicator(bermudaExternalGameStatusCommunicator);
            _externalCommunicator.SetCustomExternalWalletCommunicator(bermudaWalletCommunicator);
            Debug.Log("External Configs set.");

#endif
        }
    }

    public class GameStatusCommunicator : IExternalGameStatusCommunicator
    {
        public void ApplicationInitialized()
        {
            Debug.Log("Bermuda External Game Status : inited");
        }

        public void GameFinished(int score)
        {
            Debug.Log("Bermuda External Game Status Game Finished : " + score);
        }
    }
}