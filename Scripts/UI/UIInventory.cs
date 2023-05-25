using static Godot.Control;

namespace Inventory;

public partial class UIInventory
{
    public bool Visible
    {
        get => SceneInv.Visible;
        set => SceneInv.Visible = value;
    }

    public Container Container { get; }
    public int SlotSize { get; } = 50;
    public UIInventorySlot[] UIInventorySlots { get; set; }
    public int Size { get; }
    public int Columns { get; }
    public SceneInventory SceneInv { get; private set; }

    protected Tween Tween { get; set; }
    protected bool Animating { get; set; }

    public UIInventory(Node parent, int size, int columns, ItemCategory? itemCategoryFilter = null)
    {
        Size = size;
        Columns = columns;
        Container = new Container(size);

		SceneInv = (SceneInventory)Prefabs.Inventory.Instantiate();
        SceneInv.Columns = columns;
        parent.AddChild(SceneInv);

		// Setup inventory slots
		UIInventorySlots = new UIInventorySlot[Container.Items.Length];

		for (int i = 0; i < Container.Items.Length; i++)
			UIInventorySlots[i] = new UIInventorySlot(this, i, itemCategoryFilter);
	}

    public void Update() => UIInventorySlots.ForEach(x => x.Update());

    public void SetItem(int i, Item item) => UIInventorySlots[i].Set(item);
    public void SetItem(int x, int y, Item item) => 
        UIInventorySlots[x + y * Columns].Set(item);
	
    public Vector2 GetSlotPosition(int i) => 
        UIInventorySlots[i].Parent.GlobalPosition + Vector2.One * (SlotSize / 2);

    public void SetSlotsVisibility(int a, int b, bool visible, bool anchor)
    {
        for (int i = a; i < b; i++)
            UIInventorySlots[i].Visible = visible;

        if (anchor)
            SceneInv.SetAnchor(SceneInv.CurLayoutPreset);
    }

    public void AnimateHide(double duration = 1) => Transition(duration, false, false);
    public void AnimateShow(double duration = 1) => Transition(duration, true, false);

    protected void Transition(double duration = 1, bool entering = false, bool extended = false)
    {
        Animating = true;

        Tween = SceneInv.GetTree().CreateTween();

        var finalValue = SceneInv.CurLayoutPreset == LayoutPreset.CenterTop ?
            -SceneInv.PanelContainer.Size.Y : SceneInv.PanelContainer.Size.Y;

        if (entering)
            finalValue *= -1;

        Tween.TweenProperty(SceneInv.PanelContainer, "position:y", finalValue, duration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);

        if (!extended)
            Tween.TweenCallback(Callable.From(() => Animating = false));
    }
}
