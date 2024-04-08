using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductiveBuilding : Building
{
    public string ProductionTempID { get; private set; }
    public List<ProductiveFactor> CurrentFactors { get; private set; } // Aquest serà reemplaçat per les instàncies reals de factors més tard.
    public List<ProductionMethod> MethodsAvailable { get; private set; }
    public ProductionMethod MethodActive { get; private set; }
    public ProductionMethod MethodDefault { get; private set; }
    public string BatchCurrent { get; private set; } // Aquest serà reemplaçat per la classe real BatchRun més tard.
    public List<string> BatchBacklog { get; private set; } // Aquest serà reemplaçat per una llista de BatchRuns més tard.
    public float InputEfficiency { get; private set; }
    public float OutputEfficiency { get; private set; }
    public float CycleEfficiency { get; private set; }
    public float SalaryEfficiency { get; private set; }
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }

    public ProductiveBuilding(string id, string name, string templateID, string location, string ownerID, string inventoryID,
                              string activity, int size, int hpCurrent, int hpMax, int capacity,
                              string productionTempID, List<ProductiveFactor> currentFactors, List<ProductionMethod> methodsAvailable,
                              ProductionMethod methodActive, ProductionMethod methodDefault, string batchCurrent, 
                              List<string> batchBacklog, float inputEfficiency, float outputEfficiency,
                              float cycleEfficiency, float salaryEfficiency, int jobsPoor, int jobsMid, int jobsRich)
        : base(id, name, templateID, location, ownerID, inventoryID, activity, size, hpCurrent, hpMax, capacity)
    {
        ProductionTempID = productionTempID;
        CurrentFactors = currentFactors ?? new List<ProductiveFactor>();
        MethodsAvailable = methodsAvailable ?? new List<ProductionMethod>();
        MethodActive = methodActive;
        MethodDefault = methodDefault;
        BatchCurrent = batchCurrent;
        BatchBacklog = batchBacklog ?? new List<string>();
        InputEfficiency = inputEfficiency;
        OutputEfficiency = outputEfficiency;
        CycleEfficiency = cycleEfficiency;
        SalaryEfficiency = salaryEfficiency;
        JobsPoor = jobsPoor;
        JobsMid = jobsMid;
        JobsRich = jobsRich;
    }

    // Mètode públic per afegir factors
    public void AddFactor(ProductiveFactor factor)
    {
        if(CurrentFactors == null)
        {
            CurrentFactors = new List<ProductiveFactor>();
        }
        CurrentFactors.Add(factor);
    }
}

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


