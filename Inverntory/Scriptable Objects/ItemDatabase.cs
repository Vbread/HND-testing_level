using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    private static Dictionary<int, Item_SO> items = new Dictionary<int, Item_SO>();

    public static void RegisterItem(Item_SO item)
    {
        if (!items.ContainsKey(item.ItemID))
            items.Add(item.ItemID, item);
        else
            Debug.LogWarning($"Item with ID {item.ItemID} already registered!");
    }

    public static Item_SO GetItem(int id)
    {
        items.TryGetValue(id, out Item_SO item);
        return item;
    }
}