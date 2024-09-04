using NaughtyAttributes;
using UnityEngine;

public class ExampleController : MonoBehaviour
{
    [Button]
    private void ShowExampleGUI()
    {
        GUIManager.instance.GetGUIScreen(GUIScreenType.Example);
    }

    [Button]
    private void RemoveExampleGUI()
    {
        GUIManager.instance.RemoveGUIScreen(GUIScreenType.Example);
    }
}
