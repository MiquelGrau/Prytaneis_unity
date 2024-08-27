using UnityEngine;
using System.Collections.Generic;

public class TerrainFace {
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    private Vector3[] vertices;
    private Vector2[] uvs;
    public float heightMultiplier = 0.1f;
    public Texture2D heightMap;
    public MeshRenderer MeshRenderer { get; private set; }
    private List<Chunk> chunks = new List<Chunk>();
    public Material earthMaterial;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp, Texture2D heightMap, float heightMultiplier, MeshRenderer meshRenderer, Material earthMaterial) {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.heightMap = heightMap;
        this.heightMultiplier = heightMultiplier;
        this.MeshRenderer = meshRenderer;
        this.earthMaterial = earthMaterial;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        int chunksPerFace = Mathf.CeilToInt((float)resolution / 40);
        for (int y = 0; y < chunksPerFace; y++) {
            for (int x = 0; x < chunksPerFace; x++) {
                Chunk newChunk = new Chunk(meshRenderer.transform, earthMaterial, 40);
                chunks.Add(newChunk);
            }
        }
    }

    public void ConstructMesh() {
        int chunkResolution = 40;
        int chunksPerFace = Mathf.CeilToInt((float)resolution / chunkResolution);

        for (int y = 0; y < chunksPerFace; y++) {
            for (int x = 0; x < chunksPerFace; x++) {
                int chunkIndex = x + y * chunksPerFace;
                chunks[chunkIndex].UpdateChunk(x * chunkResolution, y * chunkResolution, chunkResolution, resolution, localUp, axisA, axisB, heightMap, heightMultiplier);
            }
        }
    }

    public Vector3[] GetVertices() {
        return vertices;
    }

    public Vector2[] GetUVs() {
        return uvs ?? new Vector2[0];
    }

    public List<Chunk> GetChunks() {
        return chunks;
    }
}
