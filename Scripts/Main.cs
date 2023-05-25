global using Godot;
global using GodotUtils;
global using System;

namespace Inventory;

public partial class Main : Node2D
{
    // Inventories
    public static UICursorSlot Cursor { get; set; }
    public static UIPlayerInventory PlayerInventory { get; set; }
    public static UIInventory OtherInventory { get; set; }

     public static UIInventory CurrencyInventory { get; set; }
     public static UIInventory ConsumableInventory { get; set; }
    // Msc
    public static CanvasLayer CanvasLayer { get; set; }
    public static SceneTree Tree { get; set; }

    public override void _Ready()
    {
        CanvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        Tree = GetTree();

        // Add UIPlayerInventory to canvas layer
        PlayerInventory = new UIPlayerInventory(CanvasLayer, 18, 9);

        // Setup player inventory
        PlayerInventory.SetItem(0, new Item(Items.CoinPink, 5));
        PlayerInventory.SetItem(1, new Item(Items.CoinRed, 10));
        PlayerInventory.SetItem(2, new Item(Items.Coin, 1));
        
        PlayerInventory.SetItem(7, new Item(Items.PotionRed, 4));
        PlayerInventory.SetItem(8, new Item(Items.PotionBlue, 100));

        //Add filtered inventories
        CurrencyInventory = new UIInventory(CanvasLayer, 3, 1, ItemCategory.Currency);
        CurrencyInventory.SceneInv.SetAnchor(Control.LayoutPreset.CenterLeft);

        ConsumableInventory = new UIInventory(CanvasLayer, 3, 1, ItemCategory.Consumable);
        ConsumableInventory.SceneInv.SetAnchor(Control.LayoutPreset.CenterRight);

        // Add UIOtherInventory to canvas layer
        OtherInventory = new UIInventory(CanvasLayer, 9, 9);
        OtherInventory.SceneInv.SetAnchor(Control.LayoutPreset.CenterTop);

        // Setup cursor
        var cursorParent = new Control { Name = "ParentCursor" };
        CanvasLayer.AddChild(cursorParent);
        Cursor = new UICursorSlot(cursorParent);
    }

    public override void _PhysicsProcess(double delta)
    {
        GodotUtils.Main.Update();

        // Update cursor
        Cursor.Update();

        PlayerInventory.Update();
        OtherInventory.Update();
    }

    public override void _Input(InputEvent @event)
    {
        // Debug
        if (Input.IsActionJustPressed("debug1"))
        {
            var container = PlayerInventory.Container;

            GD.Print("=== Player Inventory ===");

            for (int i = 0; i < container.Items.Length; i++)
            {
                var item = container.Items[i];

                if (item != null)
                    GD.Print($"[{i}] {item}");
            }
        }

        if (Input.IsActionJustPressed("debug2"))
        {

        }

        HotbarHotkeys();
    }

    private void HotbarHotkeys()
    {
        var inv = PlayerInventory;

        for (int i = 0; i < inv.Columns; i++)
            if (Input.IsActionJustPressed($"inventory_hotbar_{i + 1}"))
                if (Cursor.HasItem())
                {
                    var firstHotbarSlot = inv.Size - inv.Columns;
                    Cursor.MoveAllTo(inv.UIInventorySlots[firstHotbarSlot + i]);
                    break;
                }
    }
}
