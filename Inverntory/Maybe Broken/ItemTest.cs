using UnityEngine;
using System.Collections.Generic;

public class ItemTest : MonoBehaviour
{

    public Item_SO Item;

    public Vector3 CheckArea;

    public Vector3 Offset;

    public InventoryCSharp SendToInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        FindInventoryToSendTo();
        AddItemToInv();
    }

    public void AddItemToInv()
    {

        SendToInventory.AddItem(Item.ItemID, Item.ItemQuantity);

    }

    public void RemoveItem()
    {

        SendToInventory.RemoveItem(Item.ItemID,Item.ItemQuantity);

    }

    public void FindInventoryToSendTo()
    {
        if (Offset == Vector3.zero)
        {

            Collider[] hits = Physics.OverlapBox(transform.position, CheckArea / 2);
            foreach (var hit in hits)
            {
                var inv = hit.GetComponent<InventoryCSharp>();
                if (inv != null)
                {
                    SendToInventory = inv;
                    return;
                }
            }
        }
        else
        {

            Collider[] hits = Physics.OverlapBox(Offset, CheckArea / 2);
            foreach (var hit in hits)
            {
                var inv = hit.GetComponent<InventoryCSharp>();
                if (inv != null)
                {
                    SendToInventory = inv;
                    return;
                }
            }

        }
    }

    public void OnDrawGizmos()
    {

        if (Offset != Vector3.zero)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Offset, CheckArea);

        }
        else
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(this.transform.position, CheckArea);
        }
    }

}
