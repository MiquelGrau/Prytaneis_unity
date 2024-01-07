using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Inventory
{
    public string InventoryID { get; set; }
    public int InventoryMoney { get; set; }
    public List<InventoryResource> InventoryResources { get; set; }
    public List<InventoryItem> InventoryItems { get; set; }

    public Inventory()
    {
        InventoryResources = new List<InventoryResource>();
        InventoryItems = new List<InventoryItem>();
    }

    public void UpdateOrAddResource(string resourceID, float newQuantity, int newValue)
    {
        // Trobar l'element amb el ResourceID donat
        var inventoryResource = InventoryResources.FirstOrDefault(r => r.ResourceID == resourceID);
        
        if (inventoryResource != null)
        {
            // Actualitzar la quantitat i el valor
            inventoryResource.Quantity = newQuantity;
            inventoryResource.CurrentValue = newValue;
        }
        else
        {
            // Crear un nou InventoryResource si no existeix
            var matchedResource = DatabaseImporter.resources.FirstOrDefault(r => r.resourceID == resourceID);
            var newResourceType = matchedResource != null ? matchedResource.resourceType : null;

            var newResource = new InventoryResource
            {
                ResourceID = resourceID,
                ResourceType = newResourceType,
                Quantity = newQuantity,
                CurrentValue = newValue
            };
            InventoryResources.Add(newResource);
        }
    }

    

    // Potser voldràs incloure mètodes per a interactuar amb la funció TransferQtyAndValue.
}

public class InventoryResource
{
    public string ResourceID { get; set; }
    public string ResourceType { get; set; }
    public float Quantity { get; set; }
    public int CurrentValue { get; set; }

    // Constructor bàsic i altres mètodes comuns aquí
    public static void TransferQtyAndValue(
        string resourceID,
        Inventory originInventory,  // Asumint que originInventory és una instància d'una classe d'inventari
        float quantityOrigin,
        int valueOrigin,
        Inventory destinationInventory,  // Asumint que destinationInventory és una instància d'una classe d'inventari
        float quantityDestination,
        int valueDestination)   
    {
        // Comprovacions de validesa...

        // Calcular el valor mitjà ponderat
        float totalQuantity = quantityOrigin + quantityDestination;
        
        float weightedValueOrigin = (quantityOrigin / totalQuantity) * valueOrigin;
        float weightedValueDestination = (quantityDestination / totalQuantity) * valueDestination;

        float weightedAvgValue = weightedValueOrigin + weightedValueDestination;

        // Actualitzar la quantitat i el valor en l'inventari de destinació
        destinationInventory.UpdateOrAddResource(resourceID, totalQuantity, (int)weightedAvgValue); // Cast a int, si és necessari
    }
}

public class InventoryItem
{
    public string ItemID { get; set; }
    public int CurrentValue { get; set; }

    
}

