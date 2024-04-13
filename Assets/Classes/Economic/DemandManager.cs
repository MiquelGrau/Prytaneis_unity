using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Text;
using UnityEngine;

public class DemandManager : MonoBehaviour
{
    
    public DataManager dataManager; 
    //public CityData currentCity  { get; private set; }
    //public CityInventory currentCityInventory { get; private set; } 
    
    //public List<CityData> cities; 
    public List<LifestyleTier> lifestyleTiers; 
    
    private float timer = 0f;
    private float timeToWait = 6f; 
    
    public TextMeshProUGUI inventoryDisplayText; 

    private void Start()
    {  
        Debug.Log("Iniciant DemandManager...");
        // Obté el llistat de ciutats
        /* cities = dataManager.GetCities(); 
        if (cities == null || cities.Count == 0)        // Comprova si 'cities' és null
        {
            Debug.LogError("No s'han pogut carregar les ciutats.");
            return;
        } */
        
        // Assigna la ciutat actual des de GameManager
        //AssignCurrentCity(GameManager.Instance.CurrentCity);
        lifestyleTiers = DataManager.lifestyleTiers; 
        
        
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
            CalculatePrices();
        }
    }

    private void GetTierNeedsForCity() 
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;
        if (currentCity == null)
        {
            Debug.LogError("No s'ha assignat cap ciutat actual.");
            return;
        }
        if (currentCityInventory == null)
        {
            Debug.LogError("No s'ha assignat cap inventari de ciutat.");
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


        // Inicialitzar o resetejar les línies de CityInventoryResource per a cada ResourceType
        foreach (var tier in specificTiers)
        {
            foreach (var need in tier.LifestyleDemands)
            {
                var headerResource = currentCityInventory.InventoryResources
                    .FirstOrDefault(resline => resline.ResourceType == need.resourceType && resline.ResourceID == null);

                if (headerResource == null)
                {
                    headerResource = new CityInventoryResource(need.resourceType);
                    currentCityInventory.InventoryResources.Add(headerResource);
                }
                else
                {
                    headerResource.Quantity = 0;
                    headerResource.DemandConsume = 0;
                    headerResource.DemandCritical = 0;
                    headerResource.DemandTotal = 0;
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
                
                // Inicialitzar o resetejar les línies de CityInventoryResource basades en ResourceType
                var headerResource = currentCityInventory.InventoryResources
                .FirstOrDefault(resline => resline.ResourceType == need.resourceType && resline.ResourceID == null);
                
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

                    
                // Mou al Inventoryresource
                headerResource.DemandConsume += newDemand.DemandConsume;
                headerResource.DemandCritical += newDemand.DemandCritical;
                headerResource.DemandTotal += newDemand.DemandTotal;
     
                // Log de la informació
                /* Debug.Log($"ResourceType: {newDemand.ResourceType}, PopulationType: {newDemand.PopulationType}, " +
                        $"Variety: {newDemand.Variety}, Demands: {newDemand.DemandConsume} / " +
                        $"{newDemand.DemandCritical} / {newDemand.DemandTotal}"); */
                
            }
        }

    }

    public void AssignDemandsToVarieties()
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;

        // Primer, netejar les demandes existents de linies de recursos d'inventari
        foreach (var resline in currentCityInventory.InventoryResources)
        {
            if (resline.ResourceID != null)
            {
                resline.DemandConsume = 0;
                resline.DemandCritical = 0;
                resline.DemandTotal = 0;
            }
        }

        // Assignar les demandes a les varietats
        foreach (var demand in currentCityInventory.Demands)
        {
            // Ordenar els resource lines d'inventari per quantitat, de major a menor
            var sortedInventoryResLines = currentCityInventory.InventoryResources
                .Where(resline => resline.ResourceType == demand.ResourceType && resline.ResourceID != null)
                .OrderByDescending(resline => resline.Quantity)
                .ToList();

            // Assignar les demandes a les varietats
            for (int i = 0; i < demand.Variety && i < sortedInventoryResLines.Count; i++)
            {
                var resline = sortedInventoryResLines[i];
                resline.DemandConsume += demand.DemandConsume / demand.Variety;
                resline.DemandCritical += demand.DemandCritical / demand.Variety;
                resline.DemandTotal += demand.DemandTotal / demand.Variety;

                // Buscar el nom del recurs
                var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resline.ResourceID);
                string resourceName = matchedResource != null ? matchedResource.ResourceName : "Desconegut";

                // Afegir log per a cada resource line
                /* Debug.Log($"ResourceType: {resline.ResourceType}, ID: {resline.ResourceID}, {resourceName}, Qty: {resline.Quantity} " +
                $"Demands: {resline.DemandConsume} / {resline.DemandCritical} / {resline.DemandTotal}"); */
            }
        }

        // Sumar les quantitats per a cada ResourceType i assignar-les als elements header
        foreach (var resourceType in currentCityInventory.InventoryResources.Select(resline => resline.ResourceType).Distinct())
        {
            var headerResource = currentCityInventory.InventoryResources
                .FirstOrDefault(resline => resline.ResourceType == resourceType && resline.ResourceID == null);

            if (headerResource != null)
            {
                float totalQuantity = currentCityInventory.InventoryResources
                    .Where(resline => resline.ResourceType == resourceType && resline.ResourceID != null)
                    .Sum(resline => resline.Quantity);

                headerResource.Quantity = totalQuantity;

                // Opcional: Afegir un log per confirmar l'assignació
                //Debug.Log($"HeaderResource per a {resourceType}: Total Quantity = {totalQuantity}");
            }
        }


    }
    
    public void CalculatePrices()
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;

        foreach (var resline in currentCityInventory.InventoryResources)
        {
            if (resline.ResourceID != null)
            {
                // Calcula el quantitat de diferencia respecte la demanda
                float difQty = resline.DemandTotal == 0 ? 3f : 
                    ((float)resline.Quantity - (float)resline.DemandTotal) / (float)resline.DemandTotal;

                // Calcula la price elasticity segons la fórmula donada: y= -0.6x^3 +0.8x^2 -0.6x +1
                float priceElasticity = -0.6f * Mathf.Pow(difQty, 3) + 0.8f * Mathf.Pow(difQty, 2) - 0.6f * difQty + 1f;

                // Assegura't que la price elasticity no sigui negativa, posa valor minim, 25%
                if (priceElasticity < 0f) { priceElasticity = 0.25f; }

                // Troba el base price del recurs
                var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resline.ResourceID);
                float basePrice = matchedResource != null ? matchedResource.BasePrice : 0f;

                // Calcula el CurrentPrice
                resline.CurrentPrice = Mathf.RoundToInt(priceElasticity * basePrice);
            }
        }
    }
    
    // DISPLAY TEXTS

    private string GetCityInventoryDisplayText()
    {
        CityData currentCity = GameManager.Instance.CurrentCity;
        CityInventory currentCityInventory = currentCity.CityInventory;
        
        if (currentCityInventory == null)
        {
            return "No s'ha assignat cap inventari de ciutat.";
        }

        StringBuilder displayText = new StringBuilder();
        displayText.AppendLine($"Inventari de la Ciutat: {currentCity.cityName} (ID: {currentCity.cityID})");
        displayText.AppendLine("Recursos d'Inventari:");
        
        foreach (var resline in currentCityInventory.InventoryResources)
        {
            var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resline.ResourceID);
            int basePrice = matchedResource != null ? matchedResource.BasePrice : 0; // default a zero

            displayText.AppendLine($"{resline.ResourceID}, Type: {resline.ResourceType}, " +
                           $"Qty: {resline.Quantity}, Demands: {resline.DemandConsume} / " +
                           $"{resline.DemandCritical} / {resline.DemandTotal}, " + 
                           $"Current price: {resline.CurrentPrice} (Base price: {basePrice})");
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