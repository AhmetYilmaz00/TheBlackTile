using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class AddPointTextController : MonoBehaviour
    {
        [SerializeField] private Transform scoreTransform;
        [SerializeField] private float pathDuration;
        [SerializeField] private float pathYPos;
        [SerializeField] private TMP_Text pointText;

        private void Start()
        {
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
            StartCoroutine(AddPointEffect());
        }

        public IEnumerator AddPointEffect()
        {
            yield return new WaitForSeconds(0.3f);
            var startPos = transform.position;

            var controlPoint = new Vector3((startPos.x + scoreTransform.position.x) / 2, startPos.y + pathYPos,
                (startPos.z + scoreTransform.position.z) / 2);

            var path = new[] { startPos, controlPoint, scoreTransform.position };

            transform.DOPath(path, pathDuration, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetOptions(false)
                .SetLoops(1, LoopType.Restart);
            StartCoroutine(ScoreSizeUpEffectCoroutine());
        }

        private IEnumerator ScoreSizeUpEffectCoroutine()
        {
            yield return new WaitForSeconds(pathDuration - pathDuration / 7);
            pointText.DOColor(Color.clear, 0.15f).SetEase(Ease.Linear);
            var firstScale = scoreTransform.localScale;
            scoreTransform.DOScale(firstScale * 1.2f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                scoreTransform.DOScale(firstScale, 0.1f).SetEase(Ease.Linear);
            });
        }
    }
}