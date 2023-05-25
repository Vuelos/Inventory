﻿namespace Inventory;

public class UIInventorySlot : UISlot
{
    private readonly UIInventory uiInventory;

    public UIInventorySlot(UIInventory uiInventory, int index, ItemCategory? itemCategoryFilter = null)
    {
        base.index = index;
        this.uiInventory = uiInventory;
        container = uiInventory.Container;
        Parent = this.CreateUI(uiInventory);
        this.ItemCategoryFilter = itemCategoryFilter;
    }

    public void Update()
    {

    }

    private void HandleLeftClick()
    {
        if (Main.Cursor.IsEmpty())
        {
            this.MoveAllTo(Main.Cursor);
        }
        else
        {
            Main.Cursor.MoveAllTo(this);
        }
    }

    private void HandleShiftLeftClick()
    {
        this.TransferAll();
    }

    private void HandleRightClick()
    {
        if (Main.Cursor.IsEmpty())
        {
            this.MoveOneTo(Main.Cursor);
        }
        else
        {
            Main.Cursor.MoveOneTo(this);
        }
    }

    /// <summary>
    /// If cursor is empty, takes half of slot to cursor
    /// If slot is empty, takes half of cursor to slot
    /// </summary>
    private void HandleShiftRightClick()
    {
        if (Main.Cursor.IsEmpty())
        {
            this.MoveHalfTo(Main.Cursor);
        }
        else
        {
            Main.Cursor.MoveHalfTo(this);
        }
    }

    private void TransferAll()
    {
        if (this.IsEmpty())
            return;

        var targetInv = (uiInventory == Main.PlayerInventory) ?
            Main.OtherInventory : Main.PlayerInventory;

        var slotIndex = targetInv.Container.TryGetEmptyOrSameTypeSlot(this.Get().Type);

        if (slotIndex == -1)
            return;

        var thisItem = this.Get();
        var targetItem = targetInv.Container.Get(slotIndex);

        if (targetInv.Container.HasItem(slotIndex))
        {
            if (targetItem.Type == thisItem.Type)
            {
                targetItem.Count += thisItem.Count;

                targetInv.SetItem(slotIndex, targetItem);

                this.Remove();
            }
        }
        else
        {
            targetInv.SetItem(slotIndex, thisItem);
            this.Remove();
        }
    }

    private Panel CreateUI(UIInventory uiInventory)
    {
        var panel = new Panel { CustomMinimumSize = Vector2.One * uiInventory.SlotSize };

        panel.GuiInput += (inputEvent) =>
        {
            if (inputEvent is not InputEventMouseButton eventMouseButton)
                return;

            if (eventMouseButton.IsLeftClickPressed())
            {
                if (Input.IsKeyPressed(Key.Shift))
                {
                    this.HandleShiftLeftClick();
                    return;
                }

                this.HandleLeftClick();
                return;
            }

            if (eventMouseButton.IsRightClickPressed())
            {
                if (Input.IsKeyPressed(Key.Shift))
                {
                    this.HandleShiftRightClick();
                    return;
                }

                this.HandleRightClick();
                return;
            }
        };

        uiInventory.SceneInv.GridContainer.AddChild(panel);
        return panel;
    }
}
