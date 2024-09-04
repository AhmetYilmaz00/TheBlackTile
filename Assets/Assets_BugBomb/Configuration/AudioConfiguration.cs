using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Configuration/Audio", order = 2)]
[ExecuteInEditMode]
public class AudioConfiguration : Configuration
{
    #region DONT LOOK HERE
    public AudioClip[] _clipsArray_0;
    public AudioClip[] _clipsArray_1;
    public AudioClip[] _clipsArray_2;
    public AudioClip[] _clipsArray_3;
    public AudioClip[] _clipsArray_4;
    public AudioClip[] _clipsArray_5;
    public AudioClip[] _clipsArray_6;
    public AudioClip[] _clipsArray_7;
    public AudioClip[] _clipsArray_8;
    public AudioClip[] _clipsArray_9;
    public AudioClip[] _clipsArray_10;
    public AudioClip[] _clipsArray_11;
    public AudioClip[] _clipsArray_12;
    public AudioClip[] _clipsArray_13;
    public AudioClip[] _clipsArray_14;
    public AudioClip[] _clipsArray_15;
    public AudioClip[] _clipsArray_16;
    public AudioClip[] _clipsArray_17;
    public AudioClip[] _clipsArray_18;
    public AudioClip[] _clipsArray_19;

    public AudioClip[] GetSoundArray(int soundIndex)
    {
        switch (soundIndex)
        {
            case 0:
                return _clipsArray_0;
            case 1:
                return _clipsArray_1;
            case 2:
                return _clipsArray_2;
            case 3:
                return _clipsArray_3;
            case 4:
                return _clipsArray_4;
            case 5:
                return _clipsArray_5;
            case 6:
                return _clipsArray_6;
            case 7:
                return _clipsArray_7;
            case 8:
                return _clipsArray_8;
            case 9:
                return _clipsArray_9;
            case 10:
                return _clipsArray_10;
            case 11:
                return _clipsArray_11;
            case 12:
                return _clipsArray_12;
            case 13:
                return _clipsArray_13;
            case 14:
                return _clipsArray_14;
            case 15:
                return _clipsArray_15;
            case 16:
                return _clipsArray_16;
            case 17:
                return _clipsArray_17;
            case 18:
                return _clipsArray_18;
            case 19:
                return _clipsArray_19;
        }

        return null;
    }
    #endregion

    private static AudioConfiguration _instance;
    public static AudioConfiguration instance
    {
        get {
            if (_instance == null)
            {
                _instance = Load<AudioConfiguration>();
            }
            return _instance;
        }
        set {
            _instance = value;
        }
    }
}
