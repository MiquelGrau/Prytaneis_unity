using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
