using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivicBuilding : Building
{
    public string Function { get; private set; }
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }
    public List<Service> ServOffered { get; private set; }
    public List<Service> ServNeeded { get; private set; }

    public CivicBuilding(string id, string name, string templateID, string location, string ownerID, string inventoryID,
                          string activity, int size, int hpCurrent, int hpMax, int capacity,
                          string function, int jobsPoor, int jobsMid, int jobsRich)
        : base(id, name, templateID, location, ownerID, inventoryID, activity, size, hpCurrent, hpMax, capacity)
    {
        Function = function;
        JobsPoor = jobsPoor;
        JobsMid = jobsMid;
        JobsRich = jobsRich;

        ServOffered = new List<Service>();
        ServNeeded = new List<Service>();
    }
}

public class CivicTemplate : BuildingTemplate
{
    // Propietats específiques del template civil
    //public string TemplateID { get; private set; }
    //public string ClassName { get; private set; }
    //public string TemplateType { get; private set; }
    //public string TemplateSubtype { get; private set; }
    public string Function { get; private set; }
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }
    public float Repeat { get; private set; }
    public float Labour { get; private set; }
    public float HardMat { get; private set; }
    public float LightMat { get; private set; }
    public float SpecialMat { get; private set; }
    public float BuildPointCost { get; private set; }
    public string WaterSupport { get; private set; }
    public bool WaterCondition { get; private set; }
    
    public List<Service> ServOffered { get; private set; }
    public List<Service> ServNeeded { get; private set; }
    
    // Constructor 
    public CivicTemplate(string templateID, string className, string templateType,
                         string templateSubtype, string function, int jobsPoor, int jobsMid, int jobsRich,
                         float repeat, float labour, float hardMat, float lightMat, float specialMat,
                         float buildPointCost, string waterSupport, bool waterCondition,
                         List<Service> servOffered, List<Service> servNeeded)
        : base(templateID, className, templateType, templateSubtype)
    {
        Function = function;
        JobsPoor = jobsPoor;
        JobsMid = jobsMid;
        JobsRich = jobsRich;
        Repeat = repeat;
        Labour = labour;
        HardMat = hardMat;
        LightMat = lightMat;
        SpecialMat = specialMat;
        BuildPointCost = buildPointCost;
        WaterSupport = waterSupport;
        WaterCondition = waterCondition;
        ServOffered = servOffered;
        ServNeeded = servNeeded;
    }
}

