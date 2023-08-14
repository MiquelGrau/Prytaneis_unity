using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    // Propietats dels recursos
    public int resourceID { get; set; }
    public string resourceName { get; set; }
    public int resourceQty { get; set; }
    public int basePrice { get; set; }
    public int currentPrice { get; set; }
    public float baseWeight { get; set; }

    // Constructor per inicialitzar la classe amb valors per defecte (opcional)
    public Resource(int id, string name, int qty, int baseP, int currentP, float baseW)
    {
        resourceID = id;
        resourceName = name;
        resourceQty = qty;
        basePrice = baseP;
        currentPrice = currentP;
        baseWeight = baseW;
    }
    
    // Aquí afegeixes la teva funció ToString personalitzada
    public override string ToString()
    {
        return $"{resourceID}: {resourceName}, {basePrice} €";
    }

}

[System.Serializable]
public class ResourceList
{
    public List<Resource> resources;
}

[System.Serializable]
public class ResourceString     // classe per llegir JSONs, primer tot string i després a cada cosa
{
    public string resourceID;
    public string resourceName;
    public string resourceQty;
    public string basePrice;
    public string currentPrice;
    public string baseWeight;
}

[System.Serializable]
public class ResourceListString
{
    public List<ResourceString> resource_jsonfile;
}

