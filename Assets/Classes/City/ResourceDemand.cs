using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ResourceDemand
{
    public string resourceType; // null si es un recurs individual
    public int resourceID; // -1 si es un resourceType
    public int demandQuantity;
    public int variety;

    // Constructor per demandes de resourceType
    public ResourceDemand(string resourceType, int demandQuantity, int variety)
    {
        this.resourceType = resourceType;
        this.resourceID = -1;
        this.demandQuantity = demandQuantity;
        this.variety = variety;
    }

    // Constructor per demandes de recursos individuals
    public ResourceDemand(int resourceID, int demandQuantity)
    {
        this.resourceType = null;
        this.resourceID = resourceID;
        this.demandQuantity = demandQuantity;
        this.variety = 1; // els recursos individuals sempre tindran una varietat
    }
}


public class ResourceDemandList
{
    public List<ResourceDemand> demands;

    public ResourceDemandList()
    {
        this.demands = new List<ResourceDemand>();
    }

    // Aquest mètode afegeix demandes de resourceType
    public void AddTypeDemand(string resourceType, int demandQuantity, int variety)
    {
        demands.Add(new ResourceDemand(resourceType, demandQuantity, variety));
    }

    // Aquest mètode afegeix demandes de recursos individuals
    public void AddResourceDemand(int resourceID, int demandQuantity)
    {
        demands.Add(new ResourceDemand(resourceID, demandQuantity));
    }
}

public class CityResourceConsumer
{
    public CityData city;
    public CityInventoryList cityInventory;
    public ResourceDemandList demandList;
    public List<Resource> resources;

    public CityResourceConsumer(CityData city, CityInventoryList cityInventory, ResourceDemandList demandList, List<Resource> resources)
    {
        this.city = city;
        this.cityInventory = cityInventory;
        this.demandList = demandList;
        this.resources = resources;
    }
    public void ConsumeResources()
    {
        foreach (ResourceDemand demand in demandList.demands)
        {
            int demandPerVariety = demand.demandQuantity / demand.variety;

            // Si es un resourceType
            if (demand.resourceType != null)
            {
                var relatedResources = resources.Where(r => r.resourceType == demand.resourceType).Select(r => r.resourceID).ToList();
                var relatedInventoryItems = cityInventory.cityInventoryItems.Where(item => relatedResources.Contains(item.resourceID)).OrderByDescending(item => item.quantity).ToList();

                for (int i = 0; i < demand.variety; i++)
                {
                    int demandRemaining = demandPerVariety;

                    foreach (var inventoryItem in relatedInventoryItems)
                    {
                        if (demandRemaining <= 0)
                        {
                            break;
                        }

                        int consumed = Mathf.Min(inventoryItem.quantity, demandRemaining);
                        inventoryItem.quantity -= consumed;
                        demandRemaining -= consumed;
                    }

                    relatedInventoryItems = relatedInventoryItems.Skip(1).ToList();
                }
            }

            // Si es un recurs individual
            else if (demand.resourceID != -1)
            {
                var inventoryItem = cityInventory.cityInventoryItems.FirstOrDefault(item => item.resourceID == demand.resourceID);
                if (inventoryItem != null)
                {
                    int consumed = Mathf.Min(inventoryItem.quantity, demandPerVariety);
                    inventoryItem.quantity -= consumed;
                }
            }
        }
    }
}
