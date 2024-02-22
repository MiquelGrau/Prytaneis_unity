using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager Instance;

    // Actualitzem el delegat per utilitzar l'enum WorldSceneInteractionMode
    public delegate void ModeChangeAction(WorldSceneInteractionMode newMode);
    public event ModeChangeAction OnModeChange;
    private string routeSceneName = "RoutesScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMode(WorldSceneInteractionMode newMode)
    {
        OnModeChange?.Invoke(newMode);
    }

    public void SetDefaultMode() {
        ChangeMode(WorldSceneInteractionMode.Default);
        UnloadRouteScene();
    }

    public void SetRouteMode() {
        ChangeMode(WorldSceneInteractionMode.Route);
        LoadRouteScene();
    }

    public void LoadRouteScene()
    {
        // Comprova si l'escena ja està carregada
        if (SceneManager.GetSceneByName(routeSceneName).isLoaded)
        {
            Debug.Log($"{routeSceneName} ja està carregada.");
            return;
        }

        // Carrega l'escena additivament
        SceneManager.LoadScene(routeSceneName, LoadSceneMode.Additive);
        Debug.Log($"Carregant {routeSceneName} additivament.");
    }

    // Opcional: Mètode per descarregar RouteScene
    public void UnloadRouteScene()
    {
        if (SceneManager.GetSceneByName(routeSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(routeSceneName);
            Debug.Log($"Descarregant {routeSceneName}.");
        }
    }

}
