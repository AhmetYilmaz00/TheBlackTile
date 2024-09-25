using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIMStudio.Scripts
{
    public class MessageBox : Singleton<MessageBox>
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button okButton;

        private bool isDisconnectPannelShowing = false;
        private UnityEvent OnOkClick;

        private void Start()
        {
            okButton.onClick.AddListener(OnOKButtonClick);
            OnOkClick = new UnityEvent();
            ResetListeners();
        }

        public void ResetListeners()
        {
            OnOkClick.RemoveAllListeners();
            OnOkClick.AddListener(ClosePanel);
        }

        public void ShowDisconnectPannel()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
        Register(() => Application.ExternalEval("document.location.reload(true)"));
#endif
            isDisconnectPannelShowing = true;
            Show("Disconnected from game. Please refresh.");
        }

        public void HideDisconnectPannel()
        {
            if (isDisconnectPannelShowing)
            {
                OnOkClick.RemoveAllListeners();
                ClosePanel();
            }
        }

        public void Show(string text)
        {
            _messageText.text = text;
            Debug.Log(text);
            transform.GetChild(0).gameObject.SetActive(true);
        }

        void ClosePanel()
        {
            _panel.SetActive(false);
        }

        public void OnOKButtonClick()
        {
            OnOkClick?.Invoke();
        }

        public void Register(UnityAction action)
        {
            OnOkClick.RemoveAllListeners();
            OnOkClick.AddListener(ClosePanel);
            OnOkClick.AddListener(action);
        }
    }
}