namespace Inventory;

public partial class SceneInventory : Control
{
	public LayoutPreset CurLayoutPreset { get; private set; } = LayoutPreset.CenterTop;
	public PanelContainer PanelContainer { get; private set; }
	public GridContainer GridContainer { get; private set; }

	public int Columns { get; set; } = 9;

	public override void _Ready()
	{
		PanelContainer = GetNode<PanelContainer>("PanelContainer");
		GridContainer = PanelContainer.GetNode<GridContainer>
			("MarginContainer/GridContainer");

		GridContainer.Columns = Columns;
	}

	public void HideBackPanel() =>
		PanelContainer.AddThemeStyleboxOverride(
			name: "panel", 
			stylebox: new StyleBoxEmpty());

	public void ShowBackPanel() =>
		PanelContainer.AddThemeStyleboxOverride(
			name: "panel", 
			stylebox: PanelContainer.GetThemeStylebox("panel"));

	public void SetAnchor(LayoutPreset preset)
	{
		CurLayoutPreset = preset;

		// Set for control pivot
		SetAnchorsAndOffsetsPreset(preset);

		// Set for panel container
		PanelContainer.SetAnchorsAndOffsetsPreset(preset);
	}
}
