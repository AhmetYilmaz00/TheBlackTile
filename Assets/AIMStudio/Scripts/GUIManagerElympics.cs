using AIMStudio.Scripts;
using TMPro;
using UnityEngine;

public class GUIManagerElympics : MonoBehaviour
{
    public static GUIManagerElympics instance;

    public GameOverGUI gameOverGUI;
    public GameObject frontPage;
    public TMP_Text countDownText;
    public GameObject countDownTextContainer;

    void Awake()
    {
        instance = this;
    }
}




