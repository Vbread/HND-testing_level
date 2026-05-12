using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ItemField
{
    public int ID;
    public Item_SO Item;
}

[CreateAssetMenu(fileName = "ItemRegistry", menuName = "Scriptable Objects/Item Registry")]
public class ItemRegistry : ScriptableObject
{

    [SerializeField]
    public ItemField[] ItemRegistrys;

    private void OnEnable()
    {
        
        for (int i = 0; i < ItemRegistrys.Length; i++)
        {
            ItemRegistrys[i].ID = ItemRegistrys[i].Item.ItemID;
        }

    }

    public Item_SO FindItemByID(int FindMyID)
    {

        return ItemRegistrys[FindMyID-1].Item;

    }

}