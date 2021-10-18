using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float TimeWarpFactor = 2;

    bool paused = true;
    bool warping = false;

    private float baseTimeRate = 1;

    private void Start()
    {
        UpdateTimeScale();
    }

    public void TogglePause()
    {
        SetPause(!paused);
    }

    public void SetPause(bool newState)
    {
        paused = newState;
        warping = false;
        UpdateTimeScale();
    }

    public void ToggleWarp()
    {
        SetWarp(!warping);
    }

    public void SetWarp(bool newState)
    {
        warping = newState;

        if (warping)
        {
            baseTimeRate = TimeWarpFactor;
        }
        else
        {
            baseTimeRate = 1;
        }

        UpdateTimeScale();
    }

    private void UpdateTimeScale()
    {
        if (paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = baseTimeRate;
        }
    }
}
