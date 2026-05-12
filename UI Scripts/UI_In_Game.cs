using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct Hotbar_Slot
{
    public int SlotIndex;
    public Image SlotImage;
    public Image SlotBackgroundImage;
    public int ItemID;
    public TextMeshProUGUI QuantityText;
    public int Quantity;

}

[System.Serializable]
public struct Player_Inv
{
    public int SlotIndex;
    public Image SlotImage;
    public Image SlotBackgroundImage;
    public int ItemID;
    public TextMeshProUGUI QuantityText;
    public int Quantity;

}

[System.Serializable]
public struct GetOther_Inv
{
    public int SlotIndex;
    public Image SlotImage;
    public Image SlotBackgroundImage;
    public int ItemID;
    public TextMeshProUGUI QuantityText;
    public int Quantity;

}

[System.Serializable]
public struct GetPlayrAlt_Inv
{
    public int SlotIndex;
    public Image SlotImage;
    public Image SlotBackgroundImage;
    public int ItemID;
    public TextMeshProUGUI QuantityText;
    public int Quantity;

}

[System.Serializable]
public struct Dialogue_UI
{
    public Image Portrait;
    public TextMeshProUGUI SpeakerName;
    public TextMeshProUGUI DialogueText;
}

public class UI_In_Game : MonoBehaviour
{
    private Vector2 mousePosition;
    private Input_system_Manager PlayerInputs;

    [Header("Dialogue, dont touch")]
    public UI_Dialogue_SO CurrentDialogue;

    private Inventory_SO OtherObjectsInv;
    public Inventory_SO PlayerInv;


    public Hotbar_Slot[] HotbarUI;
    public Player_Inv[] PlayerInvUI;
    public GetOther_Inv[] GetOtherInvUI;
    public GetPlayrAlt_Inv[] GetPlayerAltUI;
    public Dialogue_UI DialogueUI;


    private int currentHotbarSlotIndex = 0;

    [Header("assign this to something for custom mouse cursor")]
    public Image MousePosImage;

    [Header("Panels")]
    public Image HotbarPanel;
    public Image InventoryPanel;

    public Image DuelOtherPanel;
    public Image DuelPlayerPanel;

    public Image DialoguePanel;

    private bool openCloseInvI = false;
    private bool openCloseInvClick = false;

    private Inventory_SO selectedInventory;
    private int selectedInventorySlot = -1;
    private bool isSelecting = false;

    private Vector2 SaveScrollValue = Vector2.zero;

    [Header("Selected Item")]
    public Item_SO SelectedItem;

    private void Awake()
    {

        Cursor.visible = false;

        PlayerInputs = new Input_system_Manager();

        PlayerInputs.IsometricView.Mouse_Position.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();

        PlayerInputs.IsometricView.OpenClose_Inventory.performed += ctx => openCloseInvI = !openCloseInvI;
        PlayerInputs.IsometricView.OpenClose_Inventory.performed += ctx => OpenInvToggle();

        PlayerInputs.IsometricView.Click_right.performed += ctx => openCloseInvClick = !openCloseInvClick;
        PlayerInputs.IsometricView.Click_right.performed += ctx => OpenInvToggle();

        PlayerInputs.IsometricView.Click_left.performed += ctx => SelectInvSlot();
        PlayerInputs.IsometricView.Click_left.performed += ctx => ChangeDialogue();

        PlayerInputs.IsometricView.On_Hotbar_Change.performed += ctx => SaveScrollValue = ctx.ReadValue<Vector2>();
        PlayerInputs.IsometricView.On_Hotbar_Change.canceled += ctx => SaveScrollValue = Vector2.zero;

        int children = 0;
        children = HotbarPanel.transform.childCount;

        for (int j = 0; j < children; j++)
        {

            HotbarUI[j].SlotIndex = j + 1;
            HotbarUI[j].SlotBackgroundImage = HotbarPanel.transform.GetChild(j).GetComponent<Image>();
            HotbarUI[j].SlotImage = HotbarUI[j].SlotBackgroundImage.transform.GetChild(0).GetComponent<Image>();
            HotbarUI[j].QuantityText = HotbarUI[j].SlotImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            HotbarUI[j].ItemID = PlayerInv.inventory[j].ItemID;
            HotbarUI[j].Quantity = PlayerInv.inventory[j].ItemQuantity;

        }

        for (int i = 0; i < HotbarUI.Length; i++)
        {
            if (HotbarUI[i].ItemID == 0)
            {
                if (HotbarUI[i].SlotImage != null)
                {
                    HotbarUI[i].SlotImage.enabled = false;
                }
                if (HotbarUI[i].QuantityText != null)
                {
                    HotbarUI[i].QuantityText.enabled = false;
                }
            }
            else
            {
                if (HotbarUI[i].SlotImage != null)
                {
                    HotbarUI[i].SlotImage.enabled = true;
                }
                if (HotbarUI[i].Quantity > 0)
                {
                    HotbarUI[i].QuantityText.enabled = true;
                    PlayerInv.inventory[i].ItemQuantity = HotbarUI[i].Quantity;
                    HotbarUI[i].QuantityText.text = HotbarUI[i].Quantity.ToString();
                }
                else
                {
                    HotbarUI[i].QuantityText.enabled = false;

                }
            }

            HotbarUI[i].SlotIndex = i;
        }
        CheckStack();

        UpdateSlotHighlights();

        InventoryPanel.enabled = false;
        children = InventoryPanel.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            InventoryPanel.transform.GetChild(i).gameObject.SetActive(false);

        }

        DuelOtherPanel.enabled = false;
        children = DuelOtherPanel.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            DuelOtherPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        DuelPlayerPanel.enabled = false;
        children = DuelPlayerPanel.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            DuelPlayerPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        DialoguePanel.enabled = false;
        children = DialoguePanel.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            DialoguePanel.transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    private void OpenInvToggle()
    {

        if (openCloseInvI)
        {

            OpenInventoryPlayerOnly();

        }
        else if (openCloseInvClick)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                if (hit.collider.gameObject.GetComponent<InventoryCSharp>() != null)
                {


                        OtherObjectsInv = hit.collider.gameObject.GetComponent<InventoryCSharp>().Inventory_SO;

                }


            }


            if (OtherObjectsInv != null)
            {
                OpenDuelInventory();
            }
            else
            {
                CloseDuelInventory();
                CloseInventoryPlayerOnly();

            }

        }
        else
        {
            CloseDuelInventory();
            CloseInventoryPlayerOnly();
        }
    }

    void Update()
    {
        MouseRetical();

        if (SaveScrollValue.y > 0)
        {
            CycleUpHotbar();
        }
        else if (SaveScrollValue.y < 0)
        {
            CycleDownHotbar();
        }

    }
    // region has mouse retical
    #region
    void OnEnable()
    {
        PlayerInputs.Enable();
    }

    void OnDisable()
    {
        PlayerInputs.Disable();
    }

    public void MouseRetical()
    {
        print(mousePosition);
        MousePosImage.rectTransform.position = mousePosition;
        if (MousePosImage != null)
        {
            MousePosImage.rectTransform.position = mousePosition;
        }
        else
        {
            Debug.LogWarning("MousePos Image reference is not assigned in the inspector.");
            MousePosImage.enabled = false;
        }


    }
    #endregion
    // region has inv management
    #region
    void CheckStack() // to see if slot has a quantity of 0 and if so, disable the image and quantity text
    {
        foreach (var slot in HotbarUI)
        {

            if (slot.Quantity <= 0)
            {
                if (slot.SlotImage != null)
                {
                    slot.SlotImage.enabled = false;
                }
                if (slot.QuantityText != null)
                {
                    slot.QuantityText.enabled = false;
                }
            }
            else
            {
                if (slot.SlotImage != null)
                {
                    slot.SlotImage.enabled = true;
                }
                if (slot.QuantityText != null)
                {
                    slot.QuantityText.enabled = true;
                    slot.QuantityText.text = slot.Quantity.ToString();
                }
            }


        }
    }

    public void CycleUpHotbar()
    {
        // Move selection up (to the right / next slot)

        currentHotbarSlotIndex = (currentHotbarSlotIndex + 1) % HotbarUI.Length;
        UpdateSlotHighlights();

        SelectedItem = HotbarUI[currentHotbarSlotIndex].ItemID != 0 ? ItemDatabase.GetItem(HotbarUI[currentHotbarSlotIndex].ItemID) : null;

    }

    public void CycleDownHotbar()
    {
        // Move selection down (to the left / previous slot)
        currentHotbarSlotIndex--;
        if (currentHotbarSlotIndex < 0)
        {
            currentHotbarSlotIndex = HotbarUI.Length - 1;
        }
        UpdateSlotHighlights();

        SelectedItem = HotbarUI[currentHotbarSlotIndex].ItemID != 0 ? ItemDatabase.GetItem(HotbarUI[currentHotbarSlotIndex].ItemID) : null;

    }

    private void UpdateSlotHighlights()
    {

        for (int i = 0; i < HotbarUI.Length; i++)
        {
            if (HotbarUI[i].SlotBackgroundImage != null)
            {
                HotbarUI[i].SlotBackgroundImage.color = Color.black;
            }
        }

        if (HotbarUI[currentHotbarSlotIndex].SlotBackgroundImage != null)
        {
            HotbarUI[currentHotbarSlotIndex].SlotBackgroundImage.color = Color.gray;
        }
    }

    public void UpdateSlot(int index, int newItemID, int quantity)
    {
        if (index < 0 || index >= HotbarUI.Length) return;

        HotbarUI[index].ItemID = newItemID;
        if (newItemID == 0)
        {
            HotbarUI[index].SlotImage.enabled = false;
            HotbarUI[index].QuantityText.enabled = false;
        }
        else
        {
            HotbarUI[index].SlotImage.enabled = true;
            HotbarUI[index].QuantityText.enabled = true;
            HotbarUI[index].QuantityText.text = quantity.ToString();
        }
    }

    public void UpdateHotbarFromInventory(Item[] inventory)
    {
        for (int i = 0; i < HotbarUI.Length; i++)
        {
            if (i < inventory.Length)
            {
                Item invSlot = inventory[i];
                if (invSlot.ItemID == 0)   // empty
                {
                    HotbarUI[i].ItemID = 0;
                    HotbarUI[i].Quantity = 0;
                    if (HotbarUI[i].SlotImage != null)
                        HotbarUI[i].SlotImage.enabled = false;
                    if (HotbarUI[i].QuantityText != null)
                        HotbarUI[i].QuantityText.enabled = false;
                }
                else
                {
                    HotbarUI[i].ItemID = invSlot.ItemID;
                    HotbarUI[i].Quantity = invSlot.ItemQuantity;
                    if (HotbarUI[i].SlotImage != null)
                    {
                        HotbarUI[i].SlotImage.enabled = true;
                    }
                    if (HotbarUI[i].QuantityText != null)
                    {
                        HotbarUI[i].QuantityText.enabled = true;
                        HotbarUI[i].QuantityText.text = invSlot.ItemQuantity.ToString();
                    }
                }
            }
            else
            {
                // Inventory smaller than hotbar – clear remaining slots
                HotbarUI[i].ItemID = 0;
                HotbarUI[i].Quantity = 0;
                if (HotbarUI[i].SlotImage != null)
                    HotbarUI[i].SlotImage.enabled = false;
                if (HotbarUI[i].QuantityText != null)
                    HotbarUI[i].QuantityText.enabled = false;
            }
        }
    }

    public void OpenInventoryPlayerOnly()
    {

        HotbarPanel.enabled = false;
        int HotBarChildren = 0;
        HotBarChildren = HotbarPanel.transform.childCount;
        for (int i = 0; i < HotBarChildren; i++)
        {
            HotbarPanel.transform.GetChild(i).gameObject.SetActive(false);

        }


        InventoryPanel.enabled = true;
        int InvChildren = 0; int Count = 0;
        InvChildren = InventoryPanel.transform.childCount;
        int j = -1;
        for (int AmoutToUse = PlayerInv.InventorySize; AmoutToUse < InvChildren;)
        {
            j++;

            if (Count >= AmoutToUse) break;

            InventoryPanel.transform.GetChild(j).gameObject.SetActive(true);
            Count++;

            PlayerInvUI[j].SlotIndex = j + 1;
            PlayerInvUI[j].SlotBackgroundImage = InventoryPanel.transform.GetChild(j).GetComponent<Image>();
            PlayerInvUI[j].SlotImage = PlayerInvUI[j].SlotBackgroundImage.transform.GetChild(0).GetComponent<Image>();
            PlayerInvUI[j].QuantityText = PlayerInvUI[j].SlotImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            PlayerInvUI[j].ItemID = PlayerInv.inventory[j].ItemID;
            PlayerInvUI[j].Quantity = PlayerInv.inventory[j].ItemQuantity;
            if (PlayerInvUI[j].ItemID != 0)
            {
                PlayerInvUI[j].QuantityText.text = PlayerInv.inventory[j].ItemQuantity.ToString();
                Item_SO CurrentItem = ItemDatabase.GetItem(PlayerInvUI[j].ItemID);
                print(CurrentItem.Name);
            }
            else
            {
                PlayerInvUI[j].QuantityText.enabled = false;
            }

        }


    }
    public void CloseInventoryPlayerOnly()
    {

        HotbarPanel.enabled = true;
        int HotBarChildren = HotbarPanel.transform.childCount;
        for (int i = 0; i < HotBarChildren; i++)
        {
            HotbarPanel.transform.GetChild(i).gameObject.SetActive(true);
        }

        InventoryPanel.enabled = false;
        int InvChildren = InventoryPanel.transform.childCount;
        for (int i = 0; i < InvChildren; i++)
        {
            InventoryPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    public void CloseDuelInventory()
    {
        int children = 0;

        OtherObjectsInv = null;


        HotbarPanel.enabled = true;
        int HotBarChildren = HotbarPanel.transform.childCount;
        for (int i = 0; i < HotBarChildren; i++)
        {
            HotbarPanel.transform.GetChild(i).gameObject.SetActive(true);
        }

        DuelOtherPanel.enabled = false;
        children = DuelOtherPanel.transform.childCount;
        for (int i = 0; i < children; i++)
        {

            if (i >= 30) break;

            DuelOtherPanel.transform.GetChild(i).gameObject.SetActive(false);

            GetOtherInvUI[i].SlotIndex = 0;
            GetOtherInvUI[i].SlotBackgroundImage = null;
            GetOtherInvUI[i].SlotImage = null;
            GetOtherInvUI[i].QuantityText = null;

            GetOtherInvUI[i].ItemID = 0;
            GetOtherInvUI[i].Quantity = 0;

        }

        DuelPlayerPanel.enabled = false;
        children = DuelPlayerPanel.transform.childCount;
        for (int i = 0; i < children; i++)
        {

            if (i >= 30) break;

            DuelPlayerPanel.transform.GetChild(i).gameObject.SetActive(false);

            GetPlayerAltUI[i].SlotIndex = 0;
            GetPlayerAltUI[i].SlotBackgroundImage = null;
            GetPlayerAltUI[i].SlotImage = null;
            GetPlayerAltUI[i].QuantityText = null;


            GetPlayerAltUI[i].ItemID = 0;

            GetPlayerAltUI[i].Quantity = 0;

        }
    }

    public void OpenDuelInventory()
    {

        foreach (Transform child in DuelOtherPanel.transform)
            child.gameObject.SetActive(false);
        foreach (Transform child in DuelPlayerPanel.transform)
            child.gameObject.SetActive(false);

        HotbarPanel.enabled = false;
        int HotBarChildren = 0;
        HotBarChildren = HotbarPanel.transform.childCount;
        for (int i = 0; i < HotBarChildren; i++)
        {
            HotbarPanel.transform.GetChild(i).gameObject.SetActive(false);

        }

        int children = 0; int countO = 0;

        DuelOtherPanel.enabled = true;
        children = DuelOtherPanel.transform.childCount;
        int j = -1;
        for (int amountToUse = OtherObjectsInv.InventorySize; amountToUse < children;)
        {
            j++;
            if (countO >= amountToUse) break;

            DuelOtherPanel.transform.GetChild(j).gameObject.SetActive(true);
            countO++;



            GetOtherInvUI[j].SlotIndex = j + 1;
            GetOtherInvUI[j].SlotBackgroundImage = DuelOtherPanel.transform.GetChild(j).GetComponent<Image>();
            GetOtherInvUI[j].SlotImage = GetOtherInvUI[j].SlotBackgroundImage.transform.GetChild(0).GetComponent<Image>();
            GetOtherInvUI[j].QuantityText = GetOtherInvUI[j].SlotImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            GetOtherInvUI[j].ItemID = OtherObjectsInv.inventory[j].ItemID;
            GetOtherInvUI[j].Quantity = OtherObjectsInv.inventory[j].ItemQuantity;
            if (GetOtherInvUI[j].ItemID != 0)
            {
                GetOtherInvUI[j].QuantityText.enabled = true;
                GetOtherInvUI[j].QuantityText.text = OtherObjectsInv.inventory[j].ItemQuantity.ToString();

            }
            else
            {
                GetOtherInvUI[j].QuantityText.enabled = false;
            }

        }

        int countP = 0;

        DuelPlayerPanel.enabled = true;
        children = DuelPlayerPanel.transform.childCount;

        j = -1;
        for (int amountToUse = PlayerInv.InventorySize; amountToUse < children;)
        {
            j++;
            if (countP >= amountToUse) break;

            DuelPlayerPanel.transform.GetChild(j).gameObject.SetActive(true);
            countP++;

            GetPlayerAltUI[j].SlotIndex = j + 1;
            GetPlayerAltUI[j].SlotBackgroundImage = DuelPlayerPanel.transform.GetChild(j).GetComponent<Image>();
            GetPlayerAltUI[j].SlotImage = GetPlayerAltUI[j].SlotBackgroundImage.transform.GetChild(0).GetComponent<Image>();
            GetPlayerAltUI[j].QuantityText = GetPlayerAltUI[j].SlotImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            GetPlayerAltUI[j].ItemID = PlayerInv.inventory[j].ItemID;
            GetPlayerAltUI[j].Quantity = PlayerInv.inventory[j].ItemQuantity;
            if (GetPlayerAltUI[j].ItemID != 0)
            {

                GetPlayerAltUI[j].QuantityText.enabled = true;
                GetPlayerAltUI[j].QuantityText.text = PlayerInv.inventory[j].ItemQuantity.ToString();

            }
            else
            {
                GetPlayerAltUI[j].QuantityText.enabled = false;
            }

        }

    }

    private (Inventory_SO inventory, int slotIndex, bool success) GetInventoryAndSlotFromRaycast(Vector2 mousePos)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = mousePos };
        var results = new System.Collections.Generic.List<RaycastResult>();

        // Find GraphicRaycaster (same as in SelectInvSlot)
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null) raycaster = canvas.GetComponent<GraphicRaycaster>();
        }
        if (raycaster == null) return (null, -1, false);

        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            Transform clicked = result.gameObject.transform;

            // Check each panel type
            // HotbarPanel
            if (HotbarPanel != null && clicked.IsChildOf(HotbarPanel.transform))
            {
                Transform slotBackground = clicked;
                while (slotBackground != null && slotBackground.parent != HotbarPanel.transform)
                    slotBackground = slotBackground.parent;
                if (slotBackground == null) continue;

                int slotIndex = slotBackground.GetSiblingIndex();
                if (slotIndex < 0 || slotIndex >= HotbarUI.Length) continue;
                return (PlayerInv, slotIndex, true);
            }

            // InventoryPanel (full player inventory)
            if (InventoryPanel != null && InventoryPanel.enabled && clicked.IsChildOf(InventoryPanel.transform))
            {
                Transform slotBackground = clicked;
                while (slotBackground != null && slotBackground.parent != InventoryPanel.transform)
                    slotBackground = slotBackground.parent;
                if (slotBackground == null) continue;

                int slotIndex = slotBackground.GetSiblingIndex();
                if (slotIndex < 0 || slotIndex >= PlayerInvUI.Length) continue;
                return (PlayerInv, slotIndex, true);
            }

            // DuelOtherPanel
            if (DuelOtherPanel != null && DuelOtherPanel.enabled && clicked.IsChildOf(DuelOtherPanel.transform))
            {
                Transform slotBackground = clicked;
                while (slotBackground != null && slotBackground.parent != DuelOtherPanel.transform)
                    slotBackground = slotBackground.parent;
                if (slotBackground == null) continue;

                int slotIndex = slotBackground.GetSiblingIndex();
                if (slotIndex < 0 || slotIndex >= GetOtherInvUI.Length) continue;
                return (OtherObjectsInv, slotIndex, true);
            }

            // DuelPlayerPanel
            if (DuelPlayerPanel != null && DuelPlayerPanel.enabled && clicked.IsChildOf(DuelPlayerPanel.transform))
            {
                Transform slotBackground = clicked;
                while (slotBackground != null && slotBackground.parent != DuelPlayerPanel.transform)
                    slotBackground = slotBackground.parent;
                if (slotBackground == null) continue;

                int slotIndex = slotBackground.GetSiblingIndex();
                if (slotIndex < 0 || slotIndex >= GetPlayerAltUI.Length) continue;
                return (PlayerInv, slotIndex, true);
            }
        }

        return (null, -1, false);
    }

    private void SelectInvSlot()
    {
        var (inv, slotIndex, success) = GetInventoryAndSlotFromRaycast(mousePosition);
        if (!success)
        {
            // Clicked outside any inventory slot – cancel selection
            isSelecting = false;
            selectedInventory = null;
            selectedInventorySlot = -1;
            return;
        }

        if (!isSelecting)
        {
            // First click – select the slot
            selectedInventory = inv;
            selectedInventorySlot = slotIndex;
            isSelecting = true;
            Debug.Log($"Selected slot: {inv?.name} index {slotIndex}");
            SelectedItem = inv?.inventory[slotIndex].ItemID != 0 ? ItemDatabase.GetItem(inv.inventory[slotIndex].ItemID) : null;
        }
        else
        {
            // Second click – perform transfer/swap
            TransferOrSwap(selectedInventory, selectedInventorySlot, inv, slotIndex);
            // Reset selection
            isSelecting = false;
            selectedInventory = null;
            selectedInventorySlot = -1;
            SelectedItem = null;
        }
    }
    private void TransferOrSwap(Inventory_SO srcInv, int srcSlot, Inventory_SO dstInv, int dstSlot)
    {
        if (srcInv == null || dstInv == null) return;
        if (srcSlot < 0 || srcSlot >= srcInv.InventorySize) return;
        if (dstSlot < 0 || dstSlot >= dstInv.InventorySize) return;

        // If same inventory and same slot, just cancel selection
        if (srcInv == dstInv && srcSlot == dstSlot)
            return;

        // Swap items
        Item srcItem = srcInv.inventory[srcSlot];
        Item dstItem = dstInv.inventory[dstSlot];

        srcInv.inventory[srcSlot] = dstItem;
        dstInv.inventory[dstSlot] = srcItem;

        // Update UI for both slots
        UpdateUISlotForInventory(srcInv, srcSlot);
        UpdateUISlotForInventory(dstInv, dstSlot);
    }

    private void UpdateUISlotForInventory(Inventory_SO inv, int slotIndex)
    {
        if (inv == null || slotIndex < 0) return;

        Item item = inv.inventory[slotIndex];

        // Update hotbar if this slot is within hotbar range
        if (inv == PlayerInv && slotIndex < HotbarUI.Length)
        {
            HotbarUI[slotIndex].ItemID = item.ItemID;
            HotbarUI[slotIndex].Quantity = item.ItemQuantity;
            if (item.ItemID == 0)
            {
                if (HotbarUI[slotIndex].SlotImage != null)
                    HotbarUI[slotIndex].SlotImage.enabled = false;
                if (HotbarUI[slotIndex].QuantityText != null)
                    HotbarUI[slotIndex].QuantityText.enabled = false;
            }
            else
            {
                if (HotbarUI[slotIndex].SlotImage != null)
                {
                    HotbarUI[slotIndex].SlotImage.enabled = true;
                    // Assuming you have a way to set the sprite from item ID
                    // You might need to assign the sprite here.
                    // For now, we'll keep it as is.
                }
                if (HotbarUI[slotIndex].QuantityText != null)
                {
                    HotbarUI[slotIndex].QuantityText.enabled = true;
                    HotbarUI[slotIndex].QuantityText.text = item.ItemQuantity.ToString();
                }
            }
        }

        // Update full inventory panel if active and slot within its range
        if (InventoryPanel != null && InventoryPanel.enabled && inv == PlayerInv && slotIndex < PlayerInvUI.Length)
        {
            PlayerInvUI[slotIndex].ItemID = item.ItemID;
            PlayerInvUI[slotIndex].Quantity = item.ItemQuantity;
            if (item.ItemID == 0)
            {
                if (PlayerInvUI[slotIndex].SlotImage != null)
                    PlayerInvUI[slotIndex].SlotImage.enabled = false;
                if (PlayerInvUI[slotIndex].QuantityText != null)
                    PlayerInvUI[slotIndex].QuantityText.enabled = false;
            }
            else
            {
                if (PlayerInvUI[slotIndex].SlotImage != null)
                {
                    PlayerInvUI[slotIndex].SlotImage.enabled = true;
                    // Set sprite if needed
                }
                if (PlayerInvUI[slotIndex].QuantityText != null)
                {
                    PlayerInvUI[slotIndex].QuantityText.enabled = true;
                    PlayerInvUI[slotIndex].QuantityText.text = item.ItemQuantity.ToString();
                }
            }
        }

        // Update duel player panel
        if (DuelPlayerPanel != null && DuelPlayerPanel.enabled && inv == PlayerInv && slotIndex < GetPlayerAltUI.Length)
        {
            GetPlayerAltUI[slotIndex].ItemID = item.ItemID;
            GetPlayerAltUI[slotIndex].Quantity = item.ItemQuantity;
            if (item.ItemID == 0)
            {
                if (GetPlayerAltUI[slotIndex].SlotImage != null)
                    GetPlayerAltUI[slotIndex].SlotImage.enabled = false;
                if (GetPlayerAltUI[slotIndex].QuantityText != null)
                    GetPlayerAltUI[slotIndex].QuantityText.enabled = false;
            }
            else
            {
                if (GetPlayerAltUI[slotIndex].SlotImage != null)
                    GetPlayerAltUI[slotIndex].SlotImage.enabled = true;
                if (GetPlayerAltUI[slotIndex].QuantityText != null)
                {
                    GetPlayerAltUI[slotIndex].QuantityText.enabled = true;
                    GetPlayerAltUI[slotIndex].QuantityText.text = item.ItemQuantity.ToString();
                }
            }
        }

        // Update duel other panel
        if (DuelOtherPanel != null && DuelOtherPanel.enabled && inv == OtherObjectsInv && slotIndex < GetOtherInvUI.Length)
        {
            GetOtherInvUI[slotIndex].ItemID = item.ItemID;
            GetOtherInvUI[slotIndex].Quantity = item.ItemQuantity;
            if (item.ItemID == 0)
            {
                if (GetOtherInvUI[slotIndex].SlotImage != null)
                    GetOtherInvUI[slotIndex].SlotImage.enabled = false;
                if (GetOtherInvUI[slotIndex].QuantityText != null)
                    GetOtherInvUI[slotIndex].QuantityText.enabled = false;
            }
            else
            {
                if (GetOtherInvUI[slotIndex].SlotImage != null)
                    GetOtherInvUI[slotIndex].SlotImage.enabled = true;
                if (GetOtherInvUI[slotIndex].QuantityText != null)
                {
                    GetOtherInvUI[slotIndex].QuantityText.enabled = true;
                    GetOtherInvUI[slotIndex].QuantityText.text = item.ItemQuantity.ToString();
                }
            }
        }
    }

    #endregion
    // region has dialogue management
    #region
    // call this function from other scripts - make sure to assign an SO when you do
    public void OpenDialogue(UI_Dialogue_SO uI_Dialogue_SO)
    {
        if (uI_Dialogue_SO == null)
        {
            Debug.LogWarning("Attempted to open dialogue with a null UI_Dialogue_SO reference.");
            return;
        }
        CloseDuelInventory();
        CloseInventoryPlayerOnly();
        CurrentDialogue = uI_Dialogue_SO;

        if (CurrentDialogue.Portrait == null)
        {
            Debug.LogWarning($"Dialogue '{CurrentDialogue.name}' has no portrait assigned.");

        }

        DialoguePanel.enabled = true;
        int children = DialoguePanel.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            DialoguePanel.transform.GetChild(i).gameObject.SetActive(true);
        }

        DialoguePanel.enabled = true;
        DialogueUI.Portrait.sprite = CurrentDialogue.Portrait;
        DialogueUI.SpeakerName.text = CurrentDialogue.SpeakerName.ToString();
        DialogueUI.DialogueText.text = CurrentDialogue.DialogueHere.ToString();
    }

    public void ChangeDialogue()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = mousePosition };
        var results = new System.Collections.Generic.List<RaycastResult>();

        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null) raycaster = canvas.GetComponent<GraphicRaycaster>();

        }

        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == DialoguePanel.gameObject)
            {
                CloseDuelInventory();
                CloseInventoryPlayerOnly();
                if (CurrentDialogue.NextDialogue != null)
                {
                    CurrentDialogue = CurrentDialogue.NextDialogue;

                    DialogueUI.Portrait.sprite = CurrentDialogue.Portrait;
                    DialogueUI.SpeakerName.text = CurrentDialogue.SpeakerName;
                    DialogueUI.DialogueText.text = CurrentDialogue.DialogueHere;

                }
                else
                {

                    DialoguePanel.enabled = false;
                    int children = DialoguePanel.transform.childCount;
                    for (int i = 0; i < children; i++)
                    {
                        DialoguePanel.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    CurrentDialogue = null;
                }
                break; // Exit loop after handling dialogue click
            }
        }

    }
    #endregion
}
