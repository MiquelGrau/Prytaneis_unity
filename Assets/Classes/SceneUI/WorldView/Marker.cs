using UnityEngine;

public class Marker : MonoBehaviour
{
    public GameObject contextMenuPrefab;
    public string cityName;
    public string id;
    public Vector3 position;
    //public DataManager dataManager;
    private GameObject contextMenuInstance;
    private WorldSceneInteractionMode currentMode = WorldSceneInteractionMode.Default;

    private void Start()
    {
        // Subscriu-te a l'event OnModeChange
        WorldSceneManager.Instance.OnModeChange += OnModeChange;
    }

    private void OnDestroy()
    {
        // Assegura't de desubscriure't de l'event quan aquest objecte es destrueixi
        if (WorldSceneManager.Instance != null)
        {
            WorldSceneManager.Instance.OnModeChange -= OnModeChange;
        }
    }

    private void OnModeChange(WorldSceneInteractionMode newMode)
    {
        // Actualitza el mode actual quan l'event sigui disparat
        currentMode = newMode;
    }

    private void OnMouseDown() // LMB, clic esquerra. Info sobre la ciutat, o preparar ruta. 
    {
        switch (currentMode)
        {
            case WorldSceneInteractionMode.Default:
                PlayerPrefs.SetString("SelectedCity", cityName);
                Debug.Log($"Mode Default: {cityName} seleccionada.");
                // Potencialment carrega una altra escena o realitza alguna acció específica del mode Default
                break;

            case WorldSceneInteractionMode.Route:
                Debug.Log($"Mode Route: Creant ruta cap a {cityName}, ID {id}.");
                /* string startNodeId = "LN0001";
                var markersManager = FindObjectOfType<MarkersManager>(); // Troba l'instància de MarkersManager
                if (markersManager != null)
                {
                    markersManager.OnNewRouteSelected(startNodeId, id); // Inicia la generació de la ruta amb l'ID actual com a destí
                } */

                // Obtenim l'agent actual des de GameManager
                var currentAgent = GameManager.Instance.CurrentAgent;
                if (currentAgent != null)
                {
                    string startNodeId = currentAgent.LocationNode; 
                    var markersManager = FindObjectOfType<MarkersManager>(); // Troba l'instància de MarkersManager
                    if (markersManager != null)
                    {
                        markersManager.OnNewRouteSelected(startNodeId, id); // Inicia la generació de la ruta amb l'ID actual com a destí
                    }
                }
                break;

            default:
                Debug.LogWarning("Acció no definida per a aquest mode.");
                break;
        }
    }
}
