using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialHandController : SingletonBehaviour<TutorialHandController>
{
    private const string Press_b = "Press";
    private const string Release_b = "Release";

    public GameObject Hand;
    public Animator Animator;

    public float PressDelay;
    public float ReleaseDelay;
    public float BetweenAnimsDelay;

    public float MoveTime;
    public float SequenceDelay;

    private bool _repeat;

    public void PressAnim(Vector3 position)
    {
        transform.position = position;
        _repeat = true;
        StartCoroutine(PressAnimCoroutine());
    }

    private IEnumerator PressAnimCoroutine()
    {
        Hand.SetActive(true);
        while (_repeat)
        {
            Animator.SetBool(Release_b, false);
            Animator.SetBool(Press_b, true);

            yield return new WaitForSeconds(0.5f);

            Animator.SetBool(Release_b, true);
            Animator.SetBool(Press_b, false);

            yield return new WaitForSeconds(0.5f);
        }

    }

    public void PressMoveAndReleaseAnim(Vector3 start, Vector3 end)
    {
        _repeat = true;
        StartCoroutine(PressMoveAndReleaseCoroutine(start, end));
    }

    private IEnumerator PressMoveAndReleaseCoroutine(Vector3 start, Vector3 end)
    {
        while(_repeat)
        {
            transform.position = start;
            Hand.SetActive(true);

            Animator.SetBool(Release_b, false);
            Animator.SetBool(Press_b, true);
            yield return new WaitForSeconds(PressDelay);

            transform.DOMove(end, 1f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
            Animator.SetBool(Press_b, false);
            Animator.SetBool(Release_b, true);

            yield return new WaitForSeconds(ReleaseDelay);
            Hand.SetActive(false);
            yield return new WaitForSeconds(BetweenAnimsDelay);
        }
    }

    public void PressMoveSequenceAndReleaseAnim(List<Vector3> positions)
    {
        _repeat = true;
        StartCoroutine(PressMoveSequenceAndReleaseCoroutine(positions));
    }

    private IEnumerator PressMoveSequenceAndReleaseCoroutine(List<Vector3> positions)
    {
        while (_repeat)
        {
            transform.position = positions[0];
            int index = 1;
            Hand.SetActive(true);

            Animator.SetBool(Release_b, false);
            Animator.SetBool(Press_b, true);
            yield return new WaitForSeconds(PressDelay);

            while(index < positions.Count)
            {
                transform.DOMove(positions[index], MoveTime).SetEase(Ease.Linear);
                yield return new WaitForSeconds(MoveTime);
                index++;
                yield return new WaitForSeconds(SequenceDelay);
            }

            Animator.SetBool(Press_b, false);
            Animator.SetBool(Release_b, true);

            yield return new WaitForSeconds(ReleaseDelay);
            Hand.SetActive(false);
            yield return new WaitForSeconds(BetweenAnimsDelay);
        }
    }

    public void StopAnimation()
    {
        DOTween.Kill(gameObject);
        _repeat = false;
        StopAllCoroutines();
        Hand.SetActive(false);
    }
}