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

    // Referències als inventaris actuals seleccionats
    public CityInventory currentCityInventory;
    public AgentInventory currentAgentInventory;
    
    // Referències a les llistes de ciutats i agents
    public List<CityData> cities;
    public List<Agent> agents;

    void Start()
    {
        // Trobar ciutats i agents
        cities = dataManager.GetCities(); 
        agents = dataManager.GetAgents(); 
        CurrentTrade = new TradeDesk();

        // Omplir els desplegables
        FillCityDropdown();
        FillAgentDropdown();

        // Afegir escoltadors d'esdeveniments als desplegables
        cityDropdown.onValueChanged.AddListener(OnCitySelected);
        agentDropdown.onValueChanged.AddListener(OnAgentSelected);
    }

    public class TradeDesk
    {
        public string TradeID;
        public string TradePartnerLeft;
        public string TradePartnerRight;
        public int MoneyLeft;
        public int MoneyRight;
        public int MoneyToBuy;
        public int MoneyToSell;
        public List<TradeResourceLine> TradeResourceLines = new List<TradeResourceLine>();
        public List<TradeResourceType> TradeResourceTypes = new List<TradeResourceType>();

        public void TradeDeskCleanup()
        {
            Debug.Log($"Fent neteja de preus: {TradeResourceLines}");
            // Elimina les línies amb quantitats a zero en ambdues bandes, linies sobrants
            TradeResourceLines.RemoveAll(line => line.QtyOriginalLeft <= 0 && line.QtyOriginalRight <= 0);

            foreach (var line in TradeResourceLines)
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
            TradeResourceLines = TradeResourceLines.OrderBy(line => line.ResourceID).ToList();

        }

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

    public class TradeResourceType
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

    
    
    void FillCityDropdown()
    {
        cityDropdown.ClearOptions();
        cityDropdown.AddOptions(cities.Select(city => city.cityName).ToList());
    }
    public void OnCitySelected(int index)
    {
        CityData selectedCity = cities[index];
        currentCityInventory = selectedCity.CityInventory;
        Debug.Log($"Nova ciutat seleccionada: {selectedCity.cityName}");
        
        AssignCityInTrade();
        CurrentTrade.TradeDeskCleanup(); 
        tradeInterface.UpdateTradeInterface();
        cityDropdown.Hide();
    }

    void FillAgentDropdown()
    {
        agentDropdown.ClearOptions();
        agentDropdown.AddOptions(agents.Select(agent => agent.agentName).ToList());
    }
    public void OnAgentSelected(int index)
    {
        Agent selectedAgent = agents[index];
        currentAgentInventory = selectedAgent.Inventory;
        Debug.Log($"Nou agent seleccionat: {selectedAgent.agentName}");
        AssignAgentInTrade();
        CurrentTrade.TradeDeskCleanup(); 
        tradeInterface.UpdateTradeInterface();
        agentDropdown.Hide();
    }

    public void AssignCityInTrade()
    {
        if (currentCityInventory == null) return;

        CurrentTrade.TradePartnerLeft = currentCityInventory.CityID;
        CurrentTrade.MoneyLeft = currentCityInventory.CityInvMoney;
        
        /* Debug.Log($"Assignat inventari de la ciutat {currentCityInventory.CityID}, "+
                  $"té {currentCityInventory.CityInvMoney} diners i conté aquests recursos:"); */

        // Reiniciar valors específics de la part esquerra
        foreach (var line in CurrentTrade.TradeResourceLines)
        {
            line.QtyDemandedLeft = 0;
            line.QtyAvailableLeft = 0;
            line.QtyCurrentLeft = 0;
            line.QtyOriginalLeft = 0;
            line.BuyPriceCurrent = 0;
            line.BuyPriceOriginal = 0;
            line.SellPriceCurrent = 0;
            line.SellPriceOriginal = 0;
        }

        // Afegir o actualitzar recursos de la ciutat
        foreach (var resource in currentCityInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceID)))
        {
            var line = CurrentTrade.TradeResourceLines.FirstOrDefault(trl => trl.ResourceID == resource.ResourceID);
            if (line == null)
            {
                line = new TradeResourceLine
                {
                    ResourceID = resource.ResourceID,
                    ResourceType = resource.ResourceType,
                    QtyDemandedLeft = (int)resource.DemandTotal,
                    QtyAvailableLeft = (int)Mathf.Max(0, resource.Quantity - resource.DemandCritical),
                    QtyOriginalLeft = (int)resource.Quantity,
                    BuyPriceOriginal = resource.CurrentPrice,
                    
                };
                CurrentTrade.TradeResourceLines.Add(line);
            }
            else
            {
                // Actualitza les propietats si el recurs ja existeix
                line.QtyOriginalLeft = (int)resource.Quantity;
                line.BuyPriceOriginal = resource.CurrentPrice;
                line.QtyDemandedLeft = (int)resource.DemandTotal;
                line.QtyAvailableLeft = (int)Mathf.Max(0, resource.Quantity - resource.DemandCritical);
            }
            
        }
        
        // Log amb informació detallada per a cada TradeResourceLine
        /* foreach (var line in CurrentTrade.TradeResourceLines)
        {
            Debug.Log($"ResourceID: {line.ResourceID}, ResourceType: {line.ResourceType}, " +
                      $"QtyOriginalLeft: {line.QtyOriginalLeft}, QtyOriginalRight: {line.QtyOriginalRight}, " +
                      $"BuyPriceOriginal: {line.BuyPriceOriginal}, ValueOriginalRight: {line.ValueOriginalRight}");
        } */
    }

    public void AssignAgentInTrade()
    {
        if (currentAgentInventory == null) return;

        // Assigna les propietats bàsiques de l'agent a CurrentTrade
        CurrentTrade.TradePartnerRight = currentAgentInventory.AgentID;
        CurrentTrade.MoneyRight = currentAgentInventory.InventoryMoney;
        
        // Reiniciar valors específics de la part dreta
        foreach (var line in CurrentTrade.TradeResourceLines)
        {
            line.QtyCurrentRight = 0;
            line.QtyOriginalRight = 0;
            line.ValueCurrentRight = 0;
            line.ValueOriginalRight = 0;
        }

        // Afegir o actualitzar recursos de l'agent
        foreach (var resource in currentAgentInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceID)))
        {
            var line = CurrentTrade.TradeResourceLines.FirstOrDefault(trl => trl.ResourceID == resource.ResourceID);
            var matchedResource = DataManager.resources.FirstOrDefault(r => r.resourceID == resource.ResourceID);
            if (line == null)
            {
                line = new TradeResourceLine
                {
                    ResourceID = resource.ResourceID,
                    //ResourceType = resource.ResourceType,
                    ResourceType = matchedResource != null ? matchedResource.resourceType : "Desconegut",
                    QtyOriginalRight = (int)resource.Quantity,
                    ValueOriginalRight = resource.CurrentValue
                };
                CurrentTrade.TradeResourceLines.Add(line);
            }
            else
            {
                // Actualitza les propietats si el recurs ja existeix
                line.ResourceType = matchedResource != null ? matchedResource.resourceType : "Desconegut"; // no caldria, pero bueno
                line.QtyOriginalRight = (int)resource.Quantity;
                line.ValueOriginalRight = resource.CurrentValue;
            }

            // Buscar la demanda de la ciutat per aquests recursos (només si QtyOriginalLeft és 0)
            if (line.QtyOriginalLeft == 0)
            {
                var headerResource = currentCityInventory.InventoryResources
                    .FirstOrDefault(ri => ri.ResourceType == line.ResourceType && ri.ResourceID == null);

                if (headerResource != null)
                {
                    line.QtyDemandedLeft = (int)headerResource.DemandTotal;
                    line.QtyAvailableLeft = 0;
                }
                RecalcPriceForResource(line);
            }

            
        }
        
        // Log amb informació detallada per a cada TradeResourceLine
        /* foreach (var line in CurrentTrade.TradeResourceLines)
        {
            Debug.Log($"ResourceID: {line.ResourceID}, ResourceType: {line.ResourceType}, " +
                      $"QtyOriginalLeft: {line.QtyOriginalLeft}, QtyOriginalRight: {line.QtyOriginalRight}, " +
                      $"BuyPriceOriginal: {line.BuyPriceOriginal}, ValueOriginalRight: {line.ValueOriginalRight}");
        } */
    }

    public void RecalcPriceForResource(TradeResourceLine line)
    {
        Debug.Log($"Recalculant preu per a ID: {line.ResourceID}, {line.ResourceType}");
        // Utilitza directament les propietats de la línia per calcular la diferència respecte a la demanda
        float difQty = line.QtyDemandedLeft == 0 ? 3f : (line.QtyCurrentLeft - line.QtyDemandedLeft) / line.QtyDemandedLeft;

        // Calcula la price elasticity segons la fórmula donada: y= -0.6x^3 +0.8x^2 -0.6x +1
        float priceElasticity = -0.6f * Mathf.Pow(difQty, 3) + 0.8f * Mathf.Pow(difQty, 2) - 0.6f * difQty + 1f;

        // Assegura't que la price elasticity no sigui negativa, posa valor mínim, 25%
        priceElasticity = Mathf.Max(priceElasticity, 0.25f);

        // Troba el base price del recurs
        var matchedResource = DataManager.resources.FirstOrDefault(r => r.resourceID == line.ResourceID);
        float basePrice = matchedResource != null ? matchedResource.basePrice : 0f;

        // Calcula el BuyPriceCurrent i SellPriceCurrent
        int currentPrice = Mathf.RoundToInt(priceElasticity * basePrice);
        line.BuyPriceCurrent = currentPrice;
        line.SellPriceCurrent = Mathf.RoundToInt(currentPrice * 0.90f); // Assumeix un descompte del 10% sobre el preu de compra

    }
    
    public void BuyResource(string resourceID)
    {
        var resourceLine = CurrentTrade.TradeResourceLines.FirstOrDefault(line => line.ResourceID == resourceID);
        if (resourceLine != null && resourceLine.QtyCurrentLeft > 0)
        {
            // Actualitza les quantitats
            resourceLine.QtyCurrentLeft--;
            resourceLine.QtyCurrentRight++;
            resourceLine.ToTradeQty++;
            
            // Actualitza els diners
            CurrentTrade.MoneyLeft += resourceLine.BuyPriceCurrent;
            CurrentTrade.MoneyRight -= resourceLine.BuyPriceCurrent;
            resourceLine.ToTradeMoney -= resourceLine.BuyPriceCurrent;

            // Recalcular preu per aquest recurs
            RecalcPriceForResource(resourceLine);
        }

        // Actualitza la UI
        tradeInterface.UpdateTradeInterface();
    }

    public void SellResource(string resourceID)
    {
        var resourceLine = CurrentTrade.TradeResourceLines.FirstOrDefault(line => line.ResourceID == resourceID);
        if (resourceLine != null && resourceLine.QtyCurrentRight > 0)
        {
            // Actualitza les quantitats
            resourceLine.QtyCurrentRight--;
            resourceLine.QtyCurrentLeft++;
            resourceLine.ToTradeQty--;
            
            // Actualitza els diners
            CurrentTrade.MoneyRight += resourceLine.SellPriceCurrent;
            CurrentTrade.MoneyLeft -= resourceLine.SellPriceCurrent;
            resourceLine.ToTradeMoney += resourceLine.BuyPriceCurrent;

            // Recalcular preu per aquest recurs
            RecalcPriceForResource(resourceLine);
        }

        // Actualitza la UI
        tradeInterface.UpdateTradeInterface();
    }

}