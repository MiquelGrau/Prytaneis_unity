using UnityEngine;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager Instance;

    // Actualitzem el delegat per utilitzar l'enum WorldSceneInteractionMode
    public delegate void ModeChangeAction(WorldSceneInteractionMode newMode);
    public event ModeChangeAction OnModeChange;

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
    }

    public void SetRouteMode() {
        ChangeMode(WorldSceneInteractionMode.Route);
    }

}
