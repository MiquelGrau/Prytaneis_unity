using UnityEngine;
using System.IO;

public class CityGrid : MonoBehaviour
{
    public BuildingDataManager buildingDataManager; 
    public CityDataManager cityDataManager; 
    
    private void Start()
    {
        RenderCity();
    }

    private void RenderCity()
    {
        // Suposant que estiguis treballant amb la primera ciutat a la llista
        CityData currentCity = cityDataManager.dataItems.cities[0];
        
        foreach (BuildingInstanceData buildingData in currentCity.buildings)
        {
            BuildingDefinition buildingDef = buildingDataManager.GetBuildingDefinition(buildingData.buildingType);
            if (buildingDef == null)
            {
                Debug.LogError($"No es pot trobar la definició per a l'edifici de tipus {buildingData.buildingType}");
                continue;
            }

            Vector3 position = new Vector3(buildingData.position.x, 0, buildingData.position.y);
            Instantiate(buildingDef.prefab, position, Quaternion.identity, transform);
        }
    }
}
