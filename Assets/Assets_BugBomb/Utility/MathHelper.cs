using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class MathHelper
{
    public static float AngleBetween(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }

    public static float AngleBetween3D(Vector3 vec1, Vector3 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.z < vec1.z) ? -1.0f : 1.0f;
        return Vector3.Angle(Vector2.right, diference) * sign;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Vector3 DirectionTo(this Vector3 from, Vector3 target)
    {
        var heading = target - from;
        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction.

        return direction;
    }

    public static Vector3 PerpendicularVector2D(this Vector3 vector, bool left = true)
    {
        return left ? new Vector3(vector.y, -vector.x, 0) : new Vector3(-vector.y, vector.x, 0);
    }

    public static Vector3 RotateDirectionByAngle(this Vector3 direction, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * direction;
    }

    public static Quaternion LookAt2D(Vector3 from, Vector3 target)
    {
        Vector3 dir = from - target;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public static float AngleOnCircle(Vector2 circleCenter, Vector2 pointOnCircle)
    {
        return Mathf.Atan2(pointOnCircle.y - circleCenter.y, pointOnCircle.x - circleCenter.x);
    }

    public static float AngleOnCircle3D(Vector3 circleCenter, Vector3 point)
    {
        return MathHelper.AngleOnCircle(new Vector2(circleCenter.x, circleCenter.z), new Vector2(point.x, point.z));
    }

    public static Vector3 MoveInCircle(Vector3 circleCenter, float radius, float angleOnCircle, float angularVelocity, bool clockWise = false)
    {
        angleOnCircle += (clockWise ? -1 : 1) * angularVelocity * Time.deltaTime;
        var x = Mathf.Cos(angleOnCircle) * radius + circleCenter.x;
        var y = Mathf.Sin(angleOnCircle) * radius + circleCenter.y;

        return new Vector3(x, y, 0);
    }

    public static Vector3 MoveInCircleByAngle3D(Vector3 circleCenter, float startAngle, float angle)
    {
        startAngle += angle;
        var x = Mathf.Cos(startAngle) + circleCenter.x;
        var z = Mathf.Sin(startAngle) + circleCenter.z;

        return new Vector3(x, 0, z);
    }
    public static Vector3 PointOnCircle(Vector3 circleCenter, float angle)
    {
        var x = Mathf.Cos(angle) + circleCenter.x;
        var y = Mathf.Sin(angle) + circleCenter.y;

        return new Vector3(x, y, 0);
    }
    public static Vector3 PointOnCircle3D(Vector3 circleCenter, float angle)
    {
        var x = Mathf.Cos(angle) + circleCenter.x;
        var z = Mathf.Sin(angle) + circleCenter.z;

        return new Vector3(x, 0, z);
    }

    public static Vector3 RotateByAngle(Vector3 vector, float angle)
    {
        var angleBetween = AngleOnCircle3D(Vector3.zero, vector);

        return MoveInCircleByAngle3D(Vector3.zero, angleBetween, angle);
    }

    public static Transform GetClosest(Vector3 referencePoint, Transform[] objects, float maxDistance = Mathf.Infinity)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = referencePoint;

        foreach (Transform possibleTarget in objects)
        {
            float dist = Vector3.Distance(possibleTarget.position, currentPos);
            if (dist < minDist && dist < maxDistance)
            {
                tMin = possibleTarget;
                minDist = dist;
            }
        }
        return tMin;
    }

    public static Vector3 GetClosest(Vector3 referencePoint, Vector3[] objects)
    {
        Vector3 tMin = Vector3.negativeInfinity;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = referencePoint;

        for (int i = 0; i < objects.Length; i++)
        {
            float dist = Vector3.Distance(objects[i], currentPos);
            if (dist < minDist)
            {
                tMin = objects[i];
                minDist = dist;
            }
        }

        return tMin;
    }

    public static float NormalizeAngle(float eulerAngle)
    {
        eulerAngle = eulerAngle % 360;

        if (eulerAngle < 0)
        {
            eulerAngle += 360;
        }

        return eulerAngle;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        min += floor;
        max += floor;
        return Mathf.Clamp(angle, min, max);
    }

    public static Quaternion RoundRotation2D(Quaternion rotation, float anglePrecision)
    {
        var z = rotation.eulerAngles.z;
        var divider = z / anglePrecision;

        var dividerLower = Mathf.Floor(divider);
        var dividerHigher = Mathf.Ceil(divider);

        if(Mathf.Abs(dividerLower * anglePrecision - z) < Mathf.Abs(dividerHigher * anglePrecision - z))
        {
            return Quaternion.Euler(0,0, dividerLower *  anglePrecision);
        }
        else
        {
            return Quaternion.Euler(0, 0, dividerHigher * anglePrecision);
        }
    }
}
