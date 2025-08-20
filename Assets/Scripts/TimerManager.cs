using System;
using System.Collections.Generic;
using UnityEngine;

public readonly struct TimerHandle
{
    private readonly WeakReference<TimerEntry> _timer;

    public TimerHandle(TimerEntry timer)
    {
        _timer = new WeakReference<TimerEntry>(timer);
    }

    public void Pause()
    {
        if (_timer.TryGetTarget(out var timer))
        {
            timer.paused = true;
        }
    }

    public void Unpause()
    {
        if (_timer.TryGetTarget(out var timer))
        {
            timer.paused = false;
        }
    }

    public void Reset()
    {
        if (_timer.TryGetTarget(out var timer))
        {
            timer.currentTime = 0.0f;
        }
    }
}

public class TimerEntry
{
    public readonly WeakReference<MonoBehaviour> owner;
    public readonly float duration;
    public float currentTime;
    public readonly Action callback;
    public bool paused;

    public TimerEntry(WeakReference<MonoBehaviour> owner, float duration, Action callback)
    {
        this.owner = owner;
        this.duration = duration;
        this.callback = callback;

        currentTime = 0.0f;
        paused = false;
    }
}

public class TimerManager : MonoBehaviour
{
    private static TimerManager _instance;

    public static TimerManager instance
    {
        get
        {
            if (_instance)
            {
                return _instance;
            }
            
            var container = new GameObject("TimerManager");
            _instance = container.AddComponent<TimerManager>();
            return _instance;
        }
    }
    
    private readonly List<TimerEntry> _timers = new();

    public void Update()
    {
        for(var i = _timers.Count - 1; i >= 0; --i)
        {
            var timer = _timers[i];
            timer.currentTime += Time.deltaTime;

            if (!(timer.currentTime >= timer.duration))
            {
                continue;
            }
            
            if (timer.owner.TryGetTarget(out var owner))
            {
                timer.callback.Invoke();
            }
            _timers.RemoveAt(i);
        }
    }

    public void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public TimerHandle CreateTimer(MonoBehaviour owner, float duration, Action callback)
    {
        var newTimer = new TimerEntry(new WeakReference<MonoBehaviour>(owner), duration, callback);
        _timers.Add(newTimer);
        return new TimerHandle(newTimer);
    }
}
