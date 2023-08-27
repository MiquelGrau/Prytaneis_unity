using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityInfoDisplayManager : MonoBehaviour
{
    public Button toggleCityInventoryButton; // Assigneu el botó dins l'inspector d'Unity
    public TMP_Text citiesInventoryListText; // Assigneu el TextMeshPro dins l'inspector d'Unity
    public CityInventoryManager cityInventoryManager; // Assigneu l'objecte que té el script CityInventoryManager dins l'inspector d'Unity
    public TradeviewUIManager tradeviewUIManager; // Assigneu l'objecte que té el script TradeviewUIManager dins l'inspector d'Unity
    
    private void Start()
    {
        toggleCityInventoryButton.onClick.AddListener(ToggleCityInfo);
        citiesInventoryListText.gameObject.SetActive(false); // Comença amb False, no es mostrarà
    }

    private void ToggleCityInfo()
    {
        if (citiesInventoryListText.gameObject.activeInHierarchy)
        {
            citiesInventoryListText.gameObject.SetActive(false);
        }
        else
        {
            DisplayCityInfo();
            citiesInventoryListText.gameObject.SetActive(true);
        }
    }

    private void DisplayCityInfo()
    {
        CityData currentCity = tradeviewUIManager.CurrentCity;
        
        // Obté la informació de la ciutat per la ciutat actual
        CityInventoryList cityInventory = cityInventoryManager.cityInventories.Find(c => c.cityInventoryID == currentCity.cityID);

        if (cityInventory == null)
        {
            citiesInventoryListText.text = "Informació no disponible";
            return;
        }

        // Format del text per mostrar la informació de la ciutat
        citiesInventoryListText.text = $"Informació de la Ciutat: {currentCity.cityName}\n";
        citiesInventoryListText.text += $"ID d'inventari: {cityInventory.cityInventoryID}\n";
        citiesInventoryListText.text += "Items:\n";
        
        foreach (var item in cityInventory.cityInventoryItems)
        {
            citiesInventoryListText.text += $"Resource ID: {item.resourceID}, Quantity: {item.quantity}, Current Price: {item.currentPrice}\n";
        }
        
    }
}
