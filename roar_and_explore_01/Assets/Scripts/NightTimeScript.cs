using UnityEngine;
using UnityEngine.UI;
using SafariGame;

public class NightTimeScript : MonoBehaviour
{
    [SerializeField] private GameObject Fog;
    void Update()
    {
        Fog.SetActive(GameModel.instance.getNightTime());
        Fog.GetComponent<SpriteRenderer>().color = new Color(96 / 255f, 96 / 255f, 96 / 255f, GetFogColor());
        transform.GetComponent<Image>().color = new Color(0f, 20 / 255f, 60 / 255f, GetSkyColor() / 255f);
    }

    private float GetFogColor()
    {
        int time = GameModel.instance.getTimeOfDay();
        if (0500 <= time && time < 0530) return 0.75f;
        if (0530 <= time && time < 0600) return 0.5f;
        if (2100 <= time && time < 2130) return 0.5f;
        if (2130 <= time && time < 2200) return 0.75f;
        return 1f;
    }

    private int GetSkyColor()
    {
        int time = GameModel.instance.getTimeOfDay();

        if (0000 <= time && time < 0600) return 150;
        if (0600 <= time && time < 0630) return 100;
        if (0630 <= time && time < 0700) return 50;
        if (2000 <= time && time < 2030) return 50;
        if (2030 <= time && time < 2100) return 100;
        if (2100 <= time && time < 2401) return 150;
        return 0;
    }
}