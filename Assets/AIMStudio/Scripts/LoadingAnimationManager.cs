using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AIMStudio.Scripts
{
    public class LoadingAnimationManager : Singleton<LoadingAnimationManager>
    {
        [SerializeField] private Transform blockerParent;
        [SerializeField] private Image loadingImage;
        [SerializeField] private bool isUpdateWork;

        private Coroutine loadingAnimation;

        public void StartLoadingAnimation()
        {
            SetLoadingImageAndBlockerActive(true);
            if (!isUpdateWork)
            {
                instance.loadingAnimation = StartCoroutine(LoadingAnimation());
            }
        }

        public void StopLoadingAnimation()
        {
            if (instance.loadingAnimation != null)
            {
                StopCoroutine(instance.loadingAnimation);
            }

            SetLoadingImageAndBlockerActive(false);
        }

        private void Update()
        {
            if (isUpdateWork)
            {
                loadingImage.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
            }
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
            if (blockerParent != null)
                blockerParent.gameObject.SetActive(active);
        }
    }
}