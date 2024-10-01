using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingAllocatorManager : MonoBehaviour
{
    // Llista d'edificis elegibles per a ser seleccionats
    public List<CivicTemplate> PossibleBuildingList = new List<CivicTemplate>();
    public BuildingManager buildingManager;

    private void Start()
    {
        
        CopyCivicTemplates();

        foreach (Location city in DataManager.Instance.allCityList)
        {
            CalculateBuildPointsForCity(city);
            ProcessCityServices(city);
            AllocateMinRatioBuildings(city);
        }
    }

    private void CopyCivicTemplates()
    {
        // Copiar els civicTemplates de DataManager a PossibleBuildingList
        PossibleBuildingList = new List<CivicTemplate>(DataManager.Instance.civicTemplates);
        //Debug.Log($"PossibleBuildingList inicialitzada amb {PossibleBuildingList.Count} templates.");
    }

    // Funció per modificar els BuildPoints
    public void CalculateBuildPointsForCity(Location location)
    {
        foreach (var civicTemplate in PossibleBuildingList)
        {
            int existingBuildingsCount = 0;

            // Comprovar quants edificis del mateix template hi ha a la ciutat
            foreach (var building in location.Buildings)
            {
                if (building.BuildingTemplateID == civicTemplate.TemplateID)
                {
                    existingBuildingsCount += building.BuildingSize;
                }
            }

            // Càlcul dels BuildPoints amb Repeat
            float repeatFactor = Mathf.Pow(1 + civicTemplate.Repeat, existingBuildingsCount);
            float adjustedBuildPoints = civicTemplate.BuildPointCost * repeatFactor;

            //Debug.Log($"Edifici {civicTemplate.ClassName} té BuildPoints ajustats: {adjustedBuildPoints} amb {existingBuildingsCount} edificis existents.");
        }
    }

    // Funció per agregar serveis oferts dels edificis als serveis de la ciutat passada com a input
    public void ProcessCityServices(Location city)
    {
        if (city == null)
        {
            Debug.LogError("La ciutat és nul·la.");
            return;
        }

        // Obtenim l'inventari de la ciutat utilitzant el nou nom de la funció
        CityInventory cityInventory = DataManager.Instance.GetLocInvByID(city.InventoryID);
        if (cityInventory == null)
        {
            Debug.LogError("L'inventari de la ciutat és nul.");
            return;
        }

        // Recorre tots els edificis de la ciutat proporcionada
        foreach (var building in city.Buildings)
        {
            if (building is CivicBuilding civicBuilding && civicBuilding.ServOffered != null)
            {
                // Recorre els serveis oferts per l'edifici
                foreach (var service in civicBuilding.ServOffered)
                {
                    // Busca el servei a la llista de serveis de la ciutat segons el tipus de servei
                    /* var existingService = city.CityInventory.Services
                        .FirstOrDefault(s => s.ResourceType == service.ResourceType); */
                    var existingService = cityInventory.Services
                    .FirstOrDefault(s => s.ResourceType == service.ResourceType);

                    if (existingService != null)
                    {
                        // Si existeix, suma la quantitat de servei ofert a la quantitat del servei de la ciutat
                        existingService.Demand += service.Quantity;
                        /* Debug.Log($"Edifici ID {building.BuildingID}, {building.BuildingName}, " +
                        $"[Offer] Actualitzant servei: {existingService.ResourceType} amb quantitat {service.Quantity}"); */
                    }
                    else
                    {
                        // Si no existeix, crea un nou servei a la llista de serveis de la ciutat
                        var newCityService = new CityService(service.ResourceType, service.Quantity, 0);
                        //city.CityInventory.Services.Add(newCityService);
                        cityInventory.Services.Add(newCityService);
                        /* Debug.Log($"Edifici ID {building.BuildingID}, {building.BuildingName}, " + 
                        $"[Offer] Afegint nou servei: {newCityService.ResourceType} amb quantitat {newCityService.Quantity}"); */
                    }
                }
            }

            // Processar serveis necessaris (ServNeeded)
            if (building is CivicBuilding civicBuildingNeeded && civicBuildingNeeded.ServNeeded != null)
            {
                // Recorre els serveis necessaris per l'edifici
                foreach (var service in civicBuildingNeeded.ServNeeded)
                {
                    // Busca el servei a la llista de serveis de la ciutat segons el tipus de servei
                    /* var existingService = city.CityInventory.Services
                        .FirstOrDefault(s => s.ResourceType == service.ResourceType); */
                    var existingService = cityInventory.Services
                        .FirstOrDefault(s => s.ResourceType == service.ResourceType);

                    if (existingService != null)
                    {
                        // Si existeix, suma la quantitat necessària a la demanda del servei de la ciutat
                        existingService.Demand += service.Quantity;
                        /* Debug.Log($"Edifici ID {building.BuildingID}, {building.BuildingName}, " +
                        $"[Demand] Actualitzant servei: {existingService.ResourceType} amb quantitat {service.Quantity}"); */
                    }
                    else
                    {
                        // Si no existeix, crea un nou servei a la llista de serveis de la ciutat
                        var newCityService = new CityService(service.ResourceType, 0, service.Quantity);
                        //city.CityInventory.Services.Add(newCityService);
                        cityInventory.Services.Add(newCityService);
                        /* Debug.Log($"Edifici ID {building.BuildingID}, {building.BuildingName}, " + 
                        $"[Demand] Afegint nou servei: {newCityService.ResourceType} amb quantitat {newCityService.Quantity}"); */
                    }
                }
            }
        }   

        Debug.Log($"Processats els serveis oferts de tots els edificis per la ciutat: {city.Name}");


        // Recorre tots els CityServices de la ciutat proporcionada
        foreach (var cityService in cityInventory.Services)
        {
            // Calcular el FulfilledRatio (Quantity / Demand). Si Demand és 0, fixa FulfilledRatio a 1.00.
            if (cityService.Demand == 0) { cityService.FulfilledRatio = 1.00f; }
            else { cityService.FulfilledRatio = cityService.Quantity / cityService.Demand; }

            // Mostrar les propietats del CityService amb un Debug.Log
            //Debug.Log($"CityService: {cityService.ResourceType}, Off: {cityService.Quantity}, Dem: {cityService.Demand}, " +
            //        $"Ratios: Real {cityService.FulfilledRatio}, Min: {cityService.MinRatio}, Opt: {cityService.OptimalRatio}");
        }
    }

    public void AllocateMinRatioBuildings(Location city)
    {
        CityInventory cityInventory = DataManager.Instance.GetLocInvByID(city.InventoryID);

        if (city == null || cityInventory == null)
        {
            Debug.LogError("La ciutat o el seu inventari és nul.");
            return;
        }

        foreach (var serviceToSatisfy in cityInventory.Services
            .Where(s => s.FulfilledRatio < s.MinRatio)
            .OrderBy(s => s.FulfilledRatio))  // Prioritzem els que estan més lluny de MinRatio
        {
            // Bucle mentre el servei no estigui satisfet i hi hagi BuildPoints disponibles
            while (city.BuildPoints > 0 && serviceToSatisfy.FulfilledRatio < serviceToSatisfy.MinRatio)
            {
                // Buscar un edifici a PossibleBuildingList que ofereixi aquest servei
                CivicTemplate buildingToAdd = PossibleBuildingList
                    .FirstOrDefault(template => template.ServOffered.Any(s => s.ResourceType == serviceToSatisfy.ResourceType));

                if (buildingToAdd == null)
                {
                    Debug.LogWarning($"No s'ha trobat cap edifici que ofereixi el servei: {serviceToSatisfy.ResourceType}");
                    break;  // Sortim d'aquest servei però continuem amb la resta
                }
                
                // Buscar si ja existeix un edifici del mateix tipus a la ciutat
                var existingBuilding = city.Buildings
                    .FirstOrDefault(b => b.BuildingTemplateID == buildingToAdd.TemplateID);
                //float buildPointCost = 0;

                if (existingBuilding != null)
                {
                    // Si ja existeix un edifici, incrementar el BuildingSize en lloc de crear un nou edifici, i gastar els punts
                    city.BuildPoints -= buildingToAdd.BuildPointCost;
                    existingBuilding.BuildingSize += 1;
                    //Debug.Log($"Edifici {existingBuilding.BuildingName} +1 tamany a {existingBuilding.BuildingSize}. " + 
                    //$"Gastat: {buildingToAdd.BuildPointCost}, BP restants: {city.BuildPoints}");
                }
                else
                {
                    // Si no hi ha cap edifici existent, crear-ne un de nou
                    buildingManager.AddCivicBuilding(buildingToAdd, city);
                    city.BuildPoints -= buildingToAdd.BuildPointCost;
                    //Debug.Log($"Nou edifici creat: {buildingToAdd.ClassName}, gastat: {buildingToAdd.BuildPointCost}, BP restants: {city.BuildPoints}");
                }
                // Ara recalcularem el BuildPointCost per al següent ús i l'assignem directament al CivicTemplate a PossibleBuildingList
                var updatedBaseTemplate = DataManager.Instance.civicTemplates.FirstOrDefault(t => t.TemplateID == buildingToAdd.TemplateID);
                if (updatedBaseTemplate != null)
                {
                    float newBuildPointCost = updatedBaseTemplate.BuildPointCost * Mathf.Pow(1 + updatedBaseTemplate.Repeat, existingBuilding != null ? existingBuilding.BuildingSize : 1);
                    buildingToAdd.BuildPointCost = newBuildPointCost;
                    //Debug.Log($"Nou cost de BuildPoints per l'edifici {buildingToAdd.ClassName}: {newBuildPointCost}");
                }

                // Actualitzar els serveis oferts per l'edifici
                foreach (var offeredService in buildingToAdd.ServOffered)
                {
                    var existingCityService = cityInventory.Services
                        .FirstOrDefault(s => s.ResourceType == offeredService.ResourceType);

                    if (existingCityService != null)
                    {
                        existingCityService.Quantity += offeredService.Quantity;
                        existingCityService.FulfilledRatio = existingCityService.Quantity / existingCityService.Demand;
                    }
                }

                //Debug.Log($"Actualitzat servei: {serviceToSatisfy.ResourceType}, FulfilledRatio: {serviceToSatisfy.FulfilledRatio}");
            }
        }
        Debug.Log("Procés d'assignació d'edificis finalitzat.");
    }


}
