using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductiveBuilding : Building
{
    public string ProductionTempID { get; private set; }
    public List<ProductiveFactor> CurrentFactors { get; set; } 
    public List<string> MethodsAvailable { get; private set; }
    public string MethodActive { get; private set; }
    public string MethodDefault { get; private set; }
    public Batch BatchCurrent { get; set; } 
    public List<Batch> BatchBacklog { get; private set; } 
    public float LinearOutput { get; set; }
    public float InputEfficiency { get; set; }
    public float OutputEfficiency { get; set; }
    public float CycleEfficiency { get; set; }
    public float SalaryEfficiency { get; set; }
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }

    public ProductiveBuilding(string id, string name, string templateID, string location, string ownerID, string inventoryID,
                              string activity, int size, int hpCurrent, int hpMax, int capacity,
                              string productionTempID, List<ProductiveFactor> currentFactors, List<string> methodsAvailable,
                              string methodActive, string methodDefault, Batch batchCurrent, 
                              List<Batch> batchBacklog, float linearOutput, float inputEfficiency, float outputEfficiency,
                              float cycleEfficiency, float salaryEfficiency, int jobsPoor, int jobsMid, int jobsRich)
        : base(id, name, templateID, location, ownerID, inventoryID, activity, size, hpCurrent, hpMax, capacity)
    {
        ProductionTempID = productionTempID;
        CurrentFactors = currentFactors ?? new List<ProductiveFactor>();
        MethodsAvailable = methodsAvailable ?? new List<string>();
        MethodActive = methodActive;
        MethodDefault = methodDefault;
        BatchCurrent = batchCurrent;
        BatchBacklog = batchBacklog ?? new List<Batch>();
        LinearOutput = linearOutput;
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
    public string DefaultMethod { get; set; }
    public List<string> PossibleMethods { get; set; } = new List<string>();
    public List<string> Factors { get; set; } = new List<string>();
    public int JobsPoor { get; private set; }
    public int JobsMid { get; private set; }
    public int JobsRich { get; private set; }
    public int Capacity { get; private set; }
    
    // Constructor
    public ProductiveTemplate(string templateID, string className, string templateType, string templateSubtype, 
                              string defaultMethod, List<string> possibleMethods,
                              List<string> factors, int jobsPoor, int jobsMid, int jobsRich, int capacity)
        : base(templateID, className, templateType, templateSubtype)
    {
    
        DefaultMethod = defaultMethod;
        PossibleMethods = possibleMethods ?? new List<string>(); 
        Factors = factors ?? new List<string>(); 
        JobsPoor = jobsPoor;
        JobsMid = jobsMid;
        JobsRich = jobsRich;
        Capacity = capacity;
    }
}


public class AgriculturalBuilding : ProductiveBuilding
{
    
    public float FrostEffect { get; private set; }
    public float HeatEffect { get; private set; }
    public int MethodAge { get; private set; }  // edat de la planta, produirà a partir de X temps segons la especie. 
    public float SoilHealth { get; private set; }

    public AgriculturalBuilding(string id, string name, string templateID, string location, string ownerID, string inventoryID,
                                string activity, int size, int hpCurrent, int hpMax, int capacity,
                                string productionTempID, List<ProductiveFactor> currentFactors, List<string> methodsAvailable,
                                string methodActive, string methodDefault, Batch batchCurrent,
                                List<Batch> batchBacklog, float linearOutput, float inputEfficiency, float outputEfficiency,
                                float cycleEfficiency, float salaryEfficiency, int jobsPoor, int jobsMid, int jobsRich,
                                float frostEffect, float heatEffect, int methodAge, float soilHealth)
        : base(id, name, templateID, location, ownerID, inventoryID, activity, size, hpCurrent, hpMax, capacity,
               productionTempID, currentFactors, methodsAvailable, methodActive, methodDefault, batchCurrent,
               batchBacklog, linearOutput, inputEfficiency, outputEfficiency, cycleEfficiency, salaryEfficiency, jobsPoor, jobsMid, jobsRich)
    {
        FrostEffect = 100f;
        HeatEffect = 100f;
        MethodAge = 0;
        SoilHealth = 100f;
    }
}
// Agricultural building no necessita template, perquè no té res especial respecte l'altre, són tot propietats
// que s'afegiran un cop ja creat l'edifici i seleccionat el Production Method a treballar. 

