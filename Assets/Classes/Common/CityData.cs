using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CityData
{
    public int cityID;
    public string cityName;
    public float latitude;
    public float longitude;
    public List<InventoryItem> cityInventory = new List<InventoryItem>();
    public int money;

    
}


[System.Serializable]
public class CityDataList
{
    public List<CityData> cities;
}
