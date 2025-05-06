using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SafariGame;

public class SidePanel : MonoBehaviour
{
    private bool visible;
    private static GameObject sidePanel;
    private TextMeshProUGUI visibilityText;

    private static Transform statsSection1;
    private static Transform statsSection2;
    private static Transform statsSection3;

    public GameObject rangerPrefab; //j

    void Start()
    {
        visible = true;
        sidePanel = transform.gameObject;
        visibilityText = GameObject.Find("VisibilityButton").transform.Find("VisibilityButtonText").GetComponent<TextMeshProUGUI>();

        statsSection1 = transform.Find("StatsSection1");
        statsSection2 = transform.Find("StatsSection2");
        statsSection3 = transform.Find("StatsSection3");
    }

    public void ToggleVisibility()
    {
        visible = !visible;
        sidePanel.SetActive(visible);
        visibilityText.text = visible ? "<" : ">";
    }

    // TODO
    public void HireNewRanger()
    {
        GameModel.instance.addRanger();
        Instantiate(rangerPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), Quaternion.identity);
    }

    public void PurchaseJeep()
    {
        GameModel.instance.buyJeep();
    }

    public void IncreaseTicketPrice()
    {
        GameModel.instance.increaseTicketPrice();
    }

    public void DecreaseTicketPrice()
    {
        GameModel.instance.decreaseTicketPrice();
    }

    // helper function to format money:
    public static string separateNumber(int n)
    {
        string str = n.ToString();
        StringBuilder result = new StringBuilder();
        int length = str.Length;
        int counter = 0;

        for (int i = length - 1; i >= 0; i--)
        {
            result.Insert(0, str[i]);
            counter++;

            if (counter % 3 == 0 && i != 0)  result.Insert(0, ' ');
        }

        return result.ToString();
    }

    void Update()
    {
        // Update stats:
        statsSection1.Find("MoneyStat").Find("MoneyText").GetComponent<TMP_Text>().text =
             separateNumber(GameModel.instance.getMoney()) + " $";
        statsSection1.Find("DateStat").Find("DateText").GetComponent<TMP_Text>().text =
            "day " + (GameModel.instance.getDaysPassed() + 1).ToString();
        statsSection2.Find("HerbivoreStat").Find("HerbivoreCount").GetComponent<TMP_Text>().text =
            "x " + GameModel.instance.getHerbivoreCount().ToString();
        statsSection2.Find("CarnivoreStat").Find("CarnivoreCount").GetComponent<TMP_Text>().text =
            "x " + GameModel.instance.getCarnivoreCount().ToString();
        statsSection3.Find("VisitorStat").Find("VisitorCount").GetComponent<TMP_Text>().text =
            "x " + GameModel.instance.getWaitingVisitors().ToString();
        statsSection3.Find("RatingStat").Find("RatingText").GetComponent<TMP_Text>().text =
            GameModel.instance.getVisitorRating().ToString() + " %";
        statsSection3.Find("VehicleStat").Find("VehicleCount").GetComponent<TMP_Text>().text =
            "x " + GameModel.instance.getJeepCount().ToString();
        statsSection3.Find("RangerStat").Find("RangerCount").GetComponent<TMP_Text>().text =
            "x " + GameModel.instance.getRangerCount().ToString();
        statsSection3.Find("TicketStat").Find("TicketPrice").GetComponent<TMP_Text>().text =
            separateNumber(GameModel.instance.getTicketPrice()) + " $";
    }
}