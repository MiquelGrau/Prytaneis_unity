using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BuildingDataManager : MonoBehaviour
{
    private string dataPath;
    public BuildingDefinitionCollection buildingDefinitions;
    public Dictionary<string, GameObject> buildingPrefabs = new Dictionary<string, GameObject>();
    public GameObject housePrefab;

    private void Awake()
    {
        LoadData();
    }

    public void LoadData()
    {
        TextAsset buildingData = Resources.Load<TextAsset>("BuildingDefinitions");
        if (buildingData != null)
        {
            string json = buildingData.text;
            buildingDefinitions = JsonUtility.FromJson<BuildingDefinitionCollection>(json);
            Debug.Log("Building data loaded: " + json);
        }
        else
        {
            Debug.LogError("Building definition file not found!");
        }
    }

    public BuildingDefinition GetBuildingDefinition(string buildingType)
    {
        if (buildingDefinitions.buildings != null && buildingDefinitions.buildings.ContainsKey(buildingType))
        {
            return buildingDefinitions.buildings[buildingType];
        }
        return null;
    }

    public void InstantiateHouse(Vector3 position)
    {
        Instantiate(housePrefab, position, Quaternion.identity);
    }
}

[System.Serializable]
public class BuildingDefinitionList
{
    public List<BuildingDefinition> buildings;
}

[System.Serializable]
public class BuildingDefinitionCollection
{
    public Dictionary<string, BuildingDefinition> buildings;
}

