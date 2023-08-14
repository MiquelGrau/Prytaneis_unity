using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryList
{
    public int inventoryID;
    public List<InventoryItem> inventoryitems; // No la diem Item directament per no barrejar amb els objectes

    public InventoryList(int inventoryID)
    {
        this.inventoryID = inventoryID;
    }

    public class InventoryItem
    {
        public int resourceID;
        public int quantity;
        public int currentPrice;

        public InventoryItem(int resourceID, int quantity, int currentPrice)
        {
            this.resourceID = resourceID;
            this.quantity = quantity;
            this.currentPrice = currentPrice;
        }
    }

    public static InventoryList LoadFromJSON(TextAsset jsonFile)
    {
        return JsonUtility.FromJson<InventoryList>(jsonFile.text);
    }

}

