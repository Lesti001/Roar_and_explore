using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject easyButton;
    public GameObject mediumButton;
    public GameObject hardButton;


    public void StartGameEasy() 
    {
        PlayerPrefs.SetInt("Difficulty", 0);
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("GameScene");
    }
    public void StartGameMedium() 
    {
        PlayerPrefs.SetInt("Difficulty", 1);
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("GameScene");
    }
    public void StartGameHard() 
    {
        PlayerPrefs.SetInt("Difficulty", 2);
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("GameScene");
    }
    public void PlayGame() 
    {        
        easyButton.SetActive(true);
        mediumButton.SetActive(true);
        hardButton.SetActive(true);
    }
    public void ExitGameButtonClick() 
    {
        Application.Quit();
        // If running in the Unity Editor, stop play mode
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        easyButton.SetActive(false);
        mediumButton.SetActive(false);
        hardButton.SetActive(false);
    } 
}