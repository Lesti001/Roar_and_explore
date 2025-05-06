using UnityEngine;

public enum Type
{
    HERBIVORE,
    CARNIVORE,
    TERRAIN
}
public enum ObjectType
{
    LION,
    GIRAFFE,
    ZEBRA,
    HYENA,
    WATER,
    FOOD,
    OTHER
}
[CreateAssetMenu(fileName = "PurchasableObject", menuName = "Scriptable Objects/PurchasableObject")]
public class PurchasableObject : ScriptableObject
{
    public string obj_name;
    public Sprite image;
    public int price;
    public Type type;
    public ObjectType obj_type;
}
