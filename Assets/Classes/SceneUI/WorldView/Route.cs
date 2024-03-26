using UnityEngine;
using System.Collections.Generic; // Afegeix aquesta línia
using System.Linq;

public class Route : MonoBehaviour
{
    public GameObject planet; // Assigna això des de l'Inspector d'Unity amb el teu objecte Earth
    private List<RouteData> createdRoutes = new List<RouteData>();
    private int routeCounter = 0;
    private List<GameObject> createdLines = new List<GameObject>();
    private int lineCounter = 0;

    void Start()
    {
    }

    // Funció per dibuixar una línia a través d'una llista de punts globals dins de "Earth"
    public void DrawLineOnEarth(List<Vector3> globalPoints)
    {
        if (globalPoints == null || globalPoints.Count < 2) return;

        string lineObjectName = $"GlobalLine_{lineCounter++}";
        GameObject lineObject = new GameObject(lineObjectName);
        lineObject.transform.SetParent(planet.transform, false);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Utilitza un shader que suporti colors
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
        lineRenderer.widthMultiplier = 0.0015f;
        lineRenderer.useWorldSpace = false;

        lineRenderer.positionCount = globalPoints.Count;
        lineRenderer.SetPositions(globalPoints.ToArray());

        createdLines.Add(lineObject);
    }

    // Funció per netejar totes les línies creades
    public void ClearAllLines()
    {
        foreach (var line in createdLines)
        {
            Destroy(line); // Destruir l'objecte de la línia
        }
        createdLines.Clear(); // Netejar la llista de línies
    }

    public void ConnectMarkersWithLine(Marker markerA, Marker markerB)
    {
        GameObject lineObject = new GameObject($"MarkersLine_{routeCounter}");
        lineObject.transform.SetParent(markerA.transform, false);
        
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material.color = Color.red;
        lineRenderer.widthMultiplier = 0.003f;
        lineRenderer.positionCount = 2;

        // Aquesta propietat fa que el LineRenderer utilitzi les coordenades locals del seu pare
        lineRenderer.useWorldSpace = false;

        // Estableix les posicions de la línia respecte al pare (markerA)
        Vector3 startPosition = Vector3.zero; // Inici és la posició local del marcador A
        Vector3 endPosition = markerA.transform.InverseTransformPoint(markerB.transform.position); // Converteix la posició global del marcador B a l'espai local del marcador A
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        string routeId = $"route_{routeCounter++}";
        RouteData newRoute = new RouteData(routeId, lineObject, markerA, markerB);
        createdRoutes.Add(newRoute);
    }

    public void RemoveRouteById(string routeId)
    {
        var routeToRemove = createdRoutes.FirstOrDefault(r => r.routeId == routeId);
        if (routeToRemove != null)
        {
            Destroy(routeToRemove.lineObject); // Elimina l'objecte de la línia de la escena
            createdRoutes.Remove(routeToRemove); // Elimina la ruta de la llista
        }
    }

    public void ClearAllRoutes()
    {
        foreach (var route in createdRoutes)
        {
            Destroy(route.lineObject); // Destruir l'objecte de línia
        }
        createdRoutes.Clear(); // Netejar la llista de rutes
        routeCounter = 0; // Opcional: Reiniciar el comptador si vols començar la numeració des de zero
    }

    Vector3 LatLongToPosition(float lat, float lon)
    {
        // Aquí, reutilitza el mètode de conversió de LatLongToPosition com en MarkersManager
        float baseRadius = planet.transform.localScale.x / 2; // Assumeix que l'escala de l'objecte és un diàmetre
        float heightOffset = 0; // Ajusta aquest valor si cal
        float radius = baseRadius + heightOffset;

        lat = Mathf.Deg2Rad * lat;
        lon = Mathf.Deg2Rad * lon;

        Vector3 direction = new Vector3(
            Mathf.Cos(lat) * Mathf.Cos(lon),
            Mathf.Sin(lat),
            Mathf.Cos(lat) * Mathf.Sin(lon)
        ).normalized;

        Vector3 position = planet.transform.position + direction * radius;

        return position;
    }

}

[System.Serializable]
public class RouteData
{
    public string routeId;
    public GameObject lineObject; // Referència a l'objecte de la línia per aquesta ruta
    public Marker startMarker;
    public Marker endMarker;

    public RouteData(string id, GameObject line, Marker start, Marker end)
    {
        routeId = id;
        lineObject = line;
        startMarker = start;
        endMarker = end;
    }
}
