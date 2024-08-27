using UnityEngine;

public static class GeoMaths
{
    public const float EarthRadiusKM = 6371;
    public const float EarthCircumferenceKM = EarthRadiusKM * Mathf.PI * 2;

    public static Coordinate PointToCoordinate(Vector3 pointOnUnitSphere)
    {
        float latitude = Mathf.Asin(pointOnUnitSphere.y);
        float a = pointOnUnitSphere.x;
        float b = -pointOnUnitSphere.z;

        float longitude = Mathf.Atan2(a, b);
        return new Coordinate(longitude, latitude);
    }

    public static Vector3 CoordinateToPoint(Coordinate coordinate, float radius = 1)
    {
        float y = Mathf.Sin(coordinate.latitude);
        float r = Mathf.Cos(coordinate.latitude);
        float x = Mathf.Sin(coordinate.longitude) * r;
        float z = -Mathf.Cos(coordinate.longitude) * r;

        return new Vector3(x, y, z) * radius;
    }

    public static float DistanceBetweenPointsOnUnitSphere(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(Vector3.Cross(a, b).magnitude, Vector3.Dot(a, b));
    }

    public static Vector2 PointToUV(Vector3 point)
    {
        Coordinate coord = PointToCoordinate(point);
        float u = coord.longitude / (2 * Mathf.PI) + 0.5f;
        float v = coord.latitude / Mathf.PI + 0.5f;

        return new Vector2(u, v);
    }
}

public struct Coordinate
{
    public float longitude;
    public float latitude;

    public Coordinate(float lon, float lat)
    {
        this.longitude = lon;
        this.latitude = lat;
    }
}
