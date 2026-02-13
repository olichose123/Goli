namespace Goli.UI.Nodes;

using Godot;

/// <summary>
/// A Control that positions itself in screen space based on a 3D Target Node3D and Camera3D.
/// </summary>
[GlobalClass]
public partial class Control3D : Control
{
    [Export]
    public Camera3D Camera;

    [Export]
    public Node3D Target;

    [Export]
    public Vector2 Offset = Vector2.Zero;

    [Export]
    bool ModifyGlobalPosition = false;

    [Export]
    bool HideWhenBehindCamera = true;

    [Export]
    bool HideWhenLargerThanViewport = true;

    public override void _Ready()
    {
        if (Camera == null)
            throw new Exceptions.MissingPropertyException("Missing Camera property for Control3D node.");

        if (Target == null)
            throw new Exceptions.MissingPropertyException("Missing Target property for Control3D node.");

        base._Ready();
    }

    bool isBehindCamera()
    {
        return Camera.IsPositionBehind(Target.GlobalPosition);
    }

    Vector2 unprojectTarget()
    {
        return Camera.UnprojectPosition(Target.GlobalPosition);
    }

    public override void _Process(double delta)
    {
        if (HideWhenLargerThanViewport && Size > Camera.GetViewport().GetVisibleRect().Size)
            Hide();
        else
            Show();

        if (HideWhenBehindCamera && isBehindCamera())
            Hide();
        else
            Show();

        if (ModifyGlobalPosition)
            GlobalPosition = unprojectTarget() + Offset;
        else
            Position = unprojectTarget() + Offset;

        base._Process(delta);
    }
}
