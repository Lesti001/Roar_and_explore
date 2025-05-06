using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using SafariGame;

public class TourManagerScript : MonoBehaviour
{
    public static TourManagerScript instance;

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

    [Header("Constants")]
    [SerializeField] private Vector2 entrancePosition = new Vector2(-5, -3);
    [SerializeField] private Vector2 exitPosition = new Vector2(5, -3);

    [Header("UI elements to disable")]
    [SerializeField] private GameObject SidePanel;
    [SerializeField] private GameObject VisibilityButton;
    [SerializeField] private GameObject GridButton;
    [SerializeField] private GameObject Hotbar;
    [SerializeField] private GameObject ShopButton;

    [Header("Jeep Prefab")]
    [SerializeField] private GameObject JeepPrefab;

    private List<List<Vector2>> roads;
    public List<List<Vector2>> getRoads() => roads;
    private List<Vector2> currentRoad;
    private bool currentlyBuilding;

    private GameObject currentRoadParent;

    private int tempSimSpeed;
    
    private void Start()
    {
        roads = new List<List<Vector2>>();
        currentlyBuilding = false;
        currentRoad = new List<Vector2>();
    }

    private void Update()
    {
        if (currentlyBuilding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (IsInBounds(clickPosition))
                {
                    Vector2 segmentStart = currentRoad.Last();
                    if (ClickedOnExit(clickPosition)) { FinishRoadBuilding(); }
                    else
                    {
                        currentRoad.Add(clickPosition);
                        DrawRoad(segmentStart, currentRoad.Last());
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                CancelRoadBuilding();
            }
        }

        if (GameModel.instance.getWaitingVisitors() > 0 && GameModel.instance.getAvailableJeeps() > 0)
        {
            int passengersToTake = Math.Min(4, GameModel.instance.getWaitingVisitors());
            int currentTicketPrice = GameModel.instance.getTicketPrice();
            StartJeepTour(passengersToTake, currentTicketPrice);
        }
    }

    private void StartJeepTour(int passengers, int ticketPrice)
    {
        // removes waiting visitors, collects ticket money
        GameModel.instance.changeTodaysVisitors(passengers);
        GameModel.instance.changeMoney(passengers * ticketPrice);
        GameModel.instance.jeepUsage(true);
        // pick a random path
        List<Vector2> randomPath = roads[UnityEngine.Random.Range(0, roads.Count)];
        // instantiate the jeep
        GameObject spawnedJeep = Instantiate(JeepPrefab, entrancePosition, Quaternion.identity);
        // send the picked path, passenger count and ticket price to the jeep
        spawnedJeep.GetComponent<JeepScript>().SetParameters(randomPath, passengers, ticketPrice);
    }

    private bool IsInBounds(Vector2 target) => (-37 <= target.x && target.x <= 37 && -3 <= target.y && target.y <= 33);

    private bool ClickedOnExit(Vector2 target)
    {
        if (Math.Abs(Math.Floor(target.x) % 2) == 1) { target.x = (float)Math.Floor(target.x); }
        else if (Math.Abs(Math.Ceiling(target.x) % 2) == 1) { target.x = (float)Math.Ceiling(target.x); }
        else { target.x += 1; }
        if (Math.Abs(Math.Floor(target.y) % 2) == 1) { target.y = (float)Math.Floor(target.y); }
        else if (Math.Abs(Math.Ceiling(target.y) % 2) == 1) { target.y = (float)Math.Ceiling(target.y); }
        else { target.y += 1; }

        return (target == exitPosition);
    }

    private void CancelRoadBuilding(bool success = false)
    {
        currentlyBuilding = false;
        currentRoad = new List<Vector2>();
        GameModel.instance.setSimulationSpeed(tempSimSpeed);
        ChangeUI(true);
        if (!success) { Destroy(currentRoadParent); }
    }

    private void FinishRoadBuilding()
    {
        // draw the last segment of road
        Vector2 segmentStart = currentRoad.Last();
        currentRoad.Add(exitPosition);
        DrawRoad(segmentStart, exitPosition);

        // add the built road and clear the temporary one
        GameModel.instance.changeMoney(-500);
        roads.Add(currentRoad);
        CancelRoadBuilding(true);
    }

    public void BuildRoad()
    {
        if (GameModel.instance.canAfford(500))
        {
            // stop time
            tempSimSpeed = GameModel.instance.getSimulationSpeed();
            GameModel.instance.setSimulationSpeed(0);
            currentlyBuilding = true;

            // create parent element for the road lines
            currentRoadParent = new GameObject("RoadParent");

            // road starts from the entrance
            currentRoad.Add(entrancePosition);

            ChangeUI(false);
        }
    }

    private void DrawRoad(Vector2 startPoint, Vector2 endPoint)
    {
        GameObject lineObj = new GameObject("RoadLine");
        lineObj.transform.SetParent(currentRoadParent.transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.startWidth = (0.5f);
        lr.endWidth = (0.5f);
        lr.startColor = (new Color(201 / 255f, 186 / 255f, 143 / 255f, 1f));
        lr.endColor = (new Color(201 / 255f, 186 / 255f, 143 / 255f, 1f));
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0));
        lr.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0));
    }

    private void ChangeUI(bool state)
    {
        SidePanel.SetActive(state);
        VisibilityButton.SetActive(state);
        GridButton.SetActive(state);
        Hotbar.SetActive(state);
        ShopButton.SetActive(state);
    }
}