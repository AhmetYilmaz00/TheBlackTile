using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct BBTime
{
    private float _time;

    public static implicit operator float(BBTime bbTime)
    {
        return bbTime._time;
    }

    public void Set(float time)
    {
        _time = time;
    }

    public bool Passed()
    {
        return Time.time >= _time;
    }

    public float TimeFromNow()
    {
        if(_time <= 0)
            return float.NegativeInfinity;

        return Time.time - _time;
    }

    public bool IsSet()
    {
        return _time > 0;
    }

    public void Reset()
    {
        _time = -1;
    }
}
