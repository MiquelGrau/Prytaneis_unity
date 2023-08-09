using UnityEngine;

[System.Serializable]
public class BuildingDefinition
{
    public string buildingType;  // Nom de l'edifici (e.g., "Hospital1")
    public Vector2Int size;      // Mida de l'edifici dins de la matriu/grid (e.g., 4x3)
    public GameObject prefab;    // Prefab d'Unity per representar visualment aquest edifici
}
