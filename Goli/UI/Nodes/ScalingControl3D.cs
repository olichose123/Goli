namespace Starships;

using Godot;
using Goli.UI.Nodes;
using Goli.Utils;

[GlobalClass]
public partial class ScalingControl3D : Control3D
{
    [Export]
    public VisualInstance3D VisualInstance;

    public override void _Process(double delta)
    {
        base._Process(delta);

        Rect2 rect = Geometry.GetTargetScreenBounds(VisualInstance, Camera);
        Position -= rect.Size / 2;
        Size = rect.Size;
    }
}
