using UnityEngine;

public class OnceInSeconds
{
    private float _seconds;
    private float _lastTimeAsked = 0;
    private bool _scaledTime;

    public OnceInSeconds(float seconds)
    {
        _seconds = seconds;
        _scaledTime = true;
    }

    public OnceInSeconds(float seconds, bool scaledTime)
    {
        _seconds = seconds;
        _scaledTime = scaledTime;
    }

    public bool Check()
    {
        if (_scaledTime)
        {
            if (_lastTimeAsked == 0 || Time.time - _lastTimeAsked >= _seconds)
            {
                _lastTimeAsked = Time.time;
                return true;
            }
            return false;
        }
        else
        {
            if (_lastTimeAsked == 0 || Time.unscaledTime - _lastTimeAsked >= _seconds)
            {
                _lastTimeAsked = Time.unscaledTime;
                return true;
            }
            return false;
        }
    }

    public void Reset()
    {
        Check();
    }
}