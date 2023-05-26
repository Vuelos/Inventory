global using Godot;
global using GodotUtils;
global using System;

using static Godot.Control;

namespace Inventory;

public partial class Main : Node2D
{
    public static UICursorSlot Cursor { get; private set; }
    public static UIInventory PlayerInventory { get; private set; }
    public static UIInventory OtherInventory { get; private set; }
    public static UIInventory CurrencyInventory { get; private set; }
    public static UIInventory ConsumableInventory { get; private set; }

    public override void _Ready()
    {
        var canvasLayer = GetNode<CanvasLayer>("CanvasLayer");

        // Add UIPlayerInventory to canvas layer
        PlayerInventory = (UIInventory)Prefabs.Inventory.Instantiate();
        PlayerInventory.Init(
            parent: canvasLayer, 
            size: 18, 
            columns: 9, 
            LayoutPreset.CenterBottom);

        // Setup player inventory
        PlayerInventory.SetItem(0, new Item(Items.CoinPink, 5));
        PlayerInventory.SetItem(1, new Item(Items.CoinRed, 10));
        PlayerInventory.SetItem(2, new Item(Items.Coin, 1));
        
        PlayerInventory.SetItem(7, new Item(Items.PotionRed, 4));
        PlayerInventory.SetItem(8, new Item(Items.PotionBlue, 100));

        // Add filtered inventories
        CurrencyInventory = (UIInventory)Prefabs.Inventory.Instantiate();
		CurrencyInventory.Init(
            parent: canvasLayer, 
            size: 3,
			columns: 1, 
            LayoutPreset.CenterLeft, 
            ItemCategory.Currency);

        ConsumableInventory = (UIInventory)Prefabs.Inventory.Instantiate();
		ConsumableInventory.Init(
			parent: canvasLayer,
			size: 3,
			columns: 1, 
            LayoutPreset.CenterRight, 
            ItemCategory.Consumable);

        // Add UIOtherInventory to canvas layer
        OtherInventory = (UIInventory)Prefabs.Inventory.Instantiate();
		OtherInventory.Init(
			parent: canvasLayer,
			size: 9,
			columns: 9, 
            LayoutPreset.CenterTop);

        // Setup cursor
        var cursorParent = new Control { Name = "ParentCursor" };
        canvasLayer.AddChild(cursorParent);
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

    void HotbarHotkeys()
    {
        var inv = PlayerInventory;

        for (int i = 0; i < inv.Columns; i++)
            if (Input.IsActionJustPressed($"inventory_hotbar_{i + 1}"))
                if (Cursor.HasItem())
                {
                    var firstHotbarSlot = inv.NumSlots - inv.Columns;
                    Cursor.MoveAllTo(inv.UIInventorySlots[firstHotbarSlot + i]);
                    break;
                }
    }
}
