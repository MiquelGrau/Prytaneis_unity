using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Marker : MonoBehaviour
{
    public GameObject contextMenuPrefab;
    public string cityName;
    public Vector3 position;
    private GameObject contextMenuInstance;

    private void OnMouseOver()
    {
    }

    private void OnMouseDown()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "WorldScene":
                PlayerPrefs.SetString("SelectedCity", cityName);
                SceneManager.LoadScene("CityScene");
                break;

            case "RouteScene":
                // Ací posa la nova funció que vols realitzar quan estàs a RouteScene
                break;

            default:
                Debug.LogWarning("Acció no definida per a aquesta escena.");
                break;
        }
    }
}
