using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro; 

public class TradeManager : MonoBehaviour
{
    
    public TradeDesk CurrentTrade { get; private set; }

    // Referències als desplegables de la UI
    public TMP_Dropdown cityDropdown;
    public TMP_Dropdown agentDropdown;
    public DataManager dataManager;
    public TradeInterface tradeInterface;
    public CityInterface cityInterface;

    // Referències als inventaris actuals seleccionats
    public CityInventory currentCityInventory;
    public AgentInventory currentAgentInventory;
    
    void Start()
    {   
        CurrentTrade = new TradeDesk();

        // Omplir els desplegables
        FillCityDropdown();
        FillAgentDropdown();

        // Afegir escoltadors d'esdeveniments als desplegables
        cityDropdown.onValueChanged.AddListener(OnCitySelected);
        agentDropdown.onValueChanged.AddListener(OnAgentSelected);

        /* Button tradeCompleteButton = tradeInterface.tradeCompleteButton;
        tradeCompleteButton.onClick.AddListener(FinalizeTrade); */
    }

    public class TradeDesk  // La classe amb "tota la negociació". No nomes les linies, sino diners totals i info de les parts. 
    {
        public string TradeID;
        public string TradePartnerLeft;
        public string TradePartnerRight;
        public float LeftMoneyStart;
        public float LeftMoneyMid;
        public float LeftMoneyEnd;
        public float LeftWaresStart;
        public float LeftWaresMid;
        public float LeftWaresEnd;
        public float RightMoneyStart;
        public float RightMoneyMid;
        public float RightMoneyEnd;
        public float RightWaresStart;
        public float RightWaresMid;
        public float RightWaresEnd;
        /* public int MoneyLeft;
        public int MoneyRight;
        public int MoneyToBuy;
        public int MoneyToSell; */
        public List<TradeResourceLine> TradeResourceLines = new List<TradeResourceLine>();
        public List<TradeResourceType> TradeResourceTypes = new List<TradeResourceType>();

    }

    public class TradeResourceLine
    {
        public string ResourceID;
        public string ResourceType;
        public int QtyDemandedLeft;     // Demanda total
        public int QtyAvailableLeft;    // Qty menys Demanda critica
        public int QtyCurrentLeft;
        public int QtyOriginalLeft;
        public int BuyPriceCurrent;
        public int BuyPriceOriginal;    
        public int SellPriceCurrent;
        public int SellPriceOriginal;
        public int QtyCurrentRight;
        public int QtyOriginalRight;
        public int ValueCurrentRight;
        public int ValueOriginalRight;
        public float ToTradeQty;
        public float ToTradeMoney;

        // Constructor i mètodes específics de TradeResourceLine
    }

    public class TradeResourceType  // Les agrupacions que farà dels recursos, també demandes. Una suma, basicament. 
    {
        public string ResourceType;
        public int TQtyCurrentLeft;
        public int TQtyOriginalLeft;
        public int TQtyDemandLeft;
        public float TQtyToTrade;
        public int TQtyCurrentRight;
        public int TQtyOriginalRight;

        // Constructor i mètodes específics de TradeResourceType
    }
    
    // Dropdowns
    // Usats per seleccionar rapidament diferents agents. 
    // Al tanto que ara és "global", farà teleport de la gent si se seleccionen. 
    /* void FillCityDropdown()
    {
        var cityNames = dataManager.GetCities().Select(city => city.Name).ToList();
        cityDropdown.ClearOptions();
        cityDropdown.AddOptions(cityNames);

        // Preselecciona la ciutat actual
        var currentCityIndex = cityNames.FindIndex(name => name == GameManager.Instance.currentCity.Name);
        if (currentCityIndex >= 0)
        {
            cityDropdown.SetValueWithoutNotify(currentCityIndex);
            AssignCityInTrade();
            TradeDeskCleanup(); 
            tradeInterface.UpdateTradeInterface();
        }
    } */

    /* public void OnCitySelected(int index)
    {
        var cities = dataManager.GetCities(); // Obtenint la llista des de DataManager
        if (index < 0 || index >= cities.Count)
        {
            Debug.LogError("Índex de ciutat seleccionada fora de rang.");
            return;
        }
        
        CityData selectedCity = cities[index];
        GameManager.Instance.AssignCurrentCity(selectedCity.LocID);
        
        AssignCityInTrade();
        TradeDeskCleanup(); 
        tradeInterface.UpdateTradeInterface();
        cityDropdown.Hide();
    } */

    void FillCityDropdown()
    {
        // Combinem ciutats i settlements amb els seus respectius prefixos
        var locationNames = new List<string>();

        var cities = DataManager.Instance.allCityList;
        var settlements = DataManager.Instance.allSettlementList;

        foreach (var city in cities)
        {
            locationNames.Add("C- " + city.Name);  // Afegir el prefix "C-" per a ciutats
        }

        foreach (var settlement in settlements)
        {
            locationNames.Add("S- " + settlement.Name);  // Afegir el prefix "S-" per a settlements
        }

        cityDropdown.ClearOptions();
        cityDropdown.AddOptions(locationNames);

        // Preselecciona la ciutat actual
        var currentCityIndex = locationNames.FindIndex(name => name.Contains(GameManager.Instance.currentLocation.Name));
        if (currentCityIndex >= 0)
        {
            cityDropdown.SetValueWithoutNotify(currentCityIndex);
            AssignCityInTrade();
            TradeDeskCleanup();
            tradeInterface.UpdateTradeInterface();
        }
    }

    

    public void OnCitySelected(int index)
    {
        var cities = DataManager.Instance.allCityList;
        var settlements = DataManager.Instance.allSettlementList;

        int totalCities = cities.Count;
        
        if (index < 0 || index >= (totalCities + settlements.Count))
        {
            Debug.LogError("Índex de localització seleccionada fora de rang.");
            return;
        }

        if (index < totalCities)
        {
            // S'ha seleccionat una ciutat
            CityData selectedCity = cities[index];
            GameManager.Instance.AssignCurrentLocation(selectedCity.LocID);
        }
        else
        {
            // S'ha seleccionat un settlement
            Settlement selectedSettlement = settlements[index - totalCities]; // L'index dels settlements ve després de les ciutats
            GameManager.Instance.AssignCurrentLocation(selectedSettlement.LocID); // Tractar el settlement com si fos una ciutat en aquest cas
        }
        
        // No cal que tinguin una ciutat de input, ja treballen amb la currentcity que ha canviat
        AssignCityInTrade();
        TradeDeskCleanup();
        //tradeInterface.UpdateTradeInterface();
        cityInterface.UpdateBuildingGridForCity();
        cityDropdown.Hide();
    }


    void FillAgentDropdown()
    {
        var agentNames = dataManager.GetAgents().Select(agent => agent.agentName).ToList();
        agentDropdown.ClearOptions();
        agentDropdown.AddOptions(agentNames);

        // Preselecciona l'agent actual
        var currentAgentIndex = agentNames.FindIndex(name => name == GameManager.Instance.currentAgent.agentName);
        if (currentAgentIndex >= 0)
        {
            agentDropdown.SetValueWithoutNotify(currentAgentIndex);
            AssignAgentInTrade();
            TradeDeskCleanup(); 
            tradeInterface.UpdateTradeInterface();
        }
    }
    public void OnAgentSelected(int index)
    {
        var agents = dataManager.GetAgents(); // Obtenint la llista des de DataManager
        if (index < 0 || index >= agents.Count)
        {
            Debug.LogError("Índex d'agent seleccionat fora de rang.");
            return;
        }
        Agent selectedAgent = agents[index];
        GameManager.Instance.AssignCurrentAgent(selectedAgent.agentID); 
        Debug.Log($"Nou agent seleccionat: {selectedAgent.agentName}");
        AssignAgentInTrade();
        TradeDeskCleanup(); 
        tradeInterface.UpdateTradeInterface();
        agentDropdown.Hide();
    }
    
    public void AssignCityInTrade()
    {
        currentCityInventory = DataManager.Instance.GetLocInvByID(GameManager.Instance.currentLocation.InventoryID);
        if (currentCityInventory == null) return;
        
        CurrentTrade.TradePartnerLeft = currentCityInventory.CityID;
        CurrentTrade.LeftMoneyStart = currentCityInventory.CityInvMoney;
        CurrentTrade.LeftWaresStart = currentCityInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);
                
        SetUpTradeLines();
    }

    public void AssignLocationInTrade()
    {
        // Obtenim la Location actual (sigui CityData o Settlement)
        var currentLocation = GameManager.Instance.currentLocation;  // Pot ser CityData o Settlement

        // Obtenim l'inventari associat a aquesta localització
        currentCityInventory = DataManager.Instance.GetLocInvByID(currentLocation.InventoryID);
        if (currentCityInventory == null) return;
        
        // Assignar dades al TradeDesk
        CurrentTrade.TradePartnerLeft = currentLocation.LocID;
        CurrentTrade.LeftMoneyStart = currentCityInventory.CityInvMoney;
        CurrentTrade.LeftWaresStart = currentCityInventory.InventoryResources
            .Where(r => !string.IsNullOrEmpty(r.ResourceType))
            .Sum(r => (int)r.Quantity);

        SetUpTradeLines();
    }


    public void AssignAgentInTrade()
    {
        // Crida les dades, guardades a game manager
        Agent currentAgent = GameManager.Instance.currentAgent;
        currentAgentInventory = currentAgent?.Inventory;
        if (currentAgentInventory == null)
        {
            Debug.LogError("No s'ha trobat l'inventari de l'agent actual.");
            return;
        }

        // Assigna les propietats bàsiques de l'agent a CurrentTrade
        CurrentTrade.TradePartnerRight = currentAgentInventory.AgentID;
        CurrentTrade.RightMoneyStart = currentAgentInventory.InventoryMoney;
        CurrentTrade.RightWaresStart = currentAgentInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);
                
        SetUpTradeLines();
    }

    public void TradeDeskCleanup()
    {
        Debug.Log($"Netejant el Trade Desk");
        if (CurrentTrade != null)
        {
            // Reset de les propietats
            if (currentCityInventory != null && currentAgentInventory != null)
            {
                CurrentTrade.LeftMoneyStart = currentCityInventory.CityInvMoney;
                CurrentTrade.LeftWaresStart = currentCityInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);
                CurrentTrade.RightMoneyStart = currentAgentInventory.InventoryMoney;
                CurrentTrade.RightWaresStart = currentAgentInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);

                // Reseteja els valors mid a zero
                CurrentTrade.LeftMoneyMid = 0;
                CurrentTrade.LeftWaresMid = 0;
                CurrentTrade.RightMoneyMid = 0;
                CurrentTrade.RightWaresMid = 0;

                // Assigna els valors end igual als start
                CurrentTrade.LeftMoneyEnd = CurrentTrade.LeftMoneyStart;
                CurrentTrade.LeftWaresEnd = CurrentTrade.LeftWaresStart;
                CurrentTrade.RightMoneyEnd = CurrentTrade.RightMoneyStart;
                CurrentTrade.RightWaresEnd = CurrentTrade.RightWaresStart;
            }

            // Elimina les línies amb quantitats a zero en ambdues bandes, linies sobrants
            CurrentTrade.TradeResourceLines.RemoveAll(line => line.QtyOriginalLeft <= 0 && line.QtyOriginalRight <= 0);

            foreach (var line in CurrentTrade.TradeResourceLines)
            {
                // Actualitza les quantitats
                line.QtyCurrentLeft = line.QtyOriginalLeft;
                line.QtyCurrentRight = line.QtyOriginalRight;
                line.ValueCurrentRight = line.ValueOriginalRight;
                line.ToTradeQty = 0;
                line.ToTradeMoney = 0;

                // Si QtyOriginalLeft és major que zero, reseteja els preus
                if (line.QtyOriginalLeft > 0)
                {
                    line.BuyPriceCurrent = line.BuyPriceOriginal;
                    line.SellPriceOriginal = (int)(line.BuyPriceOriginal * 0.90);
                    line.SellPriceCurrent = line.SellPriceOriginal;
                }
                
            }
            // Ordena les TradeResourceLines per ResourceID
            CurrentTrade.TradeResourceLines = CurrentTrade.TradeResourceLines.OrderBy(line => line.ResourceID).ToList();
        }
    }
    
    // Aquesta funció centralitza la configuració de les línies de recurs per a la negociació.
    private void SetUpTradeLines()
    {
        // Obtenir les referències a les dades de la ciutat i de l'agent des de GameManager
        Location currentLocation = GameManager.Instance.currentLocation;
        currentCityInventory = DataManager.Instance.GetLocInvByID(currentLocation.InventoryID);
        Agent currentAgent = GameManager.Instance.currentAgent;
        AgentInventory agentInventory = currentAgent.Inventory;
        
        
        // Aquest diccionari temporal ajudarà a gestionar les TradeResourceLines de manera eficient
        Dictionary<string, TradeResourceLine> tempTradeLines = new Dictionary<string, TradeResourceLine>();

        // Processar els recursos de la ciutat
        foreach (var resource in currentCityInventory.InventoryResources)
        {
            // Comprova que `ResourceID` no sigui `null` abans d'afegir-lo al diccionari
            if (!string.IsNullOrWhiteSpace(resource.ResourceID))
            {
                if (!tempTradeLines.TryGetValue(resource.ResourceID, out var line))
                {
                    line = new TradeResourceLine
                    {
                        ResourceID = resource.ResourceID,
                        ResourceType = resource.ResourceType,
                        QtyOriginalLeft = (int)resource.Quantity,
                        QtyDemandedLeft = (int)((CityInventoryResource)resource).DemandTotal, // Cast a CityInventoryResource per accedir a DemandTotal
                        QtyAvailableLeft = (int)Mathf.Max(0, resource.Quantity - ((CityInventoryResource)resource).DemandCritical),
                        BuyPriceOriginal = ((CityInventoryResource)resource).CurrentPrice, // Usa el CurrentPrice com a BuyPriceOriginal
                        SellPriceOriginal = (int)(((CityInventoryResource)resource).CurrentPrice * 0.9f) // Calcula SellPriceOriginal amb un descompte
                    };
                    tempTradeLines[resource.ResourceID] = line;
                }
                else
                {
                    // Actualitza les propietats de la línia si ja existia
                    line.QtyOriginalLeft += (int)resource.Quantity;
                    line.QtyDemandedLeft += (int)((CityInventoryResource)resource).DemandTotal;
                    line.QtyAvailableLeft += (int)Mathf.Max(0, resource.Quantity - ((CityInventoryResource)resource).DemandCritical);
                    line.BuyPriceOriginal = ((CityInventoryResource)resource).CurrentPrice;
                    line.SellPriceOriginal = (int)(((CityInventoryResource)resource).CurrentPrice * 0.9f);
                }
            }
            else
            {
                Debug.LogWarning($"ResourceID buit o nul per un recurs de la ciutat {currentLocation.Name}");
            }
        }

        // Processar els recursos de l'agent
        if(agentInventory != null)
        {
            foreach(var resource in agentInventory.InventoryResources)
            {
                if(!tempTradeLines.TryGetValue(resource.ResourceID, out var line))
                {
                    line = new TradeResourceLine
                    {
                        ResourceID = resource.ResourceID,
                        ResourceType = resource.ResourceType,
                        QtyOriginalRight = (int)resource.Quantity,
                        ValueOriginalRight = resource.CurrentValue
                    };
                    tempTradeLines.Add(resource.ResourceID, line);
                }
                else
                {
                    // Actualitza les propietats de la línia si ja existia
                    line.QtyOriginalRight += (int)resource.Quantity;
                    line.ValueOriginalRight += resource.CurrentValue;
                }
            }
        }
        // Actualitzar les TradeResourceLines de CurrentTrade amb les dades processades
        CurrentTrade.TradeResourceLines = tempTradeLines.Values.ToList();

        // Recorda cridar qualsevol altra funció necessària després de configurar les línies, com ara TradeDeskCleanup
        TradeDeskCleanup();

    }

    

    public void RecalcPriceForResource(TradeResourceLine line)
    {
        Debug.Log($"Recalculant preu per a ID: {line.ResourceID}, {line.ResourceType}");
        // Utilitza directament les propietats de la línia per calcular la diferència respecte a la demanda
        float difQty = line.QtyDemandedLeft == 0 ? 3f : 
            ((float)line.QtyCurrentLeft - (float)line.QtyDemandedLeft) / (float)line.QtyDemandedLeft;
        
        // Calcula la price elasticity segons la fórmula donada: y= -0.6x^3 +0.8x^2 -0.6x +1
        float priceElasticity = -0.6f * Mathf.Pow(difQty, 3) + 0.8f * Mathf.Pow(difQty, 2) - 0.6f * difQty + 1f;

        // Assegura't que la price elasticity no sigui negativa, posa valor mínim, 25%
        priceElasticity = Mathf.Max(priceElasticity, 0.25f);

        // Troba el base price del recurs
        var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == line.ResourceID);
        float basePrice = matchedResource != null ? matchedResource.BasePrice : 0f;

        // Calcula el BuyPriceCurrent i SellPriceCurrent
        int currentPrice = Mathf.RoundToInt(priceElasticity * basePrice);
        line.BuyPriceCurrent = currentPrice;
        line.SellPriceCurrent = Mathf.RoundToInt(currentPrice * 0.90f); // Assumeix un descompte del 10% sobre el preu de compra

        // Afegir el Debug.Log amb la informació recalculada, incloent detalls sobre Qty CurrentLeft i QtyDemandedLeft
        Debug.Log($"Recalculat preu per {matchedResource.ResourceName}, (ID: {line.ResourceID}): "+
            $"Existent {line.QtyCurrentLeft}, Demandat {line.QtyDemandedLeft}, "+
            $"Diferència {difQty}, Elasticity {priceElasticity} x base {basePrice} = total {currentPrice}");
    }
    
    public void BuyResource(string resourceID)
    {
        Debug.Log($"Comprant");
        var resourceLine = CurrentTrade.TradeResourceLines.FirstOrDefault(line => line.ResourceID == resourceID);
        if (resourceLine != null && resourceLine.QtyCurrentLeft > 0)
        {
            // Actualitza les quantitats
            resourceLine.QtyCurrentLeft--;
            CurrentTrade.LeftWaresMid--;
            resourceLine.QtyCurrentRight++;
            CurrentTrade.RightWaresMid++;
            resourceLine.ToTradeQty++;
            
            // Actualitza els diners
            CurrentTrade.LeftMoneyMid += resourceLine.BuyPriceCurrent;
            CurrentTrade.RightMoneyMid -= resourceLine.BuyPriceCurrent;
            resourceLine.ToTradeMoney -= resourceLine.BuyPriceCurrent;
            
            // Recalcular el valor unitari ponderat
            int oldQtyCurrentRight = resourceLine.QtyCurrentRight - 1; // Quantitat abans de la compra actual
            resourceLine.ValueCurrentRight = Mathf.RoundToInt((oldQtyCurrentRight * resourceLine.ValueCurrentRight + resourceLine.BuyPriceCurrent) / resourceLine.QtyCurrentRight);

            // Recalcular preu per aquest recurs
            RecalcPriceForResource(resourceLine);
        }
        CurrentTrade.LeftWaresEnd = CurrentTrade.LeftWaresStart + CurrentTrade.LeftWaresMid;
        CurrentTrade.RightWaresEnd = CurrentTrade.RightWaresStart + CurrentTrade.RightWaresMid;
        CurrentTrade.LeftMoneyEnd = CurrentTrade.LeftMoneyStart + CurrentTrade.LeftMoneyMid;
        CurrentTrade.RightMoneyEnd = CurrentTrade.RightMoneyStart + CurrentTrade.RightMoneyMid;
        
        // Actualitza la UI
        tradeInterface.UpdateTradeInterface();
    }

    public void SellResource(string resourceID)
    {
        Debug.Log($"Venent");
        var resourceLine = CurrentTrade.TradeResourceLines.FirstOrDefault(line => line.ResourceID == resourceID);
        if (resourceLine != null && resourceLine.QtyCurrentRight > 0)
        {
            // Actualitza les quantitats
            resourceLine.QtyCurrentRight--;
            CurrentTrade.RightWaresMid--;
            resourceLine.QtyCurrentLeft++;
            CurrentTrade.LeftWaresMid++;
            resourceLine.ToTradeQty--;
            
            // Actualitza els diners
            CurrentTrade.RightMoneyMid += resourceLine.SellPriceCurrent;
            CurrentTrade.LeftMoneyMid -= resourceLine.SellPriceCurrent;
            resourceLine.ToTradeMoney += resourceLine.BuyPriceCurrent;

            // Recalcular preu per aquest recurs
            RecalcPriceForResource(resourceLine);
        }
        CurrentTrade.LeftMoneyEnd = CurrentTrade.LeftMoneyStart + CurrentTrade.LeftMoneyMid;
        CurrentTrade.RightMoneyEnd = CurrentTrade.RightMoneyStart + CurrentTrade.RightMoneyMid;
        CurrentTrade.LeftWaresEnd = CurrentTrade.LeftWaresStart + CurrentTrade.LeftWaresMid;
        CurrentTrade.RightWaresEnd = CurrentTrade.RightWaresStart + CurrentTrade.RightWaresMid;
        
        // Actualitza la UI
        tradeInterface.UpdateTradeInterface();
    }

    private void ResetTradeTotals()
    {
        CurrentTrade.LeftMoneyStart = currentCityInventory.CityInvMoney;
        CurrentTrade.LeftWaresStart = currentCityInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);
        CurrentTrade.RightMoneyStart = currentAgentInventory.InventoryMoney;
        CurrentTrade.RightWaresStart = currentAgentInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);
        
        CurrentTrade.LeftMoneyMid = 0;
        CurrentTrade.RightMoneyMid = 0;
        CurrentTrade.LeftWaresMid = 0;
        CurrentTrade.RightWaresMid = 0;
        
        CurrentTrade.LeftMoneyEnd = CurrentTrade.LeftMoneyStart;
        CurrentTrade.RightMoneyEnd = CurrentTrade.RightMoneyStart;
        CurrentTrade.LeftWaresEnd = CurrentTrade.LeftWaresStart;
        CurrentTrade.RightWaresEnd = CurrentTrade.RightWaresStart;
    }

    public void FinalizeTrade()
    {
        Debug.Log("Finalitzant la venda...");
        foreach (var resourceLine in CurrentTrade.TradeResourceLines)
        {
            if (resourceLine.ToTradeQty != 0)
            {   
                float unitPrice = resourceLine.ToTradeMoney / resourceLine.ToTradeQty;

                // Si ToTradeQty és positiu, significa compra (city ven a l'agent)
                if (resourceLine.ToTradeQty > 0)
                {
                    // Restar de l'inventari de la ciutat
                    var cityResource = currentCityInventory.InventoryResources.FirstOrDefault(r => r.ResourceID == resourceLine.ResourceID);
                    if (cityResource != null)
                    {
                        cityResource.Quantity -= resourceLine.ToTradeQty;
                    }

                    // Sumar a l'inventari de l'agent
                    var agentResource = currentAgentInventory.InventoryResources.FirstOrDefault(r => r.ResourceID == resourceLine.ResourceID);
                    if (agentResource != null)
                    {
                        int oldQty = (int)agentResource.Quantity;
                        agentResource.Quantity += resourceLine.ToTradeQty;
                        agentResource.CurrentValue = Mathf.RoundToInt((oldQty * agentResource.CurrentValue + resourceLine.ToTradeQty * unitPrice) / (oldQty + resourceLine.ToTradeQty));
                    }
                    else
                    {
                        currentAgentInventory.InventoryResources.Add(new InventoryResource
                        {
                            ResourceID = resourceLine.ResourceID,
                            ResourceType = resourceLine.ResourceType,
                            Quantity = resourceLine.ToTradeQty,
                            CurrentValue = Mathf.RoundToInt(unitPrice)
                        });
                    }
                }
                // Si ToTradeQty és negatiu, significa venda (agent ven a la ciutat)
                else if (resourceLine.ToTradeQty < 0)
                {
                    // Restar de l'inventari de l'agent
                    var agentResource = currentAgentInventory.InventoryResources.FirstOrDefault(r => r.ResourceID == resourceLine.ResourceID);
                    if (agentResource != null)
                    {
                        agentResource.Quantity += resourceLine.ToTradeQty; // Aquí ToTradeQty és negatiu
                    }

                    // Sumar a l'inventari de la ciutat
                    var cityResource = currentCityInventory.InventoryResources.FirstOrDefault(r => r.ResourceID == resourceLine.ResourceID);
                    if (cityResource != null)
                    {
                        int oldQty = (int)cityResource.Quantity;
                        cityResource.Quantity -= resourceLine.ToTradeQty; // Aquí ToTradeQty és negatiu, així que es suma
                        cityResource.CurrentValue = Mathf.RoundToInt((oldQty * cityResource.CurrentValue - resourceLine.ToTradeQty * unitPrice) / (oldQty - resourceLine.ToTradeQty));
                    }
                    
                    else
                    {
                        currentCityInventory.InventoryResources.Add(new CityInventoryResource
                        (
                            resourceLine.ResourceID,
                            -resourceLine.ToTradeQty,
                            Mathf.RoundToInt(unitPrice)
                        ));
                        
                    }
                }
            }
        }

        // Transferir els diners
        currentCityInventory.CityInvMoney += Mathf.RoundToInt(CurrentTrade.LeftMoneyMid);
        currentAgentInventory.InventoryMoney += Mathf.RoundToInt(CurrentTrade.RightMoneyMid);

        // Reiniciar els valors de la taula de comerç
        TradeDeskCleanup();
        SetUpTradeLines();

        // Actualitzar la UI
        tradeInterface.UpdateTradeInterface();
        Debug.Log("Venda finalitzada.");
    }

}
