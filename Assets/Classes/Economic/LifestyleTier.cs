using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LifestyleTier
{
    public string TierID; 
    public string TierName; 
    public List<LifestyleDemand> LifestyleDemands;
    public string NextTierID;
    
    public LifestyleTier(string tierID, string nextTierID)
    {
        TierID = tierID;
        NextTierID = nextTierID;
        LifestyleDemands = new List<LifestyleDemand>();
        
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

