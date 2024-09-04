using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels", menuName = "Configuration/Levels", order = 2)]
[ExecuteInEditMode]
public class LevelsConfiguration : Configuration
{
    private static LevelsConfiguration _instance;
    public static LevelsConfiguration instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Load<LevelsConfiguration>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}
