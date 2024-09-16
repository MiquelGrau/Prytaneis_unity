using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LifestyleTier
{
    public string TierID; 
    public string TierName; 
    public string NextTierID;
    public List<LifestyleDemand> LifestyleDemands;
    public List<ServiceDemand> ServiceDemands; 

    
    public LifestyleTier(string tierID, string tierName, string nextTierID)
    {
        TierID = tierID;
        TierName = tierName;
        NextTierID = nextTierID;
        LifestyleDemands = new List<LifestyleDemand>();
        ServiceDemands = new List<ServiceDemand>(); 
    }
    
}

[System.Serializable]
public class LifestyleDemand
{
    public string ResType { get; set; }
    public string DemType { get; set; }
    public int Position { get; set; }
    public float MonthlyQty { get; set; }
    public int MonthsCrit { get; set; }
    public int MonthsTotal { get; set; }
    
    public LifestyleDemand(string resType, string demType, int position, float monthlyQty, int monthsCrit, int monthsTotal)
    {
        ResType = resType;
        DemType = demType;
        Position = position;
        MonthlyQty = monthlyQty;
        MonthsCrit = monthsCrit;
        MonthsTotal = monthsTotal;
        
    }
}

[System.Serializable]
public class ServiceDemand
{
    public string ResType { get; set; }
    public string DemType { get; set; }
    public int Position { get; set; }
    public float MonthlyQty { get; set; }
    public float Minimum { get; set; }  
    public float Optimum { get; set; }  

    public ServiceDemand(string resType, string demType, int position, float monthlyQty, float minimum, float optimum)
    {
        ResType = resType;
        DemType = demType;
        Position = position;
        MonthlyQty = monthlyQty;
        Minimum = minimum;
        Optimum = optimum;
    }
}
