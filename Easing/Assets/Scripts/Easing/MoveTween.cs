using System;
using UnityEngine;
using DG.Tweening;

public class MoveTween : MonoBehaviour
{
    [SerializeField] private Ease easeType = Ease.InOutSine;
    [SerializeField] private Vector3 localTargetOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float duration = 1f;
    [SerializeField] private float pauseBetweenLoops = 0.5f;

    Sequence seq;

    bool stopRequested = false;
    public event Action<MoveTween> LoopStopReached;

    void OnEnable()
    {
        BuildSequence();
    }

    void OnDisable()
    {
        seq?.Kill();
    }

    void BuildSequence()
    {
        seq?.Kill();
        transform.localPosition = Vector3.zero;

        seq = DOTween.Sequence()
            .Append(transform.DOLocalMove(localTargetOffset, duration).SetEase(easeType))
            .Append(transform.DOLocalMove(Vector3.zero, duration).SetEase(easeType))
            .AppendInterval(pauseBetweenLoops)
            .SetLoops(-1, LoopType.Restart)
            .SetAutoKill(false)
            .OnStepComplete(() => { if (stopRequested) { seq.Pause(); LoopStopReached?.Invoke(this); } });
    }

    // Called by the spawner right after instantiate
    public void Configure(Ease ease, Vector3 offset, float dur, float pause, Color color)
    {
        easeType = ease;
        localTargetOffset = offset;
        duration = dur;
        pauseBetweenLoops = pause;
        var spriteRend = GetComponent<SpriteRenderer>();
        if (spriteRend != null) spriteRend.color = color;
        BuildSequence();
    }

    public void RequestStop()
    {
        stopRequested = true;
    }

    public void ClearStopAndResume()
    {
        stopRequested = false;
        if (seq != null && !seq.IsPlaying()) seq.Play();
    }
}