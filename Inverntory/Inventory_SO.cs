using UnityEngine;

[System.Serializable]
public struct Item
{
    public int ItemID;
    public int ItemQuantity;
    public bool ItemHasLimit;
    public int MaxItemQuantity;
}

[CreateAssetMenu(fileName = "Inventory_SO", menuName = "Scriptable Objects/Inventory_SO")]
public class Inventory_SO : ScriptableObject
{
    [SerializeField]
    public Item[] inventory;

    public int InventorySize = 10;

    [Header("Custom settings")]
    public bool DefaultHasLimit = true;
    public int defaultMaxItemQuantity = 100;

    private void OnEnable()
    {
        if (inventory == null || inventory.Length != InventorySize)
        {
            inventory = new Item[InventorySize];
        }
    }


    public int AddItemToInventory(int itemID, int amount, bool hasLimit, int maxQuantity)
    {
        if (amount <= 0) return 0;

        // Determine effective limit based on inventory default settings
        bool effectiveHasLimit;
        int effectiveMaxQuantity;
        if (DefaultHasLimit && defaultMaxItemQuantity > 0)
        {
            effectiveHasLimit = true;
            effectiveMaxQuantity = defaultMaxItemQuantity;
        }
        else
        {
            effectiveHasLimit = hasLimit;
            effectiveMaxQuantity = maxQuantity;
        }

        int remaining = amount;

        // First try to add to existing stacks of the same item
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].ItemID == itemID)
            {
                if (effectiveHasLimit)
                {
                    int space = effectiveMaxQuantity - inventory[i].ItemQuantity;
                    if (space > 0)
                    {
                        int add = Mathf.Min(remaining, space);
                        inventory[i].ItemQuantity += add;
                        // Update the slot's limit to the effective values
                        inventory[i].ItemHasLimit = effectiveHasLimit;
                        inventory[i].MaxItemQuantity = effectiveMaxQuantity;
                        remaining -= add;
                        if (remaining == 0) return amount;
                    }
                }
                else
                {
                    // No limit – add all remaining to this stack
                    inventory[i].ItemQuantity += remaining;
                    inventory[i].ItemHasLimit = false;
                    inventory[i].MaxItemQuantity = 0;
                    return amount;
                }
            }
        }

        // If still remaining, try to find empty slots for new stacks
        if (remaining > 0)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].ItemID == 0) // Empty slot
                {
                    int stackSize = remaining;
                    if (effectiveHasLimit)
                    {
                        stackSize = Mathf.Min(remaining, effectiveMaxQuantity);
                    }
                    inventory[i].ItemID = itemID;
                    inventory[i].ItemQuantity = stackSize;
                    inventory[i].ItemHasLimit = effectiveHasLimit;
                    inventory[i].MaxItemQuantity = effectiveMaxQuantity;
                    remaining -= stackSize;
                    if (remaining == 0) return amount;
                }
            }
        }

        if (remaining > 0)
        {
            Debug.LogWarning($"Inventory full or cannot add all {amount} of item {itemID}. Added {amount - remaining}.");
        }
        return amount - remaining;
    }

    public int RemoveItemFromInventory(int itemID, int amount)
    {
        if (amount <= 0) return 0;

        int remaining = amount;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].ItemID == itemID)
            {
                int remove = Mathf.Min(remaining, inventory[i].ItemQuantity);
                inventory[i].ItemQuantity -= remove;
                remaining -= remove;

                if (inventory[i].ItemQuantity <= 0)
                {
                    // Clear the slot
                    inventory[i].ItemID = 0;
                    inventory[i].ItemQuantity = 0;
                    inventory[i].ItemHasLimit = false;
                    inventory[i].MaxItemQuantity = 0;
                }

                if (remaining == 0) return amount;
            }
        }

        Debug.LogWarning($"Not enough of item {itemID} to remove {amount}. Removed {amount - remaining}.");
        return amount - remaining;
    }

    public void ClearInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i].ItemID = 0;
            inventory[i].ItemQuantity = 0;
            inventory[i].ItemHasLimit = false;
            inventory[i].MaxItemQuantity = 0;
        }
    }

    public int GetAmount(int itemID)
    {
        int total = 0;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].ItemID == itemID)
            {
                total += inventory[i].ItemQuantity;
            }
        }
        return total;
    }

    public void ResizeInventory(int newSize)
    {
        if (newSize < 0) return;
        Item[] newInventory = new Item[newSize];
        int copyLength = Mathf.Min(inventory.Length, newSize);
        for (int i = 0; i < copyLength; i++)
        {
            newInventory[i] = inventory[i];
        }
        inventory = newInventory;
        InventorySize = newSize;
    }
}