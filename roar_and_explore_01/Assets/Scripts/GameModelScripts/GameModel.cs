using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEditor.Sprites;

namespace SafariGame
{
    public class GameModel : MonoBehaviour
    {
        public static GameModel instance;

        void Awake()
        {
            if (instance) { Destroy(gameObject); }
            else { instance = this; }
        }

        [SerializeField] private int difficulty;

        [Header("Time stats")]
        // 0 == pause, 1 == hour, 2 == day, 3 == week
        [SerializeField] private int simulationSpeed;
        [SerializeField] private int daysPassed;
        // in military time (0830 == 08:30 AM)
        [SerializeField] private int timeOfDay;
        [SerializeField] private bool isNightTime;
        [SerializeField] private float tickOfDay;
        [SerializeField] private float ticksPerSecond;

        /*  tick muködése:
         *  (jobb lenne mérni inkább az eszközön eltelt idot, mert az egységesebb)
         *  óra sebesség: 1 perc == 1 óra a játékban (x1)   (3600 tick / 1 real-life perc == 60 tick/sec)
         *  nap sebesség: 1 perc == 1 nap a játékban (x24)  (86400 tick / 1 real-life perc == 1440 tick/sec)
         *  hét sebesség: 1 perc == 1 hét a játékban (x168) (604800 tick / 1 real-life perc == 10080 tick/sec)
         *  
         *  alap sebességnek az óra sebességet véve:
         *  1 játékbeli óra eltelik minden 3600 tick alatt, 1 nap így pl 86400 tick
         */

        [Header("Safari assets")]
        [SerializeField] private int money;
        [SerializeField] private int todaysVisitors;
        [SerializeField] private int waitingVisitors;
        [SerializeField] private int visitorRating;
        [SerializeField] private int herbivoreCount;
        [SerializeField] private int carnivoreCount;
        [SerializeField] private int jeepCount;
        [SerializeField] private int availableJeeps;
        [SerializeField] private int rangerCount;
        [SerializeField] private int ticketPrice;

        [Header("Map generation")]
        private Dictionary<Vector2, GameObject> mapGrid = new Dictionary<Vector2, GameObject>();
        [SerializeField] private GameObject gate;
        [SerializeField] private Sprite entranceTexture;
        [SerializeField] private Sprite exitTexture;
        [SerializeField] private GameObject terrainPrefab;
        [SerializeField] private const int oasisTilesToGenerate = 8;
        [SerializeField] private Sprite oasisSprite;
        [SerializeField] private const int treeTilesToGenerate = 16;
        [SerializeField] private Sprite treeSprite;
        [SerializeField] private const int bushTilesToGenerate = 24;
        [SerializeField] private Sprite bushSprite;
        [SerializeField] private const int grassTilesToGenerate = 32;
        [SerializeField] private Sprite grassSprite;

        [SerializeField] private GameObject Poacher;
        [SerializeField] private GameObject TourManagerObject;

        // win condition variables:
        private int streak;
        private int winTimeframe;
        private int carnivoresToHave;
        private int herbivoresToHave;
        private int visitorsToHave;
        private int moneyToHave;

        public void Start()
        {
            difficulty = PlayerPrefs.GetInt("Difficulty", 0);
            simulationSpeed = 1;

            daysPassed = 0;
            timeOfDay = 0800;
            isNightTime = false;
            tickOfDay = 28800;
            switch (difficulty)
            {
                case 0:
                    money = 10000;
                    break;
                case 1:
                    money = 8000;
                    break;
                case 2:
                    money = 6000;
                    break;
                default:
                    money = 0;
                    break;
            }
            ticketPrice = 20;
            todaysVisitors = 0;
            waitingVisitors = 0;
            visitorRating = 30;
            jeepCount = 0;
            initializeGameArea();
            setWinConditions();
        }

        public int getSimulationSpeed() => simulationSpeed;
        public void setSimulationSpeed(int s) { if (0 <= s && s <= 3) { simulationSpeed = s; } }
        public int getDaysPassed() => daysPassed;
        public int getTimeOfDay() => timeOfDay;
        public bool getNightTime() => isNightTime;

        public int getHerbivoreCount() => herbivoreCount;
        public int getCarnivoreCount() => carnivoreCount;

        public int getMoney() => money;
        public void changeMoney(int amount) { money += amount; }
        public bool canAfford(int amount) => ( money >= amount );

        public int getTicketPrice() => ticketPrice;
        public void increaseTicketPrice() { if (ticketPrice <= 180) ticketPrice += 20; }
        public void decreaseTicketPrice() { if (40  <= ticketPrice) ticketPrice -= 20; }

        public int getWaitingVisitors() => waitingVisitors;
        public void changeTodaysVisitors(int n) { waitingVisitors -= n; todaysVisitors += n; }
        public int getVisitorRating() => visitorRating;
        public void changeVisitorRating(int score) { visitorRating = Mathf.Clamp(visitorRating + score, 0, 100); }

        public int getJeepCount() => jeepCount;
        public int getAvailableJeeps() => availableJeeps;
        public void buyJeep() { if (canAfford(500)) { changeMoney(-500); jeepCount++; availableJeeps++; } }
        public void jeepUsage(bool taking) { availableJeeps += (taking ? -1 : 1); }

        public int getRangerCount() => rangerCount;
        public void addRanger() { if (canAfford(200)) { changeMoney(-200); rangerCount++; } }
        public void removeRanger() { rangerCount--; }

        public Dictionary<Vector2, GameObject> getMapGrid() => mapGrid;

        private Vector2 getRandomTile() => new Vector2(UnityEngine.Random.Range(-19, 19) * 2 + 1, UnityEngine.Random.Range(-2, 17) * 2 + 1);

        private void initializeGameArea()
        {
            // Add entry and exit gates
            GameObject entranceObject = Instantiate(gate, new Vector2(-5, -3), Quaternion.identity);
            entranceObject.GetComponent<SpriteRenderer>().sprite = entranceTexture;
            Resize(entranceObject.GetComponent<SpriteRenderer>(), entranceObject.transform);
            mapGrid.Add(new Vector2(-5, -3), entranceObject);

            GameObject exitObject = Instantiate(gate, new Vector2(5, -3), Quaternion.identity);
            exitObject.GetComponent<SpriteRenderer>().sprite = exitTexture;
            Resize(exitObject.GetComponent<SpriteRenderer>(), exitObject.transform);
            mapGrid.Add(new Vector2(5, -3), exitObject);

            // Generate random objects
            int i = 0;
            while (i < oasisTilesToGenerate)
            {
                Vector2 target = getRandomTile();
                if (!mapGrid.ContainsKey(target))
                {
                    GameObject instancedObject = Instantiate(terrainPrefab, target, Quaternion.identity);
                    instancedObject.GetComponent<SpriteRenderer>().sprite = oasisSprite;
                    Resize(instancedObject.GetComponent<SpriteRenderer>(), instancedObject.transform);
                    mapGrid.Add(target, instancedObject);
                    AnimalManager.Instance.addWater(target);
                    i++;
                }
            }
            i = 0;
            while (i < treeTilesToGenerate)
            {
                Vector2 target = getRandomTile();
                if (!mapGrid.ContainsKey(target))
                {
                    GameObject instancedObject = Instantiate(terrainPrefab, target, Quaternion.identity);
                    instancedObject.GetComponent<SpriteRenderer>().sprite = treeSprite;
                    Resize(instancedObject.GetComponent<SpriteRenderer>(), instancedObject.transform);
                    mapGrid.Add(target, instancedObject);
                    AnimalManager.Instance.addFood(target, 6);
                    i++;
                }
            }
            i = 0;
            while (i < bushTilesToGenerate)
            {
                Vector2 target = getRandomTile();
                if (!mapGrid.ContainsKey(target))
                {
                    GameObject instancedObject = Instantiate(terrainPrefab, target, Quaternion.identity);
                    instancedObject.GetComponent<SpriteRenderer>().sprite = bushSprite;
                    Resize(instancedObject.GetComponent<SpriteRenderer>(), instancedObject.transform);
                    mapGrid.Add(target, instancedObject);
                    AnimalManager.Instance.addFood(target, 4);
                    i++;
                }
            }
            i = 0;
            while (i < grassTilesToGenerate)
            {
                Vector2 target = getRandomTile();
                if (!mapGrid.ContainsKey(target))
                {
                    GameObject instancedObject = Instantiate(terrainPrefab, target, Quaternion.identity);
                    instancedObject.GetComponent<SpriteRenderer>().sprite = grassSprite;
                    Resize(instancedObject.GetComponent<SpriteRenderer>(), instancedObject.transform);
                    mapGrid.Add(target, instancedObject);
                    AnimalManager.Instance.addFood(target, 2);
                    i++;
                }
            }
        }

        private void Resize(SpriteRenderer spriteRenderer, Transform targetTransform, int targetWidth = 200)
        {
            // Get the size of the sprite in pixels
            float spriteWidth = spriteRenderer.sprite.rect.width;
            float spriteHeight = spriteRenderer.sprite.rect.height;

            // Calculate the scaling factor to achieve the target width
            float scaleFactor = targetWidth / spriteWidth;

            // Apply the scaling factor to the target transform
            targetTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }

        private void setWinConditions()
        {
            streak = 0;
            switch (difficulty)
            {
                case 0:
                    winTimeframe = 90;
                    carnivoresToHave = 10;
                    herbivoresToHave = 10;
                    visitorsToHave = 10;
                    moneyToHave = 1000;
                    break;
                case 1:
                    winTimeframe = 180;
                    carnivoresToHave = 20;
                    herbivoresToHave = 20;
                    visitorsToHave = 20;
                    moneyToHave = 2000;
                    break;
                case 2:
                    winTimeframe = 360;
                    carnivoresToHave = 30;
                    herbivoresToHave = 30;
                    visitorsToHave = 30;
                    moneyToHave = 3000;
                    break;
            }
        }

        private bool checkWinConditions()
        {
            return (money >= moneyToHave && todaysVisitors >= visitorsToHave &&
                    herbivoreCount >= herbivoresToHave && carnivoreCount >= carnivoresToHave);
        }
        
        private void passTime()
        {
            // 86400 = 48 * 1800 = 24 * 3600
            int hours = (int)(tickOfDay / 3600);
            int minutes = (tickOfDay % 3600 >= 1800 ? 30 : 0);
            timeOfDay = hours * 100 + minutes;
        }

        private void attemptVisitation()
        {
            // visitors come between 0800 and 2000, if there is at least one jeep and road
            if ((jeepCount == 0) || !(28800 <= tickOfDay && tickOfDay <= 72000) || TourManagerObject.GetComponent<TourManagerScript>().getRoads().Count == 0) { waitingVisitors = 0; return; }

            // average performance brings ~30 visitors a day
            // 43200 ticks for daytime (0800 to 2000) => one visitor every 1440 ticks

            // the worst performance lowers this to ~ 5 visitors a day
            // that's one visitor every 8640 ticks, so a 1/6 multiplier

            // the best performance boosts average visitor count to 50 per day
            // that's one every 864 ticks, so a 10/6 multiplier

            // ticket price doesn't influence visitation chance, but sets expectations for the visitors

            float visitChance = 1f / 1440f;

            if      ( 0 <= visitorRating && visitorRating <   25) { visitChance *= (1 / 6f); } // 0.17x
            else if (25 <= visitorRating && visitorRating <   45) { visitChance *= (5 / 9f); } // 0.56x
            else if (45 <= visitorRating && visitorRating <   60) { visitChance *= (1 / 1f); } // 1.00x
            else if (60 <= visitorRating && visitorRating <   80) { visitChance *= (4 / 3f); } // 1.33x
            else if (80 <= visitorRating && visitorRating <= 100) { visitChance *= (5 / 3f); } // 1.67x

            // take simulation speed into account: 24x and 168x (also balance x1 for some reason)
            if      (simulationSpeed == 1) { visitChance *=  0.33f; }
            else if (simulationSpeed == 2) { visitChance *=  6.00f; }
            else if (simulationSpeed == 3) { visitChance *= 42.00f; }

            if (UnityEngine.Random.value < visitChance) { waitingVisitors++; }
        }

        public void Update()
        {
            switch (simulationSpeed)
            {
                case 0:
                    ticksPerSecond = 0;
                    break;
                case 1:
                    ticksPerSecond = 60;
                    break;
                case 2:
                    ticksPerSecond = 1440;
                    break;
                case 3:
                    ticksPerSecond = 10080;
                    break;
            }
            if (simulationSpeed > 0)
            {
                tickOfDay += ticksPerSecond * Time.deltaTime;
                passTime();
                attemptVisitation();
                herbivoreCount = AnimalManager.Instance.getZebras().Count + AnimalManager.Instance.getGiraffes().Count;
                carnivoreCount = AnimalManager.Instance.getLions().Count + AnimalManager.Instance.getHyenas().Count;

                bool prevNightTime = isNightTime;
                // 0600 és 2100 között
                isNightTime = !(21600 <= tickOfDay && tickOfDay <= 75600);
                if (tickOfDay >= 86400) { EndOfDay(); }
                if(prevNightTime != isNightTime)
                {
                    if(isNightTime)
                    {  
                        WhenTheNightStarts(daysPassed);
                    }
                    else
                    {
                        WhenTheDayStarts();
                    }
                }
            }
        }

        private void EndOfDay()
        {
            tickOfDay = 0;
            timeOfDay = 0;
            daysPassed++;
            if (daysPassed % 28 == 0) { if (canAfford(250 * rangerCount)) { changeMoney(-250 * rangerCount); } else { GameOver("You went bankrupt! Better luck next time!"); } }
            if (checkWinConditions())
            {
                streak++;
                if (streak >= winTimeframe)
                {
                    string s = "Congratulations! You have won the game!";
                    GameOver(s);
                }
            }
            else { streak = 0; }
            todaysVisitors = 0;
        }

        public void GameOver(string message)
        {
            GameOverScript.instance.SendGameOverText(message);
        }

        public void BuyAnimal(int price) { changeMoney(-price); }
        public void BuyTerrain(GameObject terrainInstance, Vector2 position, int price)
        {
            mapGrid.Add(position, terrainInstance);
            changeMoney(-price);
        }
        public void RemoveTerrain(Vector2 position)
        {
            Destroy(mapGrid[position]);
            mapGrid.Remove(position);
        }

        public void WhenTheNightStarts(int xthday)
        {
            for (int i = 0; i < (xthday / 4); i++)
            {
                Vector2 spawnLocation = getRandomTile();
                GameObject instancedPoacher = Instantiate(Poacher, spawnLocation, Quaternion.identity);
                Debug.Log("Poacher Spawned at " + instancedPoacher.transform.position);
            }

        }
        
        public void WhenTheDayStarts()
        {
            var poachers = GameObject.FindGameObjectsWithTag("Poacher");
            foreach(var poacher in poachers)
            {
                Debug.Log("morning => destroying poacher");
                Destroy(poacher);
            }
        }

        public GameObject isRangerSelected(Vector3 position)
        {
            var rangerfind = GameObject.FindGameObjectsWithTag("Ranger");
            foreach(var r in rangerfind)
            {
                if (r.GetComponent<RangerMovement>().isSelected)
                {
                    r.GetComponent<RangerMovement>().target = position;
                    return r;
                }
            }

            return null;
        }
    }
}