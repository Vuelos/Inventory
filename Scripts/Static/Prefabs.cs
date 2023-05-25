namespace Inventory;

public static class Prefabs
{
	public static PackedScene Inventory { get; } = Load("inventory");

	private static PackedScene Load(string path) =>
		GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}
