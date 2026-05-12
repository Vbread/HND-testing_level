using UnityEngine;

[CreateAssetMenu(fileName = "Item_SO", menuName = "Scriptable Objects/Inventory_SO_Individual")]
public class Item_SO : ScriptableObject
{
    public string Name;

    public GameObject ItemsGameObject;

    public int ItemID;
    public int ItemQuantity;
    public bool ItemHasLimit;
    public int MaxItemQuantity;

    public Sprite ItemSprite;

    private void OnEnable()
    {
        ItemDatabase.RegisterItem(this);
    }

}
