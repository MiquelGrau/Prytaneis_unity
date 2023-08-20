using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ColoredCubeGenerator : MonoBehaviour
{
    private Mesh mesh;
    public int resolution = 2;
    
    private struct MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
    }

    public struct Coordinate
    {
        public float Latitude; // in radians
        public float Longitude; // in radians

        public Coordinate(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        MeshData[] faces = GenerateFaces(resolution);
        List<Vector3> allVertices = new List<Vector3>();
        List<int> allTriangles = new List<int>();

        foreach (MeshData face in faces)
        {
            int vertexOffset = allVertices.Count;
            allVertices.AddRange(face.vertices.Select(PointOnCubeToPointOnSphere));
            allTriangles.AddRange(face.triangles.Select(tri => tri + vertexOffset));
        }

        mesh.vertices = allVertices.ToArray();
        mesh.triangles = allTriangles.ToArray();
        mesh.RecalculateNormals();

        Color[] faceColors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };
        Color[] vertexColors = new Color[allVertices.Count];

        int verticesPerFace = resolution * resolution;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < verticesPerFace; j++)
            {
                vertexColors[i * verticesPerFace + j] = faceColors[i];
            }
        }
        mesh.colors = vertexColors;
    }

    // Conversion functions
    public static Coordinate PointToCoordinate(Vector3 pointOnUnitSphere) 
    {
        float latitude = Mathf.Asin(pointOnUnitSphere.y);
        float longitude = Mathf.Atan2(pointOnUnitSphere.x, -pointOnUnitSphere.z);
        return new Coordinate(latitude, longitude);
    }

    public static Vector3 CoordinateToPoint(Coordinate coord) 
    {
        Vector3 point;
        point.x = Mathf.Cos(coord.Latitude) * Mathf.Sin(coord.Longitude);
        point.y = Mathf.Sin(coord.Latitude);
        point.z = Mathf.Cos(coord.Latitude) * Mathf.Cos(coord.Longitude);
        return point.normalized;
    }

    private Vector3 PointOnCubeToPointOnSphere(Vector3 point)
    {
        float x2 = point.x * point.x;
        float y2 = point.y * point.y;
        float z2 = point.z * point.z;

        Vector3 sPoint = new Vector3(
            point.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f),
            point.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f),
            point.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f));

        return sPoint;
    }

    private MeshData CreateFace(Vector3 normal, int resolution, int faceIndex)
    {
        Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
        Vector3 axisB = Vector3.Cross(normal, axisA);
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int vertexIndex = x + y * resolution;
                Vector2 t = new Vector2(x, y) / (resolution - 1f);
                Vector3 point = normal + axisA * (2 * t.x - 1) + axisB * (2 * t.y - 1);

                // Calculate latitude and longitude for the point
                Coordinate coord = PointToCoordinate(point);

                vertices[vertexIndex] = point;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex++] = vertexIndex;
                    triangles[triIndex++] = vertexIndex + resolution + 1;
                    triangles[triIndex++] = vertexIndex + resolution;
                    triangles[triIndex++] = vertexIndex;
                    triangles[triIndex++] = vertexIndex + 1;
                    triangles[triIndex++] = vertexIndex + resolution + 1;
                }
            }
        }

        return new MeshData { vertices = vertices, triangles = triangles };
    }

    private MeshData[] GenerateFaces(int resolution)
    {
        Vector3[] faceNormals = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        MeshData[] allMeshData = new MeshData[6];
        for (int i = 0; i < faceNormals.Length; i++)
        {
            allMeshData[i] = CreateFace(faceNormals[i], resolution, i);
        }

        return allMeshData;
    }
}
