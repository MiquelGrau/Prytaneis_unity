using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductiveFactor
{
    public string BaseTemplateID { get; private set; }
    public TemplateFactor FactorTemplate { get; private set; }
    public string FactorName { get { return FactorTemplate.FactorName; } }
    public string FactorType { get { return FactorTemplate.FactorType; } }
    public string FactorEffect { get { return FactorTemplate.FactorEffect; } }

    public ProductiveFactor(TemplateFactor templateFactor, string baseTemplateID)
    {
        FactorTemplate = templateFactor;
        BaseTemplateID = baseTemplateID;
    }
}

public class TemplateFactor // Els components d'un edifici productiu: Com funcionen, i com/quant afecten. 
{
    public string FactorID { get; private set; }
    public string FactorName { get; private set; }
    public string FactorType { get; private set; }
    public string FactorEffect { get; private set; }

    // Constructor
    public TemplateFactor(string id, string name, string type, string effect)
    {
        FactorID = id;
        FactorName = name;
        FactorType = type;
        FactorEffect = effect;
    }
    
}

public class EmployeePT : ProductiveFactor
{
    public int EffectSize { get; private set; }
    public int CurrentEmployees { get; private set; }
    public float MonthlySalary { get; private set; }

    public EmployeePT(TemplateFactor templateFactor, string baseTemplateID, int effectSize, int currentEmployees, float monthlySalary)
        : base(templateFactor, baseTemplateID)
    {
        EffectSize = effectSize;
        CurrentEmployees = currentEmployees;
        MonthlySalary = monthlySalary;
    }

    
}

// Classe derivada per al factor d'empleats
public class EmployeeFT : TemplateFactor
{
    //public string FactorID { get; private set; }
    //public string FactorName { get; private set; }
    //public string FactorType { get; private set; }
    //public string FactorEffect { get; private set; }
    public int EffectSize { get; private set; }
    public string WorkerID { get; private set; }
    public string Strata { get; private set; }
    public int Minimum { get; private set; }
    public int Optimal { get; private set; }
    public int Maximum { get; private set; }
    public string ShortfallEft { get; private set; }
    public int ShortfallSize { get; private set; }
    
    // Constructor
    public EmployeeFT(string factorID, string factorName, string factorType, 
                      string factorEffect, int effectSize, 
                      string workerID, string strata, 
                      int minimum, int optimal, int maximum, 
                      string shortfallEft, int shortfallSize)
        : base(factorID, factorName, factorType, factorEffect)
    {
        WorkerID = workerID;
        Strata = strata;
        Minimum = minimum;
        Optimal = optimal;
        Maximum = maximum;
        ShortfallEft = shortfallEft;
        ShortfallSize = shortfallSize;
        EffectSize = effectSize;
    }
}

public class ResourcePT : ProductiveFactor
{
    public int EffectSize { get; private set; }
    public Resource CurrentResource { get; private set; }
    public float CurrentQuantity { get; private set; }
    public float MonthlyConsumption { get; private set; }
    public float MonthlyValue { get; private set; }

    public ResourcePT(TemplateFactor templateFactor, string baseTemplateID, int effectSize, Resource currentResource, 
                      float currentQuantity, float monthlyConsumption, float monthlyValue)
        : base(templateFactor, baseTemplateID)
    {
        EffectSize = effectSize;
        CurrentResource = currentResource;
        CurrentQuantity = currentQuantity;
        MonthlyConsumption = monthlyConsumption;
        MonthlyValue = monthlyValue;
    }

    // Aquí pots afegir mètodes per gestionar el consum mensual, l'actualització de la quantitat actual, etc.
}

// Classe derivada per al factor de combustible
public class ResourceFT : TemplateFactor
{
    //public string FactorID { get; private set; }
    //public string FactorName { get; private set; }
    //public string FactorType { get; private set; }
    //public string FactorEffect { get; private set; }
    public int EffectSize { get; private set; }
    public string ResourceID { get; private set; }
    public float MonthlyConsumption { get; private set; }
    public int Minimum { get; private set; }
    public int Optimal { get; private set; }
    public int Maximum { get; private set; }
    public string ShortfallEft { get; private set; }
    public int ShortfallSize { get; private set; }
    
    // Constructor
    public ResourceFT(string factorID, string factorName, string factorType, 
                      string factorEffect, int effectSize,
                      string resourceID, float monthlyConsumption,
                      int minimum, int optimal, int maximum,
                      string shortfallEft, int shortfallSize)
        : base(factorID, factorName, factorType, factorEffect)
    {
        EffectSize = effectSize;
        ResourceID = resourceID;
        MonthlyConsumption = monthlyConsumption;
        Minimum = minimum;
        Optimal = optimal;
        Maximum = maximum;
        ShortfallEft = shortfallEft;
        ShortfallSize = shortfallSize;
    }
}