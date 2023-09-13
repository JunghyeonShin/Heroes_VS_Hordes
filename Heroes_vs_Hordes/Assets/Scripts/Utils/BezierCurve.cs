using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    public static Vector3 LinearCurve(Vector3 start, Vector3 end, float time)
    {
        return Vector3.Lerp(start, end, time);
    }

    public static Vector3 QuadraticCurve(Vector3 start, Vector3 temp, Vector3 end, float time)
    {
        var p1 = LinearCurve(start, temp, time);
        var p2 = LinearCurve(temp, end, time);
        return LinearCurve(p1, p2, time);
    }
}
