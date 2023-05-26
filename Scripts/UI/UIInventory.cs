namespace Inventory;

public partial class UIInventory : Control
{
	public Container Container { get; private set; }
	public UIInventorySlot[] UIInventorySlots { get; private set; }
	public int SlotPixelSize { get; } = 50;
	public int NumSlots { get; private set; }

	public LayoutPreset CurLayoutPreset { get; private set; } = LayoutPreset.CenterTop;
	public GridContainer GridContainer { get; private set; }
	public int Columns { get; private set; } = 9;

	protected Tween tween;
	protected bool animating;

	private PanelContainer panel;

	public virtual void Init(Node parent, int size, int columns, LayoutPreset layoutPreset, ItemCategory? itemCategoryFilter = null)
	{
		NumSlots = size;
		Columns = columns;
		Container = new Container(size);

		parent.AddChild(this);
		SetAnchor(layoutPreset);

		// Setup inventory slots
		UIInventorySlots = new UIInventorySlot[Container.Items.Length];

		for (int i = 0; i < Container.Items.Length; i++)
			UIInventorySlots[i] = new UIInventorySlot(this, i, itemCategoryFilter);
	}

	public override void _Ready()
	{
		panel = GetNode<PanelContainer>("PanelContainer");
		GridContainer = panel.GetNode<GridContainer>
			("MarginContainer/GridContainer");

		GridContainer.Columns = Columns;
	}

	public void HideBackPanel() =>
		panel.AddThemeStyleboxOverride(
			name: "panel", 
			stylebox: new StyleBoxEmpty());

	public void ShowBackPanel() =>
		panel.AddThemeStyleboxOverride(
			name: "panel", 
			stylebox: panel.GetThemeStylebox("panel"));

	public void SetAnchor(LayoutPreset preset)
	{
		CurLayoutPreset = preset;

		// Set for control pivot
		SetAnchorsAndOffsetsPreset(preset);

		// Set for panel container
		panel.SetAnchorsAndOffsetsPreset(preset);
	}

	public void Update() => UIInventorySlots.ForEach(x => x.Update());

	public void SetItem(int i, Item item) => UIInventorySlots[i].Set(item);
	public void SetItem(int x, int y, Item item) =>
		UIInventorySlots[x + y * Columns].Set(item);

	public Vector2 GetSlotPosition(int i) =>
		UIInventorySlots[i].Parent.GlobalPosition + Vector2.One * (SlotPixelSize / 2);

	public void SetSlotsVisibility(int a, int b, bool visible, bool anchor)
	{
		for (int i = a; i < b; i++)
			UIInventorySlots[i].Visible = visible;

		if (anchor)
			SetAnchor(CurLayoutPreset);
	}

	public void AnimateHide(double duration = 1) => Transition(duration, false, false);
	public void AnimateShow(double duration = 1) => Transition(duration, true, false);

	public void SwitchToHotbarInstant()
	{
		HideBackPanel();
		SetSlotsVisibility(0, UIInventorySlots.Length - Columns, false, true);
	}

	public void SwitchToFullInventoryInstant()
	{
		ShowBackPanel();
		SetSlotsVisibility(0, UIInventorySlots.Length - Columns, true, true);
	}

	public void SwitchToHotbarAnimated(double exitTime = 1, double reEntryTime = 1) =>
		SwitchAnimated(exitTime, reEntryTime, () => SwitchToHotbarInstant());

	public void SwitchToFullInventoryAnimated(double exitTime = 1, double reEntryTime = 1) =>
		SwitchAnimated(exitTime, reEntryTime, () => SwitchToFullInventoryInstant());

	protected void Transition(double duration = 1, bool entering = false, bool extended = false)
	{
		animating = true;

		tween = GetTree().CreateTween();

		var finalValue = CurLayoutPreset == LayoutPreset.CenterTop ?
			-panel.Size.Y : panel.Size.Y;

		if (entering)
			finalValue *= -1;

		tween.TweenProperty(panel, "position:y", finalValue, duration)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.InOut);

		if (!extended)
			tween.TweenCallback(Callable.From(() => animating = false));
	}

	void SwitchAnimated(double exitTime = 1, double reEntryTime = 1, Action action = default)
	{
		Transition(exitTime, false, true);

		tween.TweenCallback(Callable.From(() =>
		{
			action();

			// SetAnchor() mucks up position so lets reset it
			panel.Position = new Vector2(
				x: panel.Position.X,
				y: 0);

			panel.SortChildren += animateEnter;
		}));

		void animateEnter()
		{
			panel.SortChildren -= animateEnter;

			Transition(reEntryTime, true);
		}
	}
}
