using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivicTemplate : BuildingTemplate
{
    // Propietats específiques del template civil
    public string Function { get; private set; }
    public int Capacity { get; private set; }
    public int JobsRich { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsPoor { get; private set; }

    // Constructor 
    public CivicTemplate(string templateID, string className, string templateType,
                         string templateSubtype, string function, int capacity, int jobsRich, int jobsMid, int jobsPoor)
        : base(templateID, className, templateType, templateSubtype)
    {
        Function = function;
        Capacity = capacity;
        JobsRich = jobsRich;
        JobsMid = jobsMid;
        JobsPoor = jobsPoor;
    }
}