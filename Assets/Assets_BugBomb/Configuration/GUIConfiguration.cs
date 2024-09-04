using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Configuration/GUI", order = 2)]
[ExecuteInEditMode]
public class GUIConfiguration : Configuration
{
    [Header("UI")]
    public List<GUIScreen> AllGUIScreens;

    [Header("Safe Area debug")]
    public bool ForceSafeAreaForIphone = false;
    public RectOffset SafeAreaPadding;

    [Header("Ads")]
    public bool AdsEnabled = false;

    [Header("Standards")]
    public float winPanelDelay = 1.5f;
    public float winPanelWithExplosivesDelay = 1.5f;

    public string privacyPolicyURL = @"http://bugbomb.games/home/privacy-policy/";

    [Header("GUI animations")]
    public float showScreenTime = 0.7f;
    public float hideScreenTime = 0.7f;
    [Space(10)]
    public float showPopupTime = 0.7f;
    public float hidePopusTime = 0.7f;



    internal GUIScreen GetGUIScreen(GUIScreenType guiScreenType)
    {
        for (int i = 0; i < AllGUIScreens.Count; i++)
        {
            if (AllGUIScreens[i].GUIScreenType == guiScreenType)
                return AllGUIScreens[i];
        }

        return null;
    }


    private static GUIConfiguration _instance;

    public static GUIConfiguration instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Load<GUIConfiguration>();
            }
            return _instance;
        }
        set 
        {
            _instance = value;
        }
    }
}
