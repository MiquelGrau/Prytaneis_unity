using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LifestyleTier
{
    public int TierID; 
    public string TierName; 
    public List<LifestyleNeed> LifestyleDemands;
    
    public LifestyleTier(int tierID)
    {
        TierID = tierID;
        //AssignTierDetails(tierID);    // emplenarà directament això, però ja ho aplicarem més tard. 
    }
   
    
}

[System.Serializable]
public class LifestyleNeed
{
    public string resourceType;
    public float quantityPerThousand;
    public int monthsCritical;
    public int monthsTotal;
    public int resourceVariety;

    public LifestyleNeed(string resourceType, float quantityPerThousand, int monthsCritical, int monthsTotal, int resourceVariety)
    {
        this.resourceType = resourceType;
        this.quantityPerThousand = quantityPerThousand;
        this.monthsCritical = monthsCritical;
        this.monthsTotal = monthsTotal;
        this.resourceVariety = resourceVariety;
    }
    
}

