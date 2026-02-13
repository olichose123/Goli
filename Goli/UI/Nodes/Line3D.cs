namespace Goli.UI.Nodes;

using System;
using Godot;
using Goli.Utils;

/// <summary>
/// Draws a 3D line between two Node3D points, always facing the specified Camera3D.
/// </summary>
[GlobalClass, Tool]
public partial class Line3D : Node3D
{
    [Export]
    public Camera3D Camera;

    [Export]
    public Node3D Origin;

    [Export]
    public Node3D Target;

    [Export]
    public float LineWidth = 0.05f;

    [Export]
    public StandardMaterial3D Material = new StandardMaterial3D()
    {
        AlbedoColor = Color.Color8(0, 255, 0),
        BlendMode = BaseMaterial3D.BlendModeEnum.Add,
        CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
    };

    private Vector3 relativeStart;
    private Vector3 relativeEnd;
    private MeshInstance3D meshInstance;
    private ImmediateMesh mesh;

    public override void _Ready()
    {
        if (Camera == null)
            throw new Exceptions.MissingPropertyException("Missing Camera property for Line3D node.");

        if (Origin == null)
            throw new Exceptions.MissingPropertyException("Missing Origin property for Line3D node.");

        if (Target == null)
            throw new Exceptions.MissingPropertyException("Missing Target property for Line3D node.");


        base._Ready();

        meshInstance = new MeshInstance3D();
        AddChild(meshInstance);

        mesh = new ImmediateMesh();
        meshInstance.Mesh = mesh;
        meshInstance.CastShadow = MeshInstance3D.ShadowCastingSetting.Off;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        mesh.ClearSurfaces();
        Draw();

    }

    public void Draw()
    {
        if (Origin == null || Target == null || Camera == null) return;

        GlobalPosition = Origin.GlobalPosition;
        Vector3 endPos = Target.GlobalPosition - Origin.GlobalPosition;

        mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles, Material);
        if (Engine.IsEditorHint())
        {
            Geometry.DrawLine3D(GlobalPosition, endPos, LineWidth, mesh, GetViewport().GetCamera3D());
        }
        else
        {
            Geometry.DrawLine3D(GlobalPosition, endPos, LineWidth, mesh, Camera);
        }

        mesh.SurfaceEnd();
    }

}
