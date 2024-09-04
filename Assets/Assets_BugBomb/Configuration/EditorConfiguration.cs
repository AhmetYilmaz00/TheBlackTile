using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Configuration/Editor", order = 2)]
[ExecuteInEditMode]
public class EditorConfiguration : Configuration
{
    // ...


    private static EditorConfiguration _instance;
    public static EditorConfiguration instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Load<EditorConfiguration>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}
