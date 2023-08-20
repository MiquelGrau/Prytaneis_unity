using UnityEngine;

public class UIManager : MonoBehaviour
{
    public BuildingDataManager buildingDataManager;

    public void OnHouseButtonClicked()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
        buildingDataManager.InstantiateHouse(randomPosition);
    }
}
