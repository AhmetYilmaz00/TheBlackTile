using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BBPath
{
    public List<Vector3> Points = new List<Vector3>();
    public List<float> Distances = new List<float>();

    public float Length { get; private set; }


    public BBPath(List<Vector3> points)
    {
        Points = points;

        Length = 0;
        Distances.Add(0f);
        for (int i = 1; i < Points.Count; i++)
        {
            Distances.Add(GetDistanceBetween(Points[i - 1], Points[i]));
            Length += Distances.Last();
        }
    }

    public void AddPoint(Vector3 point)
    {
        if(Points.Count > 0)
        {
            Distances.Add(GetDistanceBetween(Points.Last(), point));
            Length += Distances.Last();
        }
        Points.Add(point);
    }


    public Vector3 GetPointAtDistance(float distance)
    {
        float sumDistance = 0;
        for (int i = 1; i < Points.Count; i++)
        {
            if(distance <= sumDistance + Distances[i])
            {
                return GetPointBetween(Points[i - 1], Points[i], Mathf.Lerp(0, 1, (distance - sumDistance) / Distances[i]));
            }

            sumDistance += Distances[i];
        }

        return Points.Last();
    }

    public float GetDistanceBetween(Vector3 point1, Vector3 point2)
    {
        return Vector3.Distance(point1, point2);
    }

    public Vector3 GetPointBetween(Vector3 point1, Vector3 point2, float lerp)
    {
        return Vector3.Lerp(point1, point2, lerp);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        for (int i = 1; i < Points.Count; i++)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawLine(Points[i - 1], Points[i]);
        }
    }
#endif
}