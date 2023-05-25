﻿namespace Inventory;

public class UIItem
{
    public Vector2 Position
    {
        get => Parent.Position;
        set => Parent.Position = value;
    }

    public Control Parent { get; set; }

    private readonly Node2D sprite;
    private readonly Label label;

    public UIItem(Node parent, Item item)
    {
        Parent = new Control();

        // Need 'centered' bool because cursor is offset by (InvSlotSize / 2) on both axis
        var centered = parent.Name != "ParentCursor";

        // Create sprite
        sprite = item.Type.GenerateGraphic();
        sprite.Position = centered ? Vector2.One * (50 / 2) : Vector2.Zero;
        sprite.Scale = Vector2.One * 2;
        Parent.AddChild(sprite);

        // Create count label
        var marginContainer = new MarginContainer
        {
            CustomMinimumSize = Vector2.One * 50,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Position = centered ? Vector2.Zero : Vector2.One * -25,
            Visible = item.Count != 1
        };
        marginContainer.AddThemeConstantOverride("margin_left", 3);

        label = new Label
        {
            Text = item.Count + "",
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Bottom,
            SizeFlagsVertical = Control.SizeFlags.Fill,
            MouseFilter = Control.MouseFilterEnum.Ignore // ignored by default but just in case Godot changes it in the future
        };
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_outline_size", 3);
        label.AddThemeFontSizeOverride("font_size", 20);

        marginContainer.AddChild(label);
        Parent.AddChild(marginContainer);
        parent.AddChild(Parent);
    }

    public void SetText(string text) => label.Text = text;

    public void Hide()
    {
        // not sure why its not valid all the time
        if (GodotObject.IsInstanceValid(label))
            label.Hide();
        
        sprite.Hide();
    }

    public void Show()
    {
        // not sure why its not valid all the time
        if (GodotObject.IsInstanceValid(label))
            label.Show();
        sprite.Show();
    }
}
