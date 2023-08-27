using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Marker : MonoBehaviour
{
    public GameObject contextMenuPrefab;
    public string cityName;
    public Vector3 position;
    private GameObject contextMenuInstance;

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // 1 és el botó dret del ratolí
        {
            RouteManager routeManager = FindObjectOfType<RouteManager>();
            if (routeManager)
            {
                CityData agentCityData = GetCurrentAgentCityData();
                if (agentCityData != null)
                {
                    CityData destinationCityData = GetCityDataByName(cityName);
                    if(destinationCityData != null)
                    {
                        routeManager.CreateRoute(agentCityData, destinationCityData);
                    }
                    else
                    {
                        Debug.LogError($"No es pot trobar les dades de la ciutat amb el nom {cityName}.");
                    }
                }
            }
            else
            {
                Debug.LogError("No s'ha trobat RouteManager a l'escena.");
            }
        }
    }

    private CityData GetCurrentAgentCityData()
    {
        Agent currentAgent = GameData.Instance.SelectedAgent;
        if (currentAgent != null)
        {
            CityDataManager cityDataManager = FindObjectOfType<CityDataManager>();
            if (cityDataManager && cityDataManager.dataItems != null)
            {
                return cityDataManager.dataItems.cities.FirstOrDefault(city => city.cityID == currentAgent.currentCityID);
            }
        }
        return null;
    }

    private CityData GetCityDataByName(string name)
    {
        CityDataManager cityDataManager = FindObjectOfType<CityDataManager>();
        if (cityDataManager && cityDataManager.dataItems != null)
        {
            return cityDataManager.dataItems.cities.FirstOrDefault(city => city.cityName == name);
        }
        return null;
    }

    private void OnMouseDown()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "WorldScene":
                PlayerPrefs.SetString("SelectedCity", cityName);
                SceneManager.LoadScene("CityScene");
                break;

            case "RouteScene":
                // Ací posa la nova funció que vols realitzar quan estàs a RouteScene
                break;

            default:
                Debug.LogWarning("Acció no definida per a aquesta escena.");
                break;
        }
    }
}
