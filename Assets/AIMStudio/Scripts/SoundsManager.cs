using UnityEngine;

namespace AIMStudio.Scripts
{
    public class SoundsManager : MonoBehaviour
    {
        public AudioSource baseSource;

        public AudioClip[] gameSounds;

        public static SoundsManager instance;

        bool cannotExecuteSound;
        float timeToBePaused = 0.1f;

        public static bool audioEnabled = true;

        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (SoundsManager.audioEnabled == false)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }

        public void PlayGameOverSound()
        {
            Play(gameSounds[1]);
        }

        public void PlayButtonSound()
        {
            Play(gameSounds[0]);
        }

        public void PlayGameStartSound()
        {
            Play(gameSounds[2]);
        }

        public void PlayTetrimoSpawn()
        {
            Play(gameSounds[3]);
        }

        public void PlayLineCompleted()
        {
            Play(gameSounds[4]);
        }

        public void PlayTetrimoPlacement()
        {
            Play(gameSounds[5]);
        }

        public void PlayNewHighscore()
        {
            Play(gameSounds[6]);
        }

        public void PlayPopSound()
        {
            Play(gameSounds[7]);
        }

        public void PlayCountdown0()
        {
            Play(gameSounds[8]);
        }

        public void PlayCountdown1()
        {
            Play(gameSounds[9]);
        }

        public void PlayMinutePassedSound()
        {
            Play(gameSounds[10]);
        }

        public void Play(AudioClip clip)
        {
            if (clip != null)
            {
                baseSource.PlayOneShot(clip);
            }
        }
    }
}