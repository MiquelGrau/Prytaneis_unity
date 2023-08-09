using UnityEngine;
using TMPro;

public class CityController : MonoBehaviour
{
    public CityDataManager cityDataManager;
    public BuildingDataManager buildingDataManager; // Referència afegida
    public TMP_Text cityNameText;

    void Start()
    {
        if (cityDataManager.dataItems == null || cityDataManager.dataItems.cities.Count == 0)
        {
            Debug.LogError("No s'han carregat les dades de la ciutat!");
            return;
        }

        CityData defaultCity = cityDataManager.dataItems.cities[0];
        DisplayCityData(defaultCity);
    }

    void DisplayCityData(CityData cityData)
    {
        cityNameText.text = cityData.cityName;
        
        // Aquí, també hauries de carregar i mostrar els edificis basats en cityData.buildings
        foreach (var building in cityData.buildings)
        {
            // Obtenir la definició de l'edifici a partir de BuildingDataManager
            BuildingDefinition buildingDef = buildingDataManager.GetBuildingDefinition(building.buildingType);

            // Utilitza aquesta informació per crear/instància els edificis en la posició correcta
            // Potser necessitaràs una funció o lògica per fer això
            PlaceBuilding(buildingDef, building.position);
        }
    }

    void PlaceBuilding(BuildingDefinition buildingDef, Vector2Int position)
    {
        if (buildingDef == null || buildingDef.prefab == null)
        {
            Debug.LogError($"No s'ha trobat el prefab per a l'edifici {buildingDef.buildingType}");
            return;
        }

        Vector3 spawnPosition = new Vector3(position.x, 0, position.y);
        Instantiate(buildingDef.prefab, spawnPosition, Quaternion.identity);
    }
}
