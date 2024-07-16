using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class CityInventory
{
    public string CityInvID { get; set; }
    public string CityID { get; set; }  
    public int CityInvMoney { get; set; }
    public List<CityInventoryResource> InventoryResources { get; set; } // Els recursos en sí
    public List<PopulationDemand> PopDemands { get; set; } = new List<PopulationDemand>();  // Demandes de població
    public List<BuildingDemand> BuildingDemands { get; set; } = new List<BuildingDemand>(); // Demandes de edificis
    
    // Constructor
    public CityInventory(string cityInvID, string cityID, int cityInvMoney, List<CityInventoryResource> resources)
    {
        CityInvID = cityInvID;
        CityID = cityID;
        CityInvMoney = cityInvMoney;
        InventoryResources = resources ?? new List<CityInventoryResource>(); 
    }
    

    
    
}



[System.Serializable]
public class CityInventoryResource : InventoryResource // El mateix que el Resource basic, pero amb les demandes i preu
{
    //public string ResourceID { get; set; }
    //public string ResourceType { get; set; }
    //public float Quantity { get; set; }
    //public int CurrentValue { get; set; }
    public float DemandConsume { get; set; }
    public float DemandCritical { get; set; }
    public float DemandTotal { get; set; }
    public int CurrentPrice { get; set; }
    public int PositionPoor { get; set; }
    public int PositionMid { get; set; }
    public int PositionRich { get; set; }
    
    // Constructors

    // Nou item, normal
    [JsonConstructor]
    public CityInventoryResource(string resourceId, float quantity, int currentValue)
    {
        ResourceID = resourceId;
        Quantity = quantity;
        CurrentValue = currentValue;
        
        // Busca el ResourceType corresponent al ResourceID
        var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resourceId);
        ResourceType = matchedResource != null ? matchedResource.ResourceType : null;


        //ResourceType = null;
        
        // Resta de la inicialització
        DemandConsume = 0;
        DemandCritical = 0;
        DemandTotal = 0;
        CurrentPrice = 0;
        PositionPoor = 0;
        PositionMid = 0;
        PositionRich = 0;
    }

    // Demandes a través de LifestyleTier, crea Resourcetype, nomes la capçalera (header)
    public CityInventoryResource(string resourceType)
    {
        ResourceID = null; // o podríem usar null o una cadena buida
        ResourceType = resourceType;
        Quantity = 0;
        CurrentValue = 0;
        
        // Inicialitzar la resta de propietats a zero o null
        DemandConsume = 0;
        DemandCritical = 0;
        DemandTotal = 0;
        CurrentPrice = 0;
        PositionPoor = 0;
        PositionMid = 0;
        PositionRich = 0;
        
    }
    
}

[System.Serializable]
public class PopulationDemand
{
    public string Class { get; private set; }
    public string ResourceType { get; set; }
    public string DemType { get; set; }
    public int Position { get; set; }
    public string AssignedResID { get; set; }
    public float CoveredQty { get; set; }
    public float ConsumeQty { get; set; }
    public float CritQty { get; set; }  
    public float TotalQty { get; set; }
    public bool Fulfilled { get; set; }


    // Constructor
    public PopulationDemand(string popClass, string resourceType, string demType, int position)
    {
        Class = popClass;
        ResourceType = resourceType;
        DemType = demType;
        Position = position;
        AssignedResID = null;
        ConsumeQty = 0;
        CritQty = 0;
        TotalQty = 0;
        CoveredQty = 0;
        Fulfilled = false;
    }

}

[System.Serializable]
public class BuildingDemand
{
    public string ResType { get; private set; }
    public string AssignedResID { get; set; }
    public string RelatedBuildID { get; set; }
    public float CoveredQty { get; set; }
    public float ConsumeQty { get; set; }
    public float CritQty { get; set; }
    public float TotalQty { get; set; }

    // Constructor únic per ResourceType o ResourceID
    public BuildingDemand(string resTypeOrAssignedResID, string relatedBuildID, float consumeQty, 
                            float critQty, float totalQty, bool isResType = true)
    {
        if (isResType)
        {
            ResType = resTypeOrAssignedResID;
            AssignedResID = null;
        }
        else
        {
            ResType = null;
            AssignedResID = resTypeOrAssignedResID;
        }
        RelatedBuildID = relatedBuildID;
        ConsumeQty = consumeQty;
        CritQty = critQty;
        TotalQty = totalQty;
        CoveredQty = 0; // Inicialment zero
    }
}

