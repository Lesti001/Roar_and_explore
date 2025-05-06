using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SafariGame;

public class HotbarScript : MonoBehaviour
{
    private GameObject dayPhase;
    private GameObject nightPhase;
    private TextMeshProUGUI timeText;

    private GameObject pauseButton;
    private GameObject x1Button;
    private GameObject x2Button;
    private GameObject x3Button;

    private void resetButtonColors()
    {
        pauseButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        x1Button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        x2Button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        x3Button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
    }

    public void Start()
    {
        dayPhase    = transform.Find("DayPhaseImage").gameObject;
        nightPhase  = transform.Find("NightPhaseImage").gameObject;
        timeText    = transform.Find("Time").GetComponent<TextMeshProUGUI>();
        pauseButton = transform.Find("PauseButton").gameObject;
        x1Button    = transform.Find("ButtonX1").gameObject;
        x2Button    = transform.Find("ButtonX2").gameObject;
        x3Button    = transform.Find("ButtonX3").gameObject;
    }

    private string createTimeFormat(int t)
    {
        int hour   = t / 100; 
        int minute = t % 100;
        return (hour < 10 ? "0" : "") + hour + ":" + (minute < 10 ? "0" : "") + minute;
    }

    public void pauseButtonPressed()
    {
        GameModel.instance.setSimulationSpeed(0);
        resetButtonColors();
        pauseButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 120f / 255f);
    }
    public void x1ButtonPressed()
    {
        GameModel.instance.setSimulationSpeed(1);
        resetButtonColors();
        x1Button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 120f / 255f);
    }
    public void x2ButtonPressed()
    {
        GameModel.instance.setSimulationSpeed(2);
        resetButtonColors();
        x2Button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 120f / 255f);
    }
    public void x3ButtonPressed()
    {
        GameModel.instance.setSimulationSpeed(3);
        resetButtonColors();
        x3Button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 120f / 255f);
    }

    public void Update()
    {
        // set sun/moon icon accordingly:
        dayPhase.SetActive(!GameModel.instance.getNightTime());
        nightPhase.SetActive(GameModel.instance.getNightTime());

        // write the time:
        int gameTime = GameModel.instance.getTimeOfDay();
        timeText.GetComponent<TMP_Text>().text = createTimeFormat(gameTime);
    }
}
