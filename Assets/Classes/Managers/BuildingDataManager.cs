using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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
            buildingDefinitions = new BuildingDefinitionCollection();
            
            // Utilitza Newtonsoft.Json per a la deserialització
            buildingDefinitions.buildings = JsonConvert.DeserializeObject<Dictionary<string, BuildingDefinition>>(json);
            
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

    public GameObject GetBuildingPrefab(BuildingDefinition buildingDef)
    {
        if (buildingDef == null)
            return null;

        if (!buildingPrefabs.ContainsKey(buildingDef.prefabName))
        {
            // Aquí intentes carregar el prefab des dels recursos
            GameObject prefab = Resources.Load<GameObject>("Prefab/Buildings/" + buildingDef.prefabName);
            if (prefab == null)
            {
                Debug.LogError($"No es pot trobar el prefab amb el nom {buildingDef.prefabName}");
                return null;
            }
            buildingPrefabs[buildingDef.prefabName] = prefab;
        }
        return buildingPrefabs[buildingDef.prefabName];
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

