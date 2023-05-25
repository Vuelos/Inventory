using static Godot.Control;

namespace Inventory;

public class UIPlayerInventory : UIInventory
{
    public UIPlayerInventory(Node parent, int size, int columns) : base(parent, size, columns)
    {
		SceneInv.SetAnchor(LayoutPreset.CenterBottom);
    }

    public void SwitchToHotbarInstant()
    {
		SceneInv.HideBackPanel();
        SetSlotsVisibility(0, UIInventorySlots.Length - Columns, false, true);
    }

    public void SwitchToFullInventoryInstant()
    {
		SceneInv.ShowBackPanel();
        SetSlotsVisibility(0, UIInventorySlots.Length - Columns, true, true);
    }

    public void SwitchToHotbarAnimated(double exitTime = 1, double reEntryTime = 1) =>
        SwitchAnimated(exitTime, reEntryTime, () => SwitchToHotbarInstant());

    public void SwitchToFullInventoryAnimated(double exitTime = 1, double reEntryTime = 1) =>
        SwitchAnimated(exitTime, reEntryTime, () => SwitchToFullInventoryInstant());

    private void SwitchAnimated(double exitTime = 1, double reEntryTime = 1, Action action = default)
    {
        Transition(exitTime, false, true);

        tween.TweenCallback(Callable.From(() =>
        {
            action();

            // SetAnchor() mucks up position so lets reset it
            SceneInv.PanelContainer.Position = new Vector2(
                x: SceneInv.PanelContainer.Position.X, 
                y: 0);

			SceneInv.PanelContainer.SortChildren += animateEnter;
        }));

        void animateEnter()
        {
			SceneInv.PanelContainer.SortChildren -= animateEnter;

            Transition(reEntryTime, true);
        }
    }
}
