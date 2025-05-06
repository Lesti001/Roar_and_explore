using UnityEngine;
using TMPro;
using SafariGame;

public class GameOverScript : MonoBehaviour
{
    public static GameOverScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject reason;

    public void SendGameOverText(string text)
    {
        GameModel.instance.setSimulationSpeed(0);
        mainCamera.GetComponent<CameraMovement>().allowPanning = false;
        gameOverPanel.SetActive(true);
        TextMeshProUGUI reasonText = reason.GetComponent<TextMeshProUGUI>();
        reasonText.text = text;
    }
}