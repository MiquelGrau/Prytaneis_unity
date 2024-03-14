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

    private void OnMouseDown()
    {
        string startNodeId = "LN0001"; 

        switch (currentMode)
        {
            case WorldSceneInteractionMode.Default:
                PlayerPrefs.SetString("SelectedCity", cityName);
                Debug.Log($"Mode Default: {cityName} seleccionada.");
                // Carrega una altra escena o realitza alguna acció específica del mode Default
                break;

            case WorldSceneInteractionMode.Route:
                Debug.Log($"Mode Route: Creant ruta cap a {cityName}, ID {id}.");
                WorldMapUtils.DijkstraAlgorithm(startNodeId, id, DataManager.worldMapNodes, DataManager.worldMapLandPaths);
                
                // Implementa la funció per a mode Route
                break;

            default:
                Debug.LogWarning("Acció no definida per a aquest mode.");
                break;
        }
    }

}
