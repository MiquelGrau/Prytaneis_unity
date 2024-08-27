using UnityEngine;

public class Chunk
{
    public Mesh ChunkMesh { get; private set; }
    public MeshRenderer Renderer { get; private set; }
    public Vector3[] Vertices { get; private set; }

    public Chunk(Transform parent, Material material, int resolution)
    {
        GameObject chunkObj = new GameObject("Chunk");
        chunkObj.transform.parent = parent;

        Renderer = chunkObj.AddComponent<MeshRenderer>();
        Renderer.material = material;

        MeshFilter meshFilter = chunkObj.AddComponent<MeshFilter>();
        ChunkMesh = new Mesh();
        meshFilter.sharedMesh = ChunkMesh;
    }

    public void UpdateChunk(int startX, int startY, int chunkResolution, int resolution, Vector3 localUp, Vector3 axisA, Vector3 axisB, Texture2D heightMap, float heightMultiplier)
{
    int extendedResolution = chunkResolution + 2;
    Vector3[] vertices = new Vector3[extendedResolution * extendedResolution];
    Vector2[] uvs = new Vector2[vertices.Length];
    int[] triangles = new int[chunkResolution * chunkResolution * 6];
    int triIndex = 0;

    for (int y = startY - 1; y < startY + chunkResolution + 1; y++)
    {
        for (int x = startX - 1; x < startX + chunkResolution + 1; x++)
        {
            int i = (x - (startX - 1)) + (y - (startY - 1)) * extendedResolution;
            Vector2 percent = new Vector2(x, y) / (resolution - 1);
            Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
            Vector3 pointOnUnitSphere = PointOnCubeToPointOnSphere(pointOnUnitCube);

            // Convert coordinates to longitude and latitude
            float lon = Mathf.Atan2(pointOnUnitSphere.z, pointOnUnitSphere.x);
            float lat = Mathf.Asin(pointOnUnitSphere.y);

            // Convert longitude and latitude to UV coordinates
            float u = lon / (2f * Mathf.PI) + 0.5f;
            float v = lat / Mathf.PI + 0.5f;

            // Ensure UV coordinates are within [0, 1]
            if (u < 0) u += 1;
            else if (u > 1) u -= 1;
            if (v < 0) v += 1;
            else if (v > 1) v -= 1;

            uvs[i] = new Vector2(u, v);

            // Retrieve height from the height map
            float heightValue = heightMap.GetPixelBilinear(uvs[i].x, uvs[i].y).grayscale;
            vertices[i] = pointOnUnitSphere * (1 + heightValue * heightMultiplier);

            // Create triangles for the mesh
            if (x < startX + chunkResolution && y < startY + chunkResolution && x >= startX && y >= startY)
            {
                triangles[triIndex] = i;
                triangles[triIndex + 1] = i + extendedResolution + 1;
                triangles[triIndex + 2] = i + extendedResolution;

                triangles[triIndex + 3] = i;
                triangles[triIndex + 4] = i + 1;
                triangles[triIndex + 5] = i + extendedResolution + 1;
                triIndex += 6;
            }
        }
    }

    ChunkMesh.Clear();
    ChunkMesh.vertices = vertices;
    ChunkMesh.triangles = triangles;
    ChunkMesh.uv = uvs;
    ChunkMesh.RecalculateNormals();
    ChunkMesh.RecalculateBounds();
}




    public bool IsVisibleFrom(Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        bool isWithinFrustum = GeometryUtility.TestPlanesAABB(planes, Renderer.bounds);

        Vector3 directionToChunk = Renderer.bounds.center.normalized;
        Vector3 directionToCamera = cam.transform.position.normalized;
        float angleBetween = Vector3.Angle(directionToChunk, directionToCamera);

        bool isFacingCamera = angleBetween < 90f;

        return isWithinFrustum && isFacingCamera;
    }

    private Vector3 PointOnCubeToPointOnSphere(Vector3 point)
    {
        float x2 = point.x * point.x;
        float y2 = point.y * point.y;
        float z2 = point.z * point.z;

        Vector3 sPoint = new Vector3(
            point.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f),
            point.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f),
            point.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f)
        );

        return sPoint;
    }
}
