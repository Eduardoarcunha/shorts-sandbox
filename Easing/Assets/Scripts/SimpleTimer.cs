using System;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    [SerializeField] private float duration = 3f;

    public float Duration
    {
        get => duration;
        set => duration = Mathf.Max(0.0001f, value);
    }

    public bool IsRunning { get; private set; }
    public float Elapsed { get; private set; }
    public float Remaining => Mathf.Max(0f, Duration - Elapsed);
    public float Normalized => Mathf.Clamp01(Duration <= 0 ? 1f : Elapsed / Duration);

    public event Action OnStarted;
    public event Action<float> OnTick;
    public event Action OnCompleted;

    public void StartTimer(float? overrideDuration = null)
    {
        if (overrideDuration.HasValue)
            Duration = overrideDuration.Value;

        Elapsed = 0f;
        IsRunning = true;
        OnStarted?.Invoke();
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        Elapsed = 0f;
    }

    private void Update()
    {
        if (!IsRunning) return;

        Elapsed += Time.deltaTime;
        OnTick?.Invoke(Normalized);

        if (Elapsed >= Duration)
        {
            Elapsed = Duration;
            IsRunning = false;
            OnTick?.Invoke(1f);
            OnCompleted?.Invoke();
        }
    }
}