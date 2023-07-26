using UnityEngine;

public class Markers : MonoBehaviour
{
    public GameObject markerPrefab; // Assigna el teu prefab de marcador aquí des de l'Editor d'Unity
    public Transform earthTransform; // Assigna l'objecte de la Terra aquí des de l'Editor d'Unity

    [System.Serializable]
    public struct CityMarker
    {
        public string cityName;
        public Vector2 latLong;
    }

    public CityMarker[] cityMarkers;

    private void Start()
    {
        foreach (CityMarker city in cityMarkers)
        {
            PlaceCityMarker(city);
        }
    }

    void PlaceCityMarker(CityMarker city)
    {
        Vector3 position = LatLongToPosition(city.latLong);
        GameObject marker = Instantiate(markerPrefab, position, Quaternion.identity, earthTransform);
        marker.name = city.cityName;
    }

    Vector3 LatLongToPosition(Vector2 latLong)
    {
        latLong.x = Mathf.Clamp(latLong.x, -89.9f, 89.9f); // Limita la latitud per evitar errors amb el càlcul

        float latitude = Mathf.Deg2Rad * latLong.x;
        float longitude = Mathf.Deg2Rad * latLong.y;

        float x = Mathf.Cos(latitude) * Mathf.Cos(longitude);
        float y = Mathf.Sin(latitude);
        float z = Mathf.Cos(latitude) * Mathf.Sin(longitude);

        Vector3 position = new Vector3(x, y, z).normalized * (earthTransform.localScale.x / 2 + 0.5f); // Aquesta línia suposa que la Terra té un radi d'1 unitat, ajusta segons calgui
        return position;
    }
}
