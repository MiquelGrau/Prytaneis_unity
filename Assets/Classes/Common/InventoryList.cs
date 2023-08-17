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
        this.inventoryitems = new List<InventoryItem>();
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

[System.Serializable]
public class InventoryListString
{
    public string inventoryID;
    public List<InventoryItemString> inventoryitems;
}

[System.Serializable]
public class InventoryItemString
{
    public string resourceID;
    public string quantity;
    public string currentPrice;
}

