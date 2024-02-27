using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivicTemplate : BuildingTemplate
{
    // Propietats específiques del template civil
    //public string TemplateID { get; private set; }
    //public string ClassName { get; private set; }
    //public string TemplateType { get; private set; }
    //public string TemplateSubtype { get; private set; }
    public string Function { get; private set; }
    public int Capacity { get; private set; }
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }
    
    // Constructor 
    public CivicTemplate(string templateID, string className, string templateType,
                         string templateSubtype, string function, int capacity,  int jobsPoor, int jobsMid, int jobsRich)
        : base(templateID, className, templateType, templateSubtype)
    {
        Function = function;
        Capacity = capacity;
        JobsPoor = jobsPoor;
        JobsMid = jobsMid;
        JobsRich = jobsRich;
    }
}