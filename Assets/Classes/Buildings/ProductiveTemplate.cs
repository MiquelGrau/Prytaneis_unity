using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductiveTemplate : BuildingTemplate
{
    // Propietats específiques del template de edificis productius. Valors en barra són els de la classe general. 
    //public string TemplateID { get; private set; }
    //public string ClassName { get; private set; }
    //public string TemplateType { get; private set; }
    //public string TemplateSubtype { get; private set; }
    public ProductionMethod DefaultMethod { get; private set; }
    public List<ProductionMethod> PossibleMethods { get; private set; }
    public List<TemplateFactor> Factors { get; private set; }
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }
    public int Capacity { get; private set; }
    
    // Constructor
    public ProductiveTemplate(string templateID, string className, string templateType, string templateSubtype, 
                              ProductionMethod defaultMethod, List<ProductionMethod> possibleMethods,
                              List<TemplateFactor> factors, int jobsPoor, int jobsMid, int jobsRich, int capacity)
        : base(templateID, className, templateType, templateSubtype)
    {
        Factors = factors ?? new List<TemplateFactor>(); // Assigna una llista buida si factors és null
        DefaultMethod = defaultMethod;
        PossibleMethods = possibleMethods ?? new List<ProductionMethod>(); // Assigna una llista buida si possibleMethods és null
        JobsPoor = jobsPoor;
        JobsMid = jobsMid;
        JobsRich = jobsRich;
        Capacity = capacity;
    }
}


