using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage;
using UnityEngine;

[DefaultExecutionOrder(-9980)] //before SessionManager -9975 and ExternalCommunicator -10000
public class FixExternalAuthorizer : MonoBehaviour
{
    [SerializeField] private StandaloneExternalAuthorizerConfig authorizerConfig;

    private void Awake()
    {
#if !UNITY_EDITOR || UNITY_WEBGL
			return;
#else

        ElympicsExternalCommunicator.Instance.SetCustomExternalAuthenticator(new StandaloneExternalAuthenticator(authorizerConfig));
#endif
    }
}
