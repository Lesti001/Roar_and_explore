using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SafariGame;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;
using System;
using Unity.VisualScripting;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance;
    public Animal selectedAnimalForChip;
    public GameObject button;
    private List<GameObject> allAnimals = new List<GameObject>();
    private List<GameObject> Lions = new List<GameObject>();
    private List<GameObject> Zebras = new List<GameObject>();
    private List<GameObject> Giraffes = new List<GameObject>();
    private List<GameObject> Hyenas = new List<GameObject>();
    public List<GameObject> Rangers = new List<GameObject>();
    private List<(Vector2 position, int health)> FoodSources = new List<(Vector2, int)>();
    private List<Vector2> WaterSources = new List<Vector2>();
    private List<GameObject> Jeeps = new List<GameObject>();
    public GameObject animalSpawn;


    public List<Vector2> getWaterSources() => WaterSources;
    public List<(Vector2, int)> getFoodSources() => FoodSources;
    public List<GameObject> getAllAnimals() => allAnimals;
    public List<GameObject> getChipedAnimals() 
    {
        List<GameObject> a = new List<GameObject>();
        foreach (var item in Zebras)
        {
            if (item.GetComponent<Zebra>().GetIsChiped())
            {
                a.Add(item);
            }
        }
        foreach (var item in Lions)
        {
            if (item.GetComponent<Lion>().GetIsChiped())
            {
                a.Add(item);
            }
        }
        foreach (var item in Giraffes)
        {
            if (item.GetComponent<Giraffe>().GetIsChiped())
            {
                a.Add(item);
            }
        }
        foreach (var item in Hyenas)
        {
            if (item.GetComponent<Hyena>().GetIsChiped())
            {
                a.Add(item);
            }
        }
        return a;
    }
    public List<GameObject> getZebras() => Zebras;
    public List<GameObject> getGiraffes() => Giraffes;
    public List<GameObject> getLions() => Lions;
    public List<GameObject> getHyenas() => Hyenas;
    public List<GameObject> getRangers() => Rangers;
    public List<GameObject> GetJeeps() => Jeeps;
    public void RemoveJeep(GameObject j) 
    {
        if (Jeeps.Contains(j))
        {
            Jeeps.Remove(j);
        }
    }
    public void AddJeep(GameObject j) { Jeeps.Add(j); }

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
    public void poacherKillsAnimal(GameObject a) 
    {
        if (allAnimals.Contains(a))
        {
            allAnimals.Remove(a);
            if (allAnimals.Count == 0)
            {
                GameModel.instance.GameOver("All of the animals have died! Better luck next time!");
            }
        }
        if (Lions.Contains(a))
        {
            Lions.Remove(a);
            Destroy(a);
        }
        if (Giraffes.Contains(a))
        {
            Giraffes.Remove(a);
            Destroy(a);
        }
        if (Hyenas.Contains(a))
        {
            Hyenas.Remove(a);
            Destroy(a);
        }
        if (Zebras.Contains(a))
        {
            Zebras.Remove(a);
            Destroy(a);
        }
        DecidePackLeader();
    }
    public void animalDies(GameObject a)
    {
        if (allAnimals.Contains(a))
        {
            allAnimals.Remove(a);
            if (allAnimals.Count == 0)
            {
                GameModel.instance.GameOver("All of the animals have died! Better luck next time!");
            }
        }
        if (Lions.Contains(a))
        {
            Lions.Remove(a);
        }
        else if (Hyenas.Contains(a))
        {
            Hyenas.Remove(a);
        }
        else if (Zebras.Contains(a))
        {
            Zebras.Remove(a);
        }
        else if (Giraffes.Contains(a))
        {
            Giraffes.Remove(a);
        }
        else
        {
            UnityEngine.Debug.Log("DIDNT FIND");
        }
        DecidePackLeader();
    }
    public void addAnimal(Animal l)
    {
        if (!allAnimals.Contains(l.gameObject))
        {
            allAnimals.Add(l.gameObject);
            if (l is Lion)
            {
                Lions.Add(l.gameObject);
            }
            else if (l is Hyena)
            {
                Hyenas.Add(l.gameObject);
            }
            else if (l is Giraffe)
            {
                Giraffes.Add(l.gameObject);
            }
            else if (l is Zebra)
            {
                Zebras.Add(l.gameObject);
            }
        }
        DecidePackLeader();
    }
    public void AddRanger(GameObject r) 
    {
        if (!Rangers.Contains(r))
        {
            Rangers.Add(r);
        }
    }
    public void RemoveRanger(GameObject r) 
    {
        if (Rangers.Contains(r))
        {
            Rangers.Remove(r);
        }
    }
    public void addFood(Vector2 f, int health)
    {
        if (!FoodSources.Any(fs => fs.position == f))
        {
            FoodSources.Add((f, health));
        }
    }
    public bool consumeFood(Vector2 f)
    {
        bool success = false;
        for (int i = 0; i < FoodSources.Count; i++)
        {
            if (FoodSources[i].position == f)
            {
                var plant = FoodSources[i];
                if (plant.health > 0) { success = true; }
                plant.health--;

                if (plant.health <= 0)
                {
                    FoodSources.RemoveAt(i);
                    GameModel.instance.RemoveTerrain(plant.position);
                }
                else
                {
                    FoodSources[i] = plant;
                }
                break;
            }
        }
        return success;
    }
    public void addWater(Vector2 w)
    {
        if (!WaterSources.Contains(w))
        {
            WaterSources.Add(w);
        }
    }
    public Vector2 getPackLeaderPosition(Animal a)
    {
        if (a is Lion)
        {
            foreach (var item in Lions)
            {
                if (item.GetComponent<Lion>().getIsPackLeader())
                {
                    return item.transform.position;
                }
            }
        }
        if (a is Zebra)
        {
            foreach (var item in Zebras)
            {
                if (item.GetComponent<Zebra>().getIsPackLeader())
                {
                    return item.transform.position;
                }
            }
        }
        if (a is Giraffe)
        {
            foreach (var item in Giraffes)
            {
                if (item.GetComponent<Giraffe>().getIsPackLeader())
                {
                    return item.transform.position;
                }
            }
        }
        if (a is Hyena)
        {
            foreach (var item in Hyenas)
            {
                if (item.GetComponent<Hyena>().getIsPackLeader())
                {
                    return item.transform.position;
                }
            }
        }
        return new Vector2();
    }
    public void DecidePackLeader()
    {
        if (Lions.Count > 0)
        {
            GameObject oldest = Lions[0];
            foreach (var item in Lions)
            {
                if (item.GetComponent<Lion>().getAge() > oldest.GetComponent<Lion>().getAge())
                {
                    oldest = item;
                }
            }
            oldest.GetComponent<Lion>().setPackLeader(true);
        }
        if (Zebras.Count > 0)
        {
            GameObject oldest = Zebras[0];
            foreach (var item in Zebras)
            {
                if (item.GetComponent<Zebra>().getAge() > oldest.GetComponent<Zebra>().getAge())
                {
                    oldest = item;
                }
            }
            oldest.GetComponent<Zebra>().setPackLeader(true);
        }
        if (Hyenas.Count > 0)
        {
            GameObject oldest = Hyenas[0];
            foreach (var item in Hyenas)
            {
                if (item.GetComponent<Hyena>().getAge() > oldest.GetComponent<Hyena>().getAge())
                {
                    oldest = item;
                }
            }
            oldest.GetComponent<Hyena>().setPackLeader(true);
        }
        if (Giraffes.Count > 0)
        {
            GameObject oldest = Giraffes[0];
            foreach (var item in Giraffes)
            {
                if (item.GetComponent<Giraffe>().getAge() > oldest.GetComponent<Giraffe>().getAge())
                {
                    oldest = item;
                }
            }
            oldest.GetComponent<Giraffe>().setPackLeader(true);
        }
    }
    public void breed()
    {
        foreach (var item in Lions)
        {
            if (item.GetComponent<Lion>().getLust() > 5)
            {
                foreach (var other in Lions)
                {
                    if (other != item && other.GetComponent<Lion>().getLust() > 5)
                    {
                        UnityEngine.Debug.Log("StartBreeding");
                        item.GetComponent<Lion>().setLust(0);
                        other.GetComponent<Lion>().setLust(0);
                        GameObject sanimal = Instantiate(animalSpawn, other.transform.position, Quaternion.identity);
                        sanimal.AddComponent<Lion>();
                        SpriteRenderer spriteRenderer = sanimal.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/LionSprite");
                        Resize(spriteRenderer, sanimal.transform);  
                    }
                }
            }
        }

        foreach (var item in Hyenas)
        {
            if (item.GetComponent<Hyena>().getLust() > 5)
            {
                foreach (var other in Hyenas)
                {
                    if (other != item && other.GetComponent<Hyena>().getLust() > 5)
                    {
                        UnityEngine.Debug.Log("StartBreeding");
                        item.GetComponent<Hyena>().setLust(0);
                        other.GetComponent<Hyena>().setLust(0);
                        GameObject sanimal = Instantiate(animalSpawn, other.transform.position, Quaternion.identity);
                        sanimal.AddComponent<Hyena>();
                        SpriteRenderer spriteRenderer = sanimal.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/HyenaSprite");
                        Resize(spriteRenderer, sanimal.transform);
                    }
                }
            }
        }

        foreach (var item in Giraffes)
        {
            if (item.GetComponent<Giraffe>().getLust() > 5)
            {
                foreach (var other in Giraffes)
                {
                    if (other != item && other.GetComponent<Giraffe>().getLust() > 5)
                    {
                        UnityEngine.Debug.Log("StartBreeding");
                        item.GetComponent<Giraffe>().setLust(0);
                        other.GetComponent<Giraffe>().setLust(0);
                        GameObject sanimal = Instantiate(animalSpawn, other.transform.position, Quaternion.identity);
                        sanimal.AddComponent<Giraffe>();
                        SpriteRenderer spriteRenderer = sanimal.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/GiraffeSprite");
                        Resize(spriteRenderer, sanimal.transform);
                    }
                }
            }
        }

        foreach (var item in Zebras)
        {
            if (item.GetComponent<Zebra>().getLust() > 5)
            {
                foreach (var other in Zebras)
                {
                    if (other != item && other.GetComponent<Zebra>().getLust() > 5)
                    {
                        UnityEngine.Debug.Log("StartBreeding");
                        item.GetComponent<Zebra>().setLust(0);
                        other.GetComponent<Zebra>().setLust(0);
                        GameObject sanimal = Instantiate(animalSpawn, other.transform.position, Quaternion.identity);
                        sanimal.AddComponent<Zebra>();
                        SpriteRenderer spriteRenderer = sanimal.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/ZebraSprite");
                        Resize(spriteRenderer, sanimal.transform);
                    }
                }
            }
        }
    }
    public void feedLions() 
    {
        foreach (var item in Lions)
        {
            item.GetComponent<Lion>().feeedAnimal();
        }
    }
    public void feedHyenas() 
    {
        foreach (var item in Hyenas)
        {
            item.GetComponent<Hyena>().feeedAnimal();
        }
    }

    private int time = 0;
    private void Resize(SpriteRenderer spriteRenderer, Transform targetTransform, int targetWidth = 200)
    {
        float spriteWidth = spriteRenderer.sprite.rect.width;
        float spriteHeight = spriteRenderer.sprite.rect.height;
        float scaleFactor = targetWidth / spriteWidth;
        targetTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
    public void Update()
    {
        if (button.activeSelf)
        {
            time++;
            if (time > 2000)
            {
                button.SetActive(false);
            }
        }
        else
        {
            time = 0;
        }
        breed();
    }
    public void Start()
    {
        UnityEngine.UI.Button b = button.GetComponent<UnityEngine.UI.Button>();
        b.onClick.AddListener(OnButtonClicked);
    }
    public void animalIsClicked(Animal a)
    {
        selectedAnimalForChip = a;
        if (selectedAnimalForChip == null)
        {
            button.SetActive(false);
        }
        else
        {
            button.SetActive(true); 
        }
        
    }
    void OnButtonClicked() 
    {
        if (GameModel.instance.canAfford(200))
        {
            selectedAnimalForChip.GettingChip();
            GameModel.instance.changeMoney(-200);
            selectedAnimalForChip.transform.Find("mask").gameObject.SetActive(true);
        }
        button.SetActive(false);
        selectedAnimalForChip = null;
    }
}