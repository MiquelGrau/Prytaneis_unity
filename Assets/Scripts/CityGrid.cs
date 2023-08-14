using UnityEngine;

public class CityGrid : MonoBehaviour
{
    public BuildingDataManager buildingDataManager; 
    public CityDataManager cityDataManager;

    [SerializeField]
    private float cellSize = 100f;

    private void Start()
    {
        RenderCity();
    }

    private void RenderCity()
    {
        if (cityDataManager == null)
        {
            Debug.LogError("cityDataManager no està assignat a CityGrid.");
            return;
        }

        if (cityDataManager.dataItems.cities == null || cityDataManager.dataItems.cities.Count == 0)
        {
            Debug.LogError("No hi ha ciutats disponibles a dataItems.");
            return;
        }

        CityData currentCity = cityDataManager.dataItems.cities[0];

        Debug.Log("Current City: " + (currentCity == null ? "NULL" : currentCity.cityName));
        Debug.Log("Current City Grid: " + (currentCity.grid == null ? "NULL" : "Contains Data"));
        Debug.Log("Grid Rows: " + currentCity.grid.Length);
        if (currentCity.grid.Length > 0)
            Debug.Log("Grid Columns of First Row: " + currentCity.grid[0].Length);

        
        if (currentCity == null)
        {
            Debug.LogError("La ciutat actual (index 0) no es pot carregar.");
            return;
        }

        if (currentCity.grid == null)
        {
            Debug.LogError("El grid de la ciutat actual és nul·la.");
            return;
        }

        for (int y = 0; y < currentCity.grid.Length; y++)
        {
            for (int x = 0; x < currentCity.grid[y].Length;)
            {
                string buildingType = currentCity.grid[y][x];

                if (buildingType.StartsWith("ref-"))
                {
                    x++;
                    continue;
                }

                BuildingDefinition buildingDef = buildingDataManager.GetBuildingDefinition(buildingType);
                if(buildingDef == null)
                {
                    Debug.LogError($"No es pot trobar la definició per a l'edifici de tipus {buildingType}");
                    x++;
                    continue;
                }

                GameObject buildingPrefab = buildingDataManager.GetBuildingPrefab(buildingDef);
                if (buildingPrefab == null)
                {
                    Debug.LogError($"No es pot trobar el prefab per a l'edifici de tipus {buildingType}");
                    x++;
                    continue;
                }

                float posX = (x * cellSize + y * cellSize) / 2f;
                float posZ = (x * cellSize - y * cellSize) / 4f;
                Vector3 position = new Vector3(posX, 0, posZ);

                GameObject instance = Instantiate(buildingPrefab, position, Quaternion.identity, transform);
                instance.transform.localScale = new Vector3(buildingDef.width, 1, buildingDef.height);

                x += buildingDef.width;
            }
        }

    }
}