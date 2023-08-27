using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CityInventoryList
{
    public int cityInventoryID;
    public List<CityInventoryItem> cityInventoryItems;

    public CityInventoryList(int cityInventoryID)
    {
        this.cityInventoryID = cityInventoryID;
        this.cityInventoryItems = new List<CityInventoryItem>();
    }
    public override string ToString()
    {
        return $"Inventory ID: {cityInventoryID}\nItems:\n{string.Join("\n", cityInventoryItems.Select(item => item.ToString()))}";
    }

    public class CityInventoryItem
    {
        public int resourceID;
        public string resourceType; // per assignar-lo, realment. Ja sabem qeu sempre està dins d'una altra
        public int variety; // per assignar, el farà pels més grans
        public int quantity;
        public int currentPrice;
        public int demandTotal;
        public int demandCritical;
        public int buyPrice;
        public int sellPrice;

        public CityInventoryItem(int resourceID, string resourceType, int variety, int quantity, int currentPrice, int demandTotal, int demandCritical, int buyPrice, int sellPrice)
        {
            this.resourceID = resourceID;
            this.resourceType = resourceType;
            this.variety = variety;
            this.quantity = quantity;
            this.currentPrice = currentPrice;
            this.demandTotal = demandTotal;
            this.demandCritical = demandCritical;
            this.buyPrice = buyPrice;
            this.sellPrice = sellPrice;


            
        }
        
        public override string ToString()
        {
            return $"Resource ID: {resourceID}, Type: {resourceType}, Quantity: {quantity}, Current Price: {currentPrice}";
        }
        
    }
}

[System.Serializable]
public class CityInventoryImport    // per importar el fitxer de json
{
    public List<CityInventoryListImport> inventory_jsonfile;
}
[System.Serializable]
public class CityInventoryListImport
{
    public string inventoryID;
    public List<CityInventoryItemImport> inventoryitems;
    
}
[System.Serializable]
public class CityInventoryItemImport
{
    public string resourceID;
    public string quantity;
    public string currentPrice;
    
}










