using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AIMStudio.Scripts
{
    public class LoadingAnimationManager : Singleton<LoadingAnimationManager>
    {
        [SerializeField] private Transform blockerParent;
        [SerializeField] private Image loadingImage;

        private Coroutine loadingAnimation;

        public void StartLoadingAnimation()
        {
            SetLoadingImageAndBlockerActive(true);
            instance.loadingAnimation = StartCoroutine(LoadingAnimation());
        }

        public void StopLoadingAnimation()
        {
            if (instance.loadingAnimation != null)
            {
                StopCoroutine(instance.loadingAnimation);
            }
            SetLoadingImageAndBlockerActive(false);
        }

        IEnumerator LoadingAnimation()
        {
            while (true)
            {
                if (loadingImage != null)
                {
                    loadingImage.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
                }
                yield return null;
            }
        }

        void SetLoadingImageAndBlockerActive(bool active)
        {
            blockerParent.gameObject.SetActive(active);
        }
    }
}