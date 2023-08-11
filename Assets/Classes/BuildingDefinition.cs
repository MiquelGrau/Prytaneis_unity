using UnityEngine;

[System.Serializable]
public class BuildingDefinition
{
    public string buildingType; // nom del tipus d'edifici, ex: "House1"
    public string prefabName;   // el nom del prefab a la carpeta Resources
    public int width;
    public int height;
    // altres propietats si són necessàries
}

