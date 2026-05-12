using NUnit;
using UnityEngine;

public class InventoryCSharp : MonoBehaviour
{
    public Inventory_SO Inventory_SO;
    public UI_In_Game UI;

    void Start()
    {
        // Ensure the ScriptableObject is created if missing
        if (Inventory_SO == null)
        {
            Inventory_SO = ScriptableObject.CreateInstance<Inventory_SO>();

            Inventory_SO.InventorySize = 10; 
            Inventory_SO.defaultMaxItemQuantity = 100;
            Inventory_SO.DefaultHasLimit = true;
        }

        // Initial UI update
        UpdateUI();
    }

    public void AddItem(int itemID, int quantity)
    {
        if (quantity <= 0) return;

        // Get the item definition from the database
        Item_SO itemDef = ItemDatabase.GetItem(itemID);
        if (itemDef == null)
        {
            Debug.LogError($"Item with ID {itemID} not found in database!");
            return;
        }

        int added = Inventory_SO.AddItemToInventory(
            itemID,
            quantity,
            itemDef.ItemHasLimit,
            itemDef.MaxItemQuantity
        );

        if (added < quantity)
            Debug.LogWarning($"Could only add {added} of {quantity} for item {itemID}.");

        // Update UI after any change
        UpdateUI();
    }

    public void RemoveItem(int itemID, int quantity)
    {
        if (quantity <= 0) return;

        int removed = Inventory_SO.RemoveItemFromInventory(itemID, quantity);
        if (removed < quantity)
            Debug.LogWarning($"Could only remove {removed} of {quantity} for item {itemID}.");

        UpdateUI();
    }

    public int GetItemQuantity(int itemID)
    {
        return Inventory_SO.GetAmount(itemID);
    }

    public void ClearInventory()
    {
        Inventory_SO.ClearInventory();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (UI != null)
            UI.UpdateHotbarFromInventory(Inventory_SO.inventory);
    }
}