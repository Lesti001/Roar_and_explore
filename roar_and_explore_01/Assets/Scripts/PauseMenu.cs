using System.Diagnostics;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using SafariGame;

public class PauseMenu : MonoBehaviour
{
    [Header("Scene camera for toggling panning")]
    public GameObject mainCamera;

    [Header("PauseMenu objects")]
    public GameObject pausePanel;
    public GameObject blurEffect;
    public GameObject hotbar;
    public GameObject shopButton;
    public GameObject shopPanel;
    public static bool IsPaused;

    private int simulationSpeed;
    
    public void Start() 
    {
        IsPaused = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape();
        }
    }

    public void OnEscape()
    {
        if (IsPaused)   // Resuming game
        {
            // Deactivating blur and pause menu
            pausePanel.SetActive(false);
            blurEffect.SetActive(false);

            // Activating UI elements
            shopButton.SetActive(true);
            hotbar.SetActive(true);

            GameModel.instance.setSimulationSpeed(simulationSpeed);
        }
        else           // Pausing game
        {
            simulationSpeed = GameModel.instance.getSimulationSpeed();
            GameModel.instance.setSimulationSpeed(0);

            // Activating blur and pause menu
            pausePanel.SetActive(true);
            blurEffect.SetActive(true);

            // Deactivating UI elements
            shopPanel.SetActive(false);
            shopButton.SetActive(false);
            hotbar.SetActive(false);
        }
        IsPaused = !IsPaused;
        mainCamera.GetComponent<CameraMovement>().allowPanning = !IsPaused;
    }
    public void ExitButton()
    {
        UnityEngine.Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
