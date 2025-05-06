using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using SafariGame;
using Unity.VisualScripting;

public class ShopManager : MonoBehaviour
{
    [Header("Game area borders")]
    [SerializeField] private const float x_lowerBound = -37f;
    [SerializeField] private const float x_upperBound =  37f;
    [SerializeField] private const float y_lowerBound =  -3f;
    [SerializeField] private const float y_upperBound =  33f;


    [Header("Scene camera for toggling panning")]
    public GameObject mainCamera;

    [Header("ShopManager objects")]
    public Texture2D selectionCursor;
    public GameObject animalPanel;
    public GameObject terrainPanel;
    public List<PurchasableObject> animalList;
    public List<PurchasableObject> terrainList;
    public GameObject AnimalUIPrefab;
    public GameObject TerrainUIPrefab;

    public GameObject animalSpawn;
    public GameObject terrainSpawn;
    public GameObject shopPanel;

    private PurchasableObject selectedAnimal;
    private PurchasableObject selectedTerrain;

    public void GenUI()
    {
        int herbCount = 0, carnCount = 0;
        foreach (PurchasableObject p in animalList)
        {
            int columnShift, rowShift;
            if (p.type == Type.HERBIVORE)
            {
                columnShift = 0;
                rowShift = herbCount;
                herbCount++;
            }
            else
            {
                columnShift = 400;
                rowShift = carnCount;
                carnCount++;
            }

            GameObject instancedAnimalUI = Instantiate(AnimalUIPrefab, animalPanel.transform);
            instancedAnimalUI.GetComponent<RectTransform>().anchoredPosition += new Vector2(columnShift, -170f * rowShift - 50);
            instancedAnimalUI.transform.Find("Image").GetComponent<Image>().sprite = p.image;
            instancedAnimalUI.transform.Find("AnimalName").GetComponent<TMP_Text>().text = p.name;
            instancedAnimalUI.transform.Find("AnimalPrice").GetComponent<TMP_Text>().text = p.price.ToString() + " $";
            instancedAnimalUI.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(() => SelectAnimal(p));
        }

        int i = 0;
        foreach (PurchasableObject p in terrainList)
        {
            int columnShift = 0, rowShift;
            rowShift = i % 3;
            if (i > 2) columnShift = 400;
            GameObject instancedTerrainUI = Instantiate(TerrainUIPrefab, terrainPanel.transform);
            instancedTerrainUI.GetComponent<RectTransform>().anchoredPosition += new Vector2(columnShift, -170f * rowShift - 50);
            instancedTerrainUI.transform.Find("Image").GetComponent<Image>().sprite = p.image;
            instancedTerrainUI.transform.Find("TerrainName").GetComponent<TMP_Text>().text = p.name;
            instancedTerrainUI.transform.Find("TerrainPrice").GetComponent<TMP_Text>().text = p.price.ToString() + " $";
            instancedTerrainUI.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(() => SelectTerrain(p));
            i++;
        }
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        mainCamera.GetComponent<CameraMovement>().allowPanning = false;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        mainCamera.GetComponent<CameraMovement>().allowPanning = true;
    }

    public void OpenAnimals()
    {
        animalPanel.SetActive(true);
        terrainPanel.SetActive(false);
    }

    public void OpenTerrain()
    {
        terrainPanel.SetActive(true);
        animalPanel.SetActive(false);
    }

    public void SelectAnimal(PurchasableObject pur)
    {
        if (GameModel.instance.canAfford(pur.price))
        {
            selectedAnimal = pur;

            CloseShop();

            Cursor.SetCursor(selectionCursor, Vector2.zero, CursorMode.Auto);

            // Debug.Log("Ready to place: " + pur.name);
        }
        else
        {
            // Debug.Log("Not enough money to buy: " + pur.name);
        }
    }

    private void PlaceAnimal(Vector2 position)
    {
        // Instantiate the animal prefab
        GameObject sanimal = Instantiate(animalSpawn, position, Quaternion.identity);

        SpriteRenderer spriteRenderer = sanimal.GetComponent<SpriteRenderer>();

        // Set the sprite to the selected animal's image
        spriteRenderer.sprite = selectedAnimal.image;

        // Resize the sprite
        Resize(spriteRenderer, sanimal.transform);

        // Perform other actions (e.g., buying the animal)
        GameModel.instance.BuyAnimal(selectedAnimal.price);

        switch (selectedAnimal.obj_type)
        {
            case ObjectType.LION:
                sanimal.AddComponent<Lion>();
                break;
            case ObjectType.GIRAFFE:
                sanimal.AddComponent<Giraffe>();
                break;
            case ObjectType.ZEBRA:
                sanimal.AddComponent<Zebra>();
                break;
            case ObjectType.HYENA:
                sanimal.AddComponent<Hyena>();
                break;
        }

        // Reset the selected animal and cursor
        selectedAnimal = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void SelectTerrain(PurchasableObject pur)
    {
        // TODO: implement terrain placement properly
        if (GameModel.instance.canAfford(pur.price))
        {
            selectedTerrain = pur;

            CloseShop();

            Cursor.SetCursor(selectionCursor, Vector2.zero, CursorMode.Auto);

            // Debug.Log("Ready to place: " + pur.name);
        }
        else
        {
            // Debug.Log("Not enough money to buy: " + pur.name);
        }
    }

    private void PlaceTerrain(Vector2 position)
    {
        // Instantiate the terrain prefab
        GameObject sterrain = Instantiate(terrainSpawn, position, Quaternion.identity);
        SpriteRenderer spriteRenderer = sterrain.GetComponent<SpriteRenderer>();

        // Set the sprite to the selected animal's image
        spriteRenderer.sprite = selectedTerrain.image;

        // Resize the sprite
        Resize(spriteRenderer, sterrain.transform);

        GameModel.instance.BuyTerrain(sterrain, position, selectedTerrain.price);

        switch (selectedTerrain.obj_type)
        {
            case ObjectType.WATER:
                AnimalManager.Instance.addWater(sterrain.gameObject.transform.position);
                break;
            case ObjectType.FOOD:
                if (selectedTerrain.obj_name == "Tree")
                {
                    sterrain.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("OverEntity");
                    AnimalManager.Instance.addFood(sterrain.gameObject.transform.position, 6);
                }
                else if (selectedTerrain.obj_name == "Bush")
                {
                    AnimalManager.Instance.addFood(sterrain.gameObject.transform.position, 4);
                }
                else if (selectedTerrain.obj_name == "Grass")
                {
                    AnimalManager.Instance.addFood(sterrain.gameObject.transform.position, 2);
                }
                break;
            case ObjectType.OTHER: // azaz camera
                Transform spritemask = sterrain.transform.Find("Sprite Mask");
                spritemask.localScale = new Vector3(2.5f, 2.5f, 1f);
                break;
        }

        // Reset the selected animal and cursor
        selectedTerrain = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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

    /* grid tiles have a width of 2 units, so every (odd, odd) coordinate is in the center of a tile
     * this function rounds every coordinate to the nearest odd number, for centered placement
     */
    private Vector2 GetPlacementLocation()
    {
        // get position of click
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // offset coordinates to match the selection cursor's center
        mousePosition.x += 0.3f;
        mousePosition.y -= 0.3f;

        // round to the center of the tile
        if (Math.Abs(Math.Floor(mousePosition.x) % 2) == 1) { mousePosition.x = (float)Math.Floor(mousePosition.x); }
        else if (Math.Abs(Math.Ceiling(mousePosition.x) % 2) == 1) { mousePosition.x = (float)Math.Ceiling(mousePosition.x); }
        else { mousePosition.x += 1; }
        if (Math.Abs(Math.Floor(mousePosition.y) % 2) == 1) { mousePosition.y = (float)Math.Floor(mousePosition.y); }
        else if (Math.Abs(Math.Ceiling(mousePosition.y) % 2) == 1) { mousePosition.y = (float)Math.Ceiling(mousePosition.y); }
        else { mousePosition.y += 1; }

        return mousePosition;
    }

    private void CancelPlacement()
    {
        selectedAnimal = null;
        selectedTerrain = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private bool isInBounds(Vector2 location) => ((x_lowerBound <= location.x && location.x <= x_upperBound) &&
                                                  (y_lowerBound <= location.y && location.y <= y_upperBound));
    private bool isFree(Vector2 location) => (!GameModel.instance.getMapGrid().ContainsKey(location));

    void Start()
    {
        GenUI();
    }

    void Update()
    {
        // check for left-click to place the selected item
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 placementLocation = GetPlacementLocation();
            // Debug.Log(placementLocation);
            if (selectedAnimal != null && isInBounds(placementLocation)) { PlaceAnimal(placementLocation); }
            else if (selectedTerrain != null && isInBounds(placementLocation) && isFree(placementLocation)) { PlaceTerrain(placementLocation); }
            else { CancelPlacement(); }
        }

        // cancel placement on right-click
        if (Input.GetMouseButtonDown(1)) { CancelPlacement(); }
    }
}