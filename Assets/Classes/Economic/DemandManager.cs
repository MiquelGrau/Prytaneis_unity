using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Text;
using UnityEngine;

public class DemandManager : MonoBehaviour
{

    public DataManager<CityDataList> dataManager; 
    public CityData currentCity  { get; private set; }
    public CityInventory currentCityInventory { get; private set; } 
    
    public List<CityData> cities; 
    public DatabaseImporter databaseImporter; 
    public List<LifestyleTier> lifestyleTiers; 
    
    private float timer = 0f;
    private float timeToWait = 6f; 
    
    public TextMeshProUGUI inventoryDisplayText; 

    private void Start()
    {  
        Debug.Log("Iniciant DemandManager...");
        
        // Obté el llistat de ciutats
        cities = dataManager.GetCities(); 
        if (cities == null || cities.Count == 0)        // Comprova si 'cities' és null
        {
            Debug.LogError("No s'han pogut carregar les ciutats.");
            return;
        }
        
        // Assigna la ciutat actual
        AssignCurrentCity("C0001");
        lifestyleTiers = databaseImporter.lifestyleTiers; 
        
        
        // Calcula les demandes basades en la ciutat actual
        GetTierNeedsForCity();
        Debug.Log("Demandes per població a la ciutat calculades");

        

    }

    private void Update()
    {
        timer += Time.deltaTime; // Aquesta línia incrementa el temporitzador basant-se en el temps real que ha passat

        if (timer >= timeToWait)
        {
            // Reset del temporitzador
            timer = 0f;

            Debug.Log("Tic tac! Temporitzador cada 6 segons.");

            AssignDemandsToVarieties();
            inventoryDisplayText.text = GetCityInventoryDisplayText();
            
        }
    }


    private void AssignCurrentCity(string cityID)
    {
        currentCity = cities.Find(city => city.cityID == cityID);

        if (currentCity != null)
        {
            Debug.Log($"Ciutat assignada: {currentCity.cityID}, {currentCity.cityName}");
            currentCityInventory = currentCity.CityInventory; 
            if (currentCityInventory != null)
            {
                Debug.Log($"Inventari de la ciutat assignada: {currentCityInventory.CityInvID}");
            }
            else
            {
                Debug.LogError($"No s'ha trobat l'inventari per a la ciutat '{currentCity.cityName}'");
            }
            Debug.Log($"PoorLifestyleID: {currentCity.PoorLifestyleID}, MidLifestyleID: {currentCity.MidLifestyleID}, RichLifestyleID: {currentCity.RichLifestyleID}");
        }
        else
        {
            Debug.LogError($"No s'ha trobat cap ciutat amb l'ID '{cityID}'");
        }
    }

    private void GetTierNeedsForCity()
    {
        if (currentCity == null)
        {
            Debug.LogError("No s'ha assignat cap ciutat actual.");
            return;
        }

        // Netejar les CityDemands existents
        currentCityInventory.Demands.Clear();

        // Obtenir els LifestyleTiers per a cada grup de població
        LifestyleTier[] specificTiers = {
            lifestyleTiers.Find(tier => tier.TierID == currentCity.PoorLifestyleID),
            lifestyleTiers.Find(tier => tier.TierID == currentCity.MidLifestyleID),
            lifestyleTiers.Find(tier => tier.TierID == currentCity.RichLifestyleID)
        };

        int[] populations = { currentCity.PoorPopulation, currentCity.MidPopulation, currentCity.RichPopulation };
        string[] populationNames = { "pobra", "mitjana", "rica" };


        // Inicialitzar o resetejar les línies de CityInventoryItem per a cada ResourceType
        foreach (var tier in specificTiers)
        {
            foreach (var need in tier.LifestyleDemands)
            {
                var headerItem = currentCityInventory.InventoryItems
                    .FirstOrDefault(item => item.ResourceType == need.resourceType && item.ResourceID == null);

                if (headerItem == null)
                {
                    headerItem = new CityInventoryItem(need.resourceType);
                    currentCityInventory.InventoryItems.Add(headerItem);
                }
                else
                {
                    headerItem.Quantity = 0;
                    headerItem.DemandConsume = 0;
                    headerItem.DemandCritical = 0;
                    headerItem.DemandTotal = 0;
                }
            }
        }

        // Ara calcula les demandes per a cada grup de població. Es queda guardat a DEMANDS, no a inventory. 
        for (int i = 0; i < specificTiers.Length; i++)
        {
            LifestyleTier tier = specificTiers[i];
            // Debug part
            if (tier == null)
            {
                Debug.LogError($"El tier per a la població {populationNames[i]} és null.");
                continue;
            }
            
            // Per a cada tier que tinc a la ciutat (es a dir, els 3 que hi ha), llista les needs i volca-ho al CityInventory
            foreach (var need in tier.LifestyleDemands)
            {
                
                // Inicialitzar o resetejar les línies de CityInventoryItem basades en ResourceType
                var headerItem = currentCityInventory.InventoryItems
                .FirstOrDefault(item => item.ResourceType == need.resourceType && item.ResourceID == null);
                
                CityInventory.CityDemands newDemand = new CityInventory.CityDemands(
                    CityInventory.CityDemands.DemandType.ResourceType, 
                    need.resourceType, 
                    populationNames[i], 
                    need.resourceVariety);
                
                // Calcular les demandes
                newDemand.DemandConsume = populations[i] * need.quantityPerThousand / 1000;
                newDemand.DemandCritical = newDemand.DemandConsume * need.monthsCritical;
                newDemand.DemandTotal = newDemand.DemandConsume * need.monthsTotal;

                // Afegir a la llista de CityDemands
                currentCityInventory.Demands.Add(newDemand);

                    
                // Mou al InventoryItem
                headerItem.DemandConsume += newDemand.DemandConsume;
                headerItem.DemandCritical += newDemand.DemandCritical;
                headerItem.DemandTotal += newDemand.DemandTotal;
     
                // Log de la informació
                Debug.Log($"ResourceType: {newDemand.ResourceType}, PopulationType: {newDemand.PopulationType}, " +
                        $"Variety: {newDemand.Variety}, Demands: {newDemand.DemandConsume} / " +
                        $"{newDemand.DemandCritical} / {newDemand.DemandTotal}");
                
            }
        }

    }

    public void AssignDemandsToVarieties()
    {
        // Primer, netejar les demandes existents en els items d'inventari
        foreach (var item in currentCityInventory.InventoryItems)
        {
            if (item.ResourceID != null)
            {
                item.DemandConsume = 0;
                item.DemandCritical = 0;
                item.DemandTotal = 0;
            }
        }

        // Assignar les demandes a les varietats
        foreach (var demand in currentCityInventory.Demands)
        {
            // Ordenar els items d'inventari per quantitat, de major a menor
            var sortedInventoryItems = currentCityInventory.InventoryItems
                .Where(item => item.ResourceType == demand.ResourceType && item.ResourceID != null)
                .OrderByDescending(item => item.Quantity)
                .ToList();

            // Assignar les demandes a les varietats
            for (int i = 0; i < demand.Variety && i < sortedInventoryItems.Count; i++)
            {
                var item = sortedInventoryItems[i];
                item.DemandConsume += demand.DemandConsume / demand.Variety;
                item.DemandCritical += demand.DemandCritical / demand.Variety;
                item.DemandTotal += demand.DemandTotal / demand.Variety;

                // Buscar el nom del recurs
                var matchedResource = DatabaseImporter.resources.FirstOrDefault(r => r.resourceID == item.ResourceID);
                string resourceName = matchedResource != null ? matchedResource.resourceName : "Desconegut";

                // Afegir log per a cada item
                Debug.Log($"ResourceType: {item.ResourceType}, ID: {item.ResourceID}, {resourceName}, Qty: {item.Quantity} " +
                $"Demands: {item.DemandConsume} / {item.DemandCritical} / {item.DemandTotal}");
            }
        }

        // Sumar les quantitats per a cada ResourceType i assignar-les als elements header
        foreach (var resourceType in currentCityInventory.InventoryItems.Select(item => item.ResourceType).Distinct())
        {
            var headerItem = currentCityInventory.InventoryItems
                .FirstOrDefault(item => item.ResourceType == resourceType && item.ResourceID == null);

            if (headerItem != null)
            {
                float totalQuantity = currentCityInventory.InventoryItems
                    .Where(item => item.ResourceType == resourceType && item.ResourceID != null)
                    .Sum(item => item.Quantity);

                headerItem.Quantity = totalQuantity;

                // Opcional: Afegir un log per confirmar l'assignació
                Debug.Log($"HeaderItem per a {resourceType}: Total Quantity = {totalQuantity}");
            }
        }


    }
    
    // DISPLAY TEXTS

    private string GetCityInventoryDisplayText()
    {
        if (currentCityInventory == null)
        {
            return "No s'ha assignat cap inventari de ciutat.";
        }

        StringBuilder displayText = new StringBuilder();
        displayText.AppendLine($"Inventari de la Ciutat: {currentCity.cityName} (ID: {currentCity.cityID})");
        displayText.AppendLine("Items d'Inventari:");
        
        foreach (var item in currentCityInventory.InventoryItems)
        {
            displayText.AppendLine($"{item.ResourceID}, Type: {item.ResourceType}, " +
                                $"Qty: {item.Quantity}, Demands: {item.DemandConsume} / " +
                                $"{item.DemandCritical} / {item.DemandTotal}");
        }

        displayText.AppendLine("\nDemandes:");
        foreach (var demand in currentCityInventory.Demands)
        {
            displayText.AppendLine($"- Rtype: {demand.ResourceType}, PopType: {demand.PopulationType}, " +
                                $"Variety: {demand.Variety}, Demands: {demand.DemandConsume} /" +
                                $" {demand.DemandCritical} / {demand.DemandTotal}");
        }

        return displayText.ToString();
    }

    
}