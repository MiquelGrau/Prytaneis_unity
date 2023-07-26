using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int resolution = 10;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;
    public Material earthMaterial;
    public Texture2D heightMap;
    public float heightMultiplier = 0.1f;
    private Camera mainCamera;
    private ZoomCamera zoomCamera;
    private RotateObject rotateObject;
    private List<GameObject> objectsToDestroy = new List<GameObject>();

    private void Start() {
        ManuallyUpdatePlanet();

        mainCamera = Camera.main;
        zoomCamera = mainCamera.GetComponent<ZoomCamera>();
        rotateObject = GetComponent<RotateObject>();
    }
     
    public void ManuallyUpdatePlanet()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        // Elimina els chunks i les meshes existents
        foreach (Transform child in transform)
        {
            objectsToDestroy.Add(child.gameObject);
        }
        foreach (var obj in objectsToDestroy)
        {
            DestroyImmediate(obj);
        }
        objectsToDestroy.Clear();

        meshFilters = new MeshFilter[6];
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = earthMaterial;
            meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i], heightMap, heightMultiplier, meshObj.GetComponent<MeshRenderer>(), earthMaterial);
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void Update()
    {
        if (zoomCamera.HasZoomed || rotateObject.HasRotated)
        {
            CheckVisibility();
        }
    }

    void CheckVisibility()
    {
        if (terrainFaces == null)
        {
            return;
        }
        if (mainCamera != null) 
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            foreach (var face in terrainFaces)
            {
                if (face != null)  // Afegeix aquesta comprovació
                {
                    var chunks = face.GetChunks();
                    if (chunks != null)
                    {
                        foreach (var chunk in chunks)
                        {
                            if (chunk != null && chunk.Renderer != null)
                            {
                                bool isVisible = chunk.IsVisibleFrom(mainCamera);
                                if (chunk.Renderer.enabled != isVisible)
                                {
                                    chunk.Renderer.enabled = isVisible;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
