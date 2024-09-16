using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.AIM_Studio
{
    public class AddPointTextController : MonoBehaviour
    {
        public bool isWorking;
        [SerializeField] private float pathDuration;
        [SerializeField] private float pathYPos;
        [SerializeField] private TMP_Text pointText;

        private Transform _scoreTransform;
        private Color _initColor;
        private Vector3 _initPosition;

        private void Start()
        {
            _initColor = new Color(0.282353f, 0.254902f, 0.254902f);
            _initPosition = transform.position;
        }

        public void AddPointEffectStart(string text, TMP_Text tmpText, int score)
        {
            isWorking = true;
            pointText.text = "+" + text;
            _scoreTransform = GameObject.FindWithTag("ScoreText").transform;
            transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBounce);
            StartCoroutine(AddPointEffect(tmpText, score));
        }

        private IEnumerator AddPointEffect(TMP_Text tmpText, int score)
        {
            yield return new WaitForSeconds(0.3f);
            var startPos = transform.position;

            var controlPoint = new Vector3((startPos.x + _scoreTransform.position.x) / 2, startPos.y + pathYPos,
                (startPos.z + _scoreTransform.position.z) / 2);

            var path = new[] { startPos, controlPoint, _scoreTransform.position };

            transform.DOPath(path, pathDuration, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetOptions(false)
                .SetLoops(1, LoopType.Restart);
            StartCoroutine(ScoreSizeUpEffectCoroutine(tmpText, score));
        }

        private IEnumerator ScoreSizeUpEffectCoroutine(TMP_Text tmpText, int score)
        {
            yield return new WaitForSeconds(pathDuration - pathDuration / 7);
            pointText.DOColor(Color.clear, 0.15f).SetEase(Ease.Linear);
            _scoreTransform.DOScale( 1.2f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                tmpText.text = score.ToString();
                _scoreTransform.DOScale(1, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    transform.localScale = Vector3.zero;
                    pointText.color = _initColor;
                    transform.position = _initPosition;
                    isWorking = false;
                });
            });
        }
    }
}