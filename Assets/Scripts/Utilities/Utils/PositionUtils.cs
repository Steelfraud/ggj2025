using System.Collections.Generic;
using UnityEngine;

public static class PositionUtils
{

    internal static float BallInCameraZEffectScale = 0.5f;
    internal static float BallInCameraYEffectScale = 0f;

    public static Vector3 RandomPositionInArea(Vector3 area)
    {
        float randomX = Random.Range(0, area.x);
        float randomY = Random.Range(0, area.y);
        float randomZ = Random.Range(0, area.z);

        return new Vector3(randomX, randomY, randomZ);
    }

    public static Vector3 RandomPositionInArea(float radius)
    {
        float randomX = Random.Range(0, radius);
        float randomY = Random.Range(0, radius);
        float randomZ = Random.Range(0, radius);

        return new Vector3(randomX, randomY, randomZ);
    }

    public static Vector3 RandomPositionInAreaFromCenter(float radius)
    {
        float randomX = Random.Range(-radius, radius);
        float randomY = Random.Range(-radius, radius);
        float randomZ = Random.Range(-radius, radius);

        return new Vector3(randomX, randomY, randomZ);
    }

    public static Vector3 RandomPositionInSquareCenter(float radius)
    {
        return RandomPositionInSquareCenter(radius, 0);
    }

    public static Vector3 RandomPositionInSquareCenter(float radius, float yPos)
    {
        Vector3 randomPosition = RandomPositionInAreaFromCenter(radius);
        randomPosition.y = yPos;

        return randomPosition;
    }

    public static Vector3 RandomPositionInSquare(float radius)
    {
        return RandomPositionInSquare(radius, 0);
    }

    public static Vector3 RandomPositionInSquare(float radius, float yPos)
    {
        Vector3 randomPosition = RandomPositionInArea(radius);
        randomPosition.y = yPos;

        return randomPosition;
    }
    
    /* TODO:
     * Vlad optimize
     */
    public static Vector3 GetClosestPlanarFromVectors(List<Vector3> vectors, Vector3 to)
    {
        float currentSmallestDistance = float.MaxValue;
        float currentDistance = 0f;
        Vector3 currentVector = Vector3.zero;
        Vector3 vectorToReturn = Vector3.zero;

        for (int i = 0; i < vectors.Count; i++)
        {
            currentVector = vectors[i];

            currentVector.y = 0;
            currentDistance = Vector3.Distance(currentVector, to);

            if (currentDistance < currentSmallestDistance)
            {
                currentSmallestDistance = currentDistance;
                vectorToReturn = vectors[i];
            }
        }

        return vectorToReturn;
    }

    public static float PlanarDistance(this Vector3 first, Vector3 second)
    {
        Vector3 planarFirst = first;
        planarFirst.y = 0;

        Vector3 planarSecond = second;
        planarSecond.y = 0;

        return Vector3.Distance(planarFirst, planarSecond);
    }

    public static GameObject GetClosestObjectFromList(Vector3 closestTo, List<GameObject> objects)
    {
        float smallestDistance = float.MaxValue;
        float distanceToCurrent = float.MaxValue;
        GameObject currentClosestObject = null;

        foreach (GameObject obj in objects)
        {
            if (obj == null)
            {
                continue;
            }

            distanceToCurrent = Vector3.Distance(closestTo, obj.transform.position);

            if (distanceToCurrent < smallestDistance)
            {
                smallestDistance = distanceToCurrent;
                currentClosestObject = obj;
            }
        }

        return currentClosestObject;
    }

    //first-order intercept using absolute target position
    public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;

        float t = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);

        return targetPosition + t * targetRelativeVelocity;
    }

    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;

        if (velocitySquared < 0.001f)
        {
            return 0f;
        }

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));

            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        {
            //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a), t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);

            if (t1 > 0f)
            {
                if (t2 > 0f)
                {
                    return Mathf.Min(t1, t2); //both are positive
                }
                else
                {
                    return t1; //only t1 is positive
                }
            }
            else
            {
                return Mathf.Max(t2, 0f); //don't shoot back in time
            }
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
        {
            return 0f;
        }
        else //determinant = 0; one intercept path, pretty much never happens
        {
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
        }
    }

    /// <summary>
    /// Returns -1 if vector is to the left, 1 to the right and 0 for fwd/bwd 
    /// </summary>
    /// <param name="transformForward">forward vector of the object</param>
    /// <param name="targetDir">direction towards point you're checking</param>
    /// <returns></returns>
    public static int AngleDir(Vector3 transformForward, Vector3 targetDir, Vector3 up)
    {
        Vector3 cros = Vector3.Cross(transformForward, targetDir);
        float dir = Vector3.Dot(cros, up);

        if (dir > 0.0)
        {
            return 1;
        }
        else if (dir < 0.0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
    //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
    //same plane, use ClosestPointsOnTwoLines() instead.
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + lineVec1 * s;

            return true;
        }
        else
        {
            intersection = Vector3.zero;

            return false;
        }
    }

    public static float GetSinValue(float time, float sinSpeed = 1f, float sinMagnitude = 1f)
    {
        float sinValue = Mathf.Sin(time * sinSpeed) * sinMagnitude;

        return sinValue;
    }

    public static GameObject MakeMeDebugCube(this Vector3 cubePos, string name, bool disable = true)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = cubePos;
        go.transform.name = name;

        if (disable)
        {
            go.GetComponent<Collider>().enabled = false;
        }

        return go;
    }

    public static float GetSmallestDistanceToALineSquared(Vector3 origin, Vector3 direction, Vector3 point)
    {
        Vector3 v = ProjectPointOnLine(origin, direction, point);

        return (v - point).sqrMagnitude;
    }

    public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
    {
        lineVec = lineVec.normalized;
        //get vector from point on line to point in space
        Vector3 linePointToPoint = point - linePoint;

        float t = Vector3.Dot(linePointToPoint, lineVec);

        return linePoint + lineVec * t;
    }

}