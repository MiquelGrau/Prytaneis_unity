using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionMethod
{
    public string MethodID { get; private set; }
    public string MethodName { get; private set; }
    public string MethodType { get; private set; }
    public int CycleTime { get; private set; }
    public List<MethodInput> Inputs { get; private set; }
    public List<MethodOutput> Outputs { get; private set; }

    // Definicions per a inputs i outputs
    public class MethodInput
    {
        public string ResourceID { get; set; }  // És String i no una Classe Resource, perquè es pugui buscar des del json
        public string ResourceType { get; set; }
        public string ResourceSubtype { get; set; }
        public float Amount { get; set; }

        
        // Constructor amb InputType
        public MethodInput(string resource, float amount, InputType inputType)
        {
            switch (inputType)
            {
                case InputType.ResourceID:
                    ResourceID = resource;
                    ResourceType = null;
                    ResourceSubtype = null;
                    break;
                case InputType.ResourceType:
                    ResourceID = null;
                    ResourceType = resource;
                    ResourceSubtype = null;
                    break;
                case InputType.ResourceSubtype:
                    ResourceID = null;
                    ResourceType = null;
                    ResourceSubtype = resource;
                    break;
            }
            Amount = amount;
        }
        public enum InputType
        {
            ResourceID,
            ResourceType,
            ResourceSubtype
        }
        
    }

    public class MethodOutput
    {
        public string ResourceID { get; set; }
        public float Amount { get; set; }
        public int Chance { get; set; }
        public string Type { get; set; }
    }

    // Constructor
    public ProductionMethod(string methodID, string methodName, string methodType, int cycleTime,
                            List<MethodInput> inputs, List<MethodOutput> outputs)
    {
        MethodID = methodID;
        MethodName = methodName;
        MethodType = methodType;
        CycleTime = cycleTime;
        Inputs = inputs ?? new List<MethodInput>();
        Outputs = outputs ?? new List<MethodOutput>();
    }

    
}

public class Batch
{
    public string BatchID { get; private set; }
    public List<BatchInput> BatchInputs { get; private set; }
    public List<BatchOutput> BatchOutputs { get; private set; }
    public int CycleTimeTotal { get; set; }
    public int CycleTimeProgress { get; set; }
    public float ExpectedSalary { get; set; }
    public float ExpectedOverhead { get; set; }
    public float ExpectedCosts { get; set; }
    public float AccruedSalary { get; private set; }
    public float AccruedOverhead { get; private set; }
    public float AccruedCosts { get; private set; }

    public class BatchInput
    {
        public Resource InputResource { get; private set; }
        public float InputAmount { get; private set; }
        public float InputUnitValue { get; private set; }

        public BatchInput(Resource inputResource, float inputAmount, float inputUnitValue)
        {
            InputResource = inputResource;
            InputAmount = inputAmount;
            InputUnitValue = inputUnitValue;
        }
    }
    
    public class BatchOutput
    {
        public Resource OutputResource { get; private set; }
        public float OutputAmount { get; private set; }
        public int OutputChance { get; private set; }
        public int OutputExpGain { get; private set; }
        public float OutputUnitValue { get; private set; }

        public BatchOutput(Resource outputResource, float outputAmount, int outputChance, int outputExpGain, float outputUnitValue)
        {
            OutputResource = outputResource;
            OutputAmount = outputAmount;
            OutputChance = outputChance;
            OutputExpGain = outputExpGain;
            OutputUnitValue = outputUnitValue;
        }
    }


    public Batch(string batchID, List<BatchInput> batchInputs, List<BatchOutput> batchOutputs, int cycleTimeTotal, float expectedSalary, float expectedOverhead, float expectedCosts)
    {
        BatchID = batchID;
        BatchInputs = batchInputs ?? new List<BatchInput>();
        BatchOutputs = batchOutputs ?? new List<BatchOutput>();
        CycleTimeTotal = cycleTimeTotal;
        CycleTimeProgress = 0;
        ExpectedSalary = expectedSalary;
        ExpectedOverhead = expectedOverhead;
        ExpectedCosts = expectedCosts;
        AccruedSalary = 0;
        AccruedOverhead = 0;
        AccruedCosts = 0;
    }
    
    public bool IsCompleted()
    {
        return CycleTimeProgress >= CycleTimeTotal;
    }
}



