using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEarthGenerator : MonoBehaviour
{
    public int resolution = 40;
    public Material earthMaterial;  // Material amb el shader Unlit/Texture

    private void Start()
    {
        GenerateFlatEarth();
    }

    void GenerateFlatEarth()
    {
        // Crear un nou GameObject per al pla de la Terra
        GameObject face = new GameObject("EarthPlane");
        face.transform.parent = transform;

        // Afegir components per renderitzar la malla
        MeshRenderer meshRenderer = face.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = earthMaterial;  // Assignar el nou material

        MeshFilter meshFilter = face.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateFlatMesh();

        // Aplicar escala de 0.01 a tots els eixos
        face.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    Mesh CreateFlatMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        // Recórrer cada punt de la malla
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                float longitude = percent.x * 360f - 180f; // Convertir percent a longitud (-180 a 180)
                float latitude = percent.y * 180f - 90f; // Convertir percent a latitud (-90 a 90)
                float elevation = 0f; // Elevació inicial fixada a 0

                vertices[i] = new Vector3(longitude, latitude, elevation);
                uvs[i] = percent;

                // Definir els triangles de la malla
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // Definir l'ordre dels triangles per assegurar normals correctes
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution;
                    triangles[triIndex + 2] = i + resolution + 1;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + resolution + 1;
                    triangles[triIndex + 5] = i + 1;
                    triIndex += 6;
                }
            }
        }

        // Crear la malla i assignar les coordenades
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

    // Estructura per mantenir les coordenades de longitud, latitud i elevació
    public struct Coordinate
    {
        public float longitude;
        public float latitude;
        public float elevation;

        public Coordinate(float lon, float lat, float elev)
        {
            this.longitude = lon;
            this.latitude = lat;
            this.elevation = elev;
        }
    }
}
