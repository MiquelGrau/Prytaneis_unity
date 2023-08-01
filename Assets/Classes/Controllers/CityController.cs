using UnityEngine;
using TMPro;

public class CityController : MonoBehaviour
{
    public CityDataManager cityDataManager;
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
        // Aquí, actualitza la UI o qualsevol altre component de la cityScene amb la informació de cityData.
        // Per exemple, si tens un text que mostra el nom de la ciutat:
        cityNameText.text = cityData.cityName;
    }
}
