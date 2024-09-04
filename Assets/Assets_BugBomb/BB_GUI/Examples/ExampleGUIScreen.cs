using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleGUIScreen : MonoBehaviour
{
    public Button CloseButton;

    private GUIScreen GUIScreen;

    private void Awake()
    {
        GUIScreen = GetComponent<GUIScreen>();

        CloseButton.onClick.AddListener(Example_CloseGuiScreen);
    }


    private void Example_CloseGuiScreen()
    {
        GUIManager.instance.RemoveGUIScreen(GUIScreen.GUIScreenType);
    }
}
