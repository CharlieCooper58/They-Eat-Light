using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private static List<Timer> timers;
    private static TimerManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            timers = new List<Timer>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static Timer SetTimer(float time)
    {
        Timer newTimer = new Timer(time);
        if(timers != null)
        {
            timers.Add(newTimer);
        }
        return newTimer;
    }

    private void LateUpdate()
    {
        float t = Time.deltaTime;

        for(int i = timers.Count-1; i >= 0; --i)
        {
            timers[i].Tick(t);
            if (timers[i].markedForDisposal)
            {
                timers.RemoveAt(i);
            }
        }
    }
}

public class Timer
{
    float time;
    public bool markedForDisposal;
    public event Action OnTimerEnd;
    public Timer(float timeInSeconds)
    {
        this.time = timeInSeconds;
    }
    public void Tick(float tickTime)
    {
        time -= tickTime;
        if(time <= 0)
        {
            markedForDisposal = true;
            if(OnTimerEnd != null)
            {
                OnTimerEnd.Invoke();
            }
        }
    }

    public void Dispose()
    {
        markedForDisposal = true;
    }
}
