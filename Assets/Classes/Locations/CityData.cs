using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class CityData : Location
{
    // Generat a la classe Location
    // public string Name;
    // public string LocID;
    // public string NodeID;
    // public float Latitude;
    // public float Longitude;
    // public List<Building> Buildings { get; set; } = new List<Building>();
    
    
    // Demografia i politica
    public int PoorPopulation { get; set; }
    public int MidPopulation { get; set; }
    public int RichPopulation { get; set; }
    public int Population { get { return PoorPopulation + MidPopulation + RichPopulation; } }
    public string PoorLifestyleID { get; set; }
    public string MidLifestyleID { get; set; }
    public string RichLifestyleID { get; set; } 
    public string OwnerID { get; set; }
    public string PoliticalStatus { get; set; }
    
    // Economic
    public string CityInventoryID { get; set; }
    public string[][] Grid;
    public float BuildPoints { get; set; } 
    
    // Constructor, si venen des del json
    public CityData(string locationID, string name, string nodeID, string cityInventoryID, 
                    int poorPop, int midPop, int richPop, 
                    string poorID, string midID, string richID, 
                    float buildPoints, string politicalStatus, string ownerID )
        : base(name, locationID, nodeID)  // Crida al constructor de Location
    {
    
    
        PoorPopulation = poorPop;
        MidPopulation = midPop;
        RichPopulation = richPop;
        PoorLifestyleID = poorID;
        MidLifestyleID = midID;
        RichLifestyleID = richID;
        OwnerID = ownerID;
        PoliticalStatus = politicalStatus;
        CityInventoryID = cityInventoryID;
        BuildPoints = buildPoints;
        
        Latitude = 0f;
        Longitude = 0f;
        Buildings = new List<Building>();
        Grid = null;

        
    }

}

[System.Serializable]
public class CityInventory
{
    public string CityInvID { get; set; }
    public string CityID { get; set; }  
    public int CityInvMoney { get; set; }
    public List<CityInventoryResource> InventoryResources { get; set; } // Wares: population + building dems
    public List<PopulationDemand> PopDemands { get; set; } = new List<PopulationDemand>();  // Demandes de recursos, població
    public List<BuildingDemand> BuildingDemands { get; set; } = new List<BuildingDemand>(); // Demandes de recursos, edificis
    public List<CityService> Services { get; set; } = new List<CityService>(); // Services
    
    
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

[System.Serializable]
public class CityService : InventoryResource
{
    //public string ResourceID { get; set; }
    //public string ResourceType { get; set; }
    //public float Quantity { get; set; }
    //public int CurrentValue { get; set; }
    public float Demand { get; set; }
    public float Price { get; set; }
    public int PositionPoor { get; set; }
    public int PositionMid { get; set; }
    public int PositionRich { get; set; }
    public float FulfilledRatio { get; set; }
    public float MinRatio { get; set; }
    public float OptimalRatio { get; set; }

    // Constructor per inicialitzar la classe
    [JsonConstructor]
    public CityService(string resourceId)
    {
        ResourceID = resourceId;
        
        // Buscar el ResourceType corresponent al ResourceID
        var matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == resourceId);
        ResourceType = matchedResource != null ? matchedResource.ResourceType : null;

        // Inicialitzar la resta de propietats
        Demand = 0;
        Price = 0;
        PositionPoor = 0;
        PositionMid = 0;
        PositionRich = 0;
        FulfilledRatio = 0;
        MinRatio = 0;
        OptimalRatio = 0;
    }

    // Nou constructor per ResourceType
    public CityService(string resourceType, float quantity, float demand)
    {
        ResourceID = null;  // Deixem el ResourceID com a null
        ResourceType = resourceType;
        Quantity = quantity;
        Demand = demand;
        Price = 0;
        
        // Inicialitzar la resta de propietats a zero
        PositionPoor = 0;
        PositionMid = 0;
        PositionRich = 0;
        FulfilledRatio = 0;
        MinRatio = 0;
        OptimalRatio = 0;
    }
    
}


