public class MarkersManager : MonoBehaviour
{
    public DataManager dataManager;
    public Planet planet;
    public GameObject markerPrefab;

    private void Start()
    {
        foreach (var city in dataManager.cities)
        {
            AddMarker(city.latitude, city.longitude, city.cityName);
        }
    }

    // La resta del codi del MarkersManager es manté igual
}

.
