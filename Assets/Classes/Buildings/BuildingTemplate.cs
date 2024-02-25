using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTemplate
{
    public string TemplateID { get; private set; }
    public string ClassName { get; private set; }
    public string TemplateType { get; private set; }
    public string TemplateSubtype { get; private set; }

    // Constructor protegit a la classe base
    protected BuildingTemplate(string templateID, string className, string templateType, string templateSubtype)
    {
        TemplateID = templateID;
        ClassName = className;
        TemplateType = templateType;
        TemplateSubtype = templateSubtype;
    }
    
}


