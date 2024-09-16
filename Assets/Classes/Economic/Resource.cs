using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    // Propietats dels recursos. Nomes definicions, les quantitats aniran als inventaris
    public string ResourceID;
    public string ResourceName;
    public string ResourceType;
    public string ResourceSubtype;
    public int BasePrice;
    public float BaseWeight;

    public Resource(string id, string name, string type, string subtype, int price, float weight)
    {
        ResourceID = id;
        ResourceName = name;
        ResourceType = type;
        ResourceSubtype = subtype;
        BasePrice = price;
        BaseWeight = weight;
    }

}

[System.Serializable]
public class ResourceType
{
    public string Name;
    public List<ResourceSubtype> Subtypes; 
    public bool IsService;  

    public ResourceType(string name, bool isService)
    {
        Name = name;
        IsService = isService;
        Subtypes = new List<ResourceSubtype>();
    }
    
    public void AddSubtype(ResourceSubtype subtype)
    {
        Subtypes.Add(subtype);
    }
}

[System.Serializable]
public class ResourceSubtype
{
    public string Name;

    public ResourceSubtype(string name)
    {
        Name = name;
    }
}

