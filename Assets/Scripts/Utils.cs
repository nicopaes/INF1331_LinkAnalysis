using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
    {
        var randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

        var randomDistance = UnityEngine.Random.Range(minRadius, maxRadius);

        var point = origin + randomDirection * randomDistance;

        return point;
    }

    public static Vector2 GetRandomPointInAnnulusInsideBox(Vector2 origin, float minRadious, float maxRadius, float minX, float maxX, float minY, float maxY)
    {
        int securityCount = 1000;
        Vector2 point = RandomPointInAnnulus(origin, minRadious, maxRadius);
        while (!(point.x < maxX && point.x > minX && point.y > minY && point.y < maxY)) // If NOT inside bound
        {
            point = RandomPointInAnnulus(origin, minRadious, maxRadius);
            securityCount--;
            if(securityCount <= 0 ) break;
        }
        return point;
    }
}
