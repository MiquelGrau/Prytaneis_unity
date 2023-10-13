using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    // Propietats dels recursos. Nomes definicions, les quantitats aniran als inventaris
    public string resourceID;
    public string resourceName;
    public string resourceType;
    public string resourceSubtype;
    public int basePrice;
    public float baseWeight;

    public Resource(string id, string name, string type, string subtype, int price, float weight)
    {
        resourceID = id;
        resourceName = name;
        resourceType = type;
        resourceSubtype = subtype;
        basePrice = price;
        baseWeight = weight;
    }

}