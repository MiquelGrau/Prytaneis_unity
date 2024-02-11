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
    public List<CityInventoryResource> InventoryResources { get; set; }
    public List<CityDemands> Demands { get; set; } = new List<CityDemands>();
    
    // Constructor
    public CityInventory(string cityInvID, string cityID, int cityInvMoney, List<CityInventoryResource> resources)
    {
        CityInvID = cityInvID;
        CityID = cityID;
        CityInvMoney = cityInvMoney;
        InventoryResources = resources ?? new List<CityInventoryResource>(); // Assigna una nova llista buida si items és null
    }
    

    [System.Serializable]
    public class CityDemands
    {
        public string ResourceType { get; private set; }
        public string ResourceID { get; private set; }
        public string ResourceSubtype { get; private set; }
        public string PopulationType { get; private set; }
        public int Variety { get; private set; }
        public float DemandConsume { get; set; }
        public float DemandCritical { get; set; }
        public float DemandTotal { get; set; }

        public enum DemandType
        {
            ResourceType,
            ResourceSubtype,
            ResourceID
        }

        // Constructor per Resource Type
        public CityDemands(DemandType type, string firstParameter, string populationType, int variety)
        {
            switch (type)
            {
                case DemandType.ResourceType:
                    ResourceType = firstParameter;
                    ResourceSubtype = null;
                    ResourceID = null;
                    break;
                case DemandType.ResourceSubtype:
                    ResourceType = null;
                    ResourceSubtype = firstParameter;
                    ResourceID = null;
                    break;
                case DemandType.ResourceID:
                    ResourceType = null;
                    ResourceSubtype = null;
                    ResourceID = firstParameter;
                    break;
            }

            PopulationType = populationType;
            Variety = variety;
            DemandConsume = 0;
            DemandCritical = 0;
            DemandTotal = 0;
        }

        
    }


}



[System.Serializable]
public class CityInventoryResource : InventoryResource
{
    //public string ResourceID { get; set; }
    //public string ResourceType { get; set; }
    //public float Quantity { get; set; }
    //public int CurrentValue { get; set; }
    public float DemandConsume { get; set; }
    public float DemandCritical { get; set; }
    public float DemandTotal { get; set; }
    public int VarietyAssigned { get; set; }
    public int CurrentPrice { get; set; }
    
    // Constructors

    // Nou item, normal
    [JsonConstructor]
    public CityInventoryResource(string resourceId, float quantity, int currentValue)
    {
        ResourceID = resourceId;
        Quantity = quantity;
        CurrentValue = currentValue;
        
        // Busca el ResourceType corresponent al ResourceID
        var matchedResource = DataManager.resources.FirstOrDefault(r => r.resourceID == resourceId);
        ResourceType = matchedResource != null ? matchedResource.resourceType : null;


        //ResourceType = null;
        
        // Resta de la inicialització
        DemandConsume = 0;
        DemandCritical = 0;
        DemandTotal = 0;
        VarietyAssigned = 0;
        CurrentPrice = 0;
        
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
        VarietyAssigned = 0;
        CurrentPrice = 0;
        
    }
    
}
