using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivicBuilding : Building
{
    // Refs que venen de Building
    //public string BuildingID { get; set; }
    //public string BuildingName { get; set; }
    //public string BuildingTemplateID { get; set; }
    //public string BuildingLocation { get; set; }
    //public string BuildingOwnerID { get; set; }
    //public string RelatedInventoryID { get; set; }
    //public string ActivityStatus { get; set; }
    //public int BuildingSize { get; set; }
    //public int HPCurrent { get; set; }
    //public int HPMaximum { get; set; }
    public string Function { get; private set; }
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }
    public List<Service> ServOffered { get; set; }
    public List<Service> ServNeeded { get; set; }

    public CivicBuilding(string id, string name, string templateID, string location, string ownerID, string inventoryID,
                          string activity, int size, int hpCurrent, int hpMax,
                          string function, int jobsPoor, int jobsMid, int jobsRich,
                          List<Service> servOffered = null, List<Service> servNeeded = null)
        : base(id, name, templateID, location, ownerID, inventoryID, activity, size, hpCurrent, hpMax)
    {
        Function = function;
        JobsPoor = jobsPoor;
        JobsMid = jobsMid;
        JobsRich = jobsRich;

        ServOffered = servOffered ?? new List<Service>();
        ServNeeded = servNeeded ?? new List<Service>();
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
    public float BuildPointCost { get; set; }   // Obert per a recalcular, al AllocateBuildingMng
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

