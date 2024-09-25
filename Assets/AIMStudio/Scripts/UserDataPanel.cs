using ElympicsLobbyPackage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIMStudio.Scripts
{
    public class UserDataPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI nicknameText2;
        [SerializeField] private TextMeshProUGUI walletAdressText;
        [SerializeField] private Button button;

        [SerializeField] private float showY = -50f;
        [SerializeField] private float hideY = 50f;

        private RectTransform rectTransform;

        #region Monobehaviours and OnClick Callbacks

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            button.onClick.AddListener(onPlayerDataPanelClick);

            ElympicsAuthenticationHandler.instance.OnStatusChanged += ElympicsOnStatusChanged;
            ElympicsAuthenticationHandler.instance.OnStatusCheckEverySecond += ElympicsOnStatusChanged;
        }

        private void OnDestroy()
        {
            if (ElympicsAuthenticationHandler.instance)
            {
                ElympicsAuthenticationHandler.instance.OnStatusChanged -= ElympicsOnStatusChanged;
                ElympicsAuthenticationHandler.instance.OnStatusCheckEverySecond -= ElympicsOnStatusChanged;
            }
        }

        private void onPlayerDataPanelClick()
        {
            ElympicsExternalCommunicator.Instance.WalletCommunicator.ExternalShowAccountInfo();
        }

        #endregion

        private void ElympicsOnStatusChanged()
        {
            if (ElympicsAuthenticationHandler.instance.IsTelegram())
            {
                button.enabled = false;
            }

            if (ElympicsAuthenticationHandler.instance.IsGuest())
            {
                HideUserDataPanel();
                return;
            }

            if (ElympicsAuthenticationHandler.instance.IsAuthenticated())
            {
                var nickname = ElympicsAuthenticationHandler.instance.GetPlayerName();
                nicknameText.text = nickname;
                nicknameText2.text = nickname;
                if (ElympicsAuthenticationHandler.instance.IsEthAddressShowable())
                {
                    string ethAddress = ElympicsAuthenticationHandler.instance.GetPlayerEthAddress();
                    if (!string.IsNullOrEmpty(ethAddress))
                    {
                        ethAddress = ethAddress.Substring(0, 4) + ".." + ethAddress.Substring(ethAddress.Length - 2, 2);
                    }

                    walletAdressText.text = ethAddress;
                }
                else
                {
                    walletAdressText.enabled = false;
                }

                //not sure if this is needed, delete if not
                ShowUserDataPanel();
            }
        }

        public void HideUserDataPanel()
        {
            //ADD COROUTINE / ANIMATION HERE
            rectTransform.anchoredPosition = new Vector3(0, hideY, 0);
        }

        public void ShowUserDataPanel()
        {
            //ADD COROUTINE / ANIMATION HERE
            rectTransform.anchoredPosition = new Vector3(0, showY, 0);
        }
    }
}