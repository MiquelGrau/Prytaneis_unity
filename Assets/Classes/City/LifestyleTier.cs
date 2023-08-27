using System.Collections.Generic;

[System.Serializable]
public class LifestyleTier
{
    public int LifestyleTierID;  // ID que va de 1 a 15.
    public List<LifestyleDemand> LifestyleDemands;  // Demanda específica d'aquest Lifestyle

    public LifestyleTier(int id)
    {
        if (id < 1 || id > 15)
            throw new System.ArgumentOutOfRangeException("SatisfactionID ha de ser entre 1 i 15, incloent-hi.");

        LifestyleTierID = id;
        LifestyleDemands = new List<LifestyleDemand>();
    }

   
    
}

[System.Serializable]
public class LifestyleDemand
{
    public string resourceType;
    public int demandPerPopulation;
    public int monthsCritical;
    public int monthsTotal;
    public int variety;

    public LifestyleDemand(string resourceType, int demandPerPopulation, int monthsCritical, int monthsTotal, int variety)
    {
        this.resourceType = resourceType;
        this.demandPerPopulation = demandPerPopulation;
        this.monthsCritical = monthsCritical;
        this.monthsTotal = monthsTotal;
        this.variety = variety;
    }
}

// per importar el JSON
[System.Serializable]
public class LifestyleDataWrapper
{
    public List<LifestyleTierJSON> lifestyleData_jsonfile;
}

[System.Serializable]
public class LifestyleTierJSON
{
    public int lifestyleTierID;
    public List<LifestyleDemand> data;
}
