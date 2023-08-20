using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SphereWorld : MonoBehaviour
{
    private Mesh mesh;
    public int gridSize = 100;
    private List<Vector2> uvs;

    private struct MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        uvs = new List<Vector2>();

        MeshData[] faces = GenerateFaces(gridSize);
        List<Vector3> allVertices = new List<Vector3>();
        List<int> allTriangles = new List<int>();

        foreach (MeshData face in faces)
        {
            int vertexOffset = allVertices.Count;
            allVertices.AddRange(face.vertices);
            allTriangles.AddRange(face.triangles.Select(tri => tri + vertexOffset));
        }

        mesh.vertices = allVertices.ToArray();
        mesh.triangles = allTriangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
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
                vertices[vertexIndex] = PointOnCubeToPointOnSphere(point);

                Vector2 uv = SpherePointToUV(vertices[vertexIndex]);
                uvs.Add(uv);

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

    private Vector2 SpherePointToUV(Vector3 point)
    {
        float longitude = Mathf.Atan2(point.z, point.x);
        float latitude = Mathf.Asin(point.y);

        float u = Mathf.InverseLerp(-Mathf.PI, Mathf.PI, longitude);
        float v = Mathf.InverseLerp(-Mathf.PI / 2f, Mathf.PI / 2f, latitude);

        return new Vector2(u, v);
    }
}
