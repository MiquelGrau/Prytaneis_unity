using UnityEngine;

public class ShowUVs : MonoBehaviour
{
    public GameObject uvPrefab; // Assigna aquí la petita esfera que has creat.
    public TerrainFace terrainFace; // Assigna la cara de l'esfera des d'on vols agafar les coordenades UV.

    void Start()
    {
        Vector3[] vertices = terrainFace.GetVertices();
        Vector2[] uvs = terrainFace.GetUVs();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 uv = uvs[i];
            Vector3 position = new Vector3(uv.x * transform.localScale.x, 0, uv.y * transform.localScale.z);
            Instantiate(uvPrefab, position, Quaternion.identity, transform);
        }
    }
}
