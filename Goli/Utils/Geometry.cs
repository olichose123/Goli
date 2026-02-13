namespace Goli.Utils;

using Godot;
using Godot.Collections;

public class Geometry
{
    public static Rect2 GetTargetScreenBounds(VisualInstance3D target, Camera3D camera)
    {
        Aabb localAabb = target.GetAabb();
        Transform3D globalTransform = target.GlobalTransform;

        Vector3[] corners = new Vector3[8];
        Vector3 size = localAabb.Size;
        Vector3 pos = localAabb.Position;

        corners[0] = globalTransform * pos;
        corners[1] = globalTransform * (pos + new Vector3(size.X, 0, 0));
        corners[2] = globalTransform * (pos + new Vector3(0, size.Y, 0));
        corners[3] = globalTransform * (pos + new Vector3(0, 0, size.Z));
        corners[4] = globalTransform * (pos + new Vector3(size.X, size.Y, 0));
        corners[5] = globalTransform * (pos + new Vector3(size.X, 0, size.Z));
        corners[6] = globalTransform * (pos + new Vector3(0, size.Y, size.Z));
        corners[7] = globalTransform * (pos + size);

        Vector2 firstScreenPos = camera.UnprojectPosition(corners[0]);
        Vector2 min = firstScreenPos;
        Vector2 max = firstScreenPos;

        for (int i = 1; i < 8; i++)
        {
            if (camera.IsPositionBehind(corners[i])) continue;

            Vector2 screenPos = camera.UnprojectPosition(corners[i]);

            min.X = Mathf.Min(min.X, screenPos.X);
            min.Y = Mathf.Min(min.Y, screenPos.Y);
            max.X = Mathf.Max(max.X, screenPos.X);
            max.Y = Mathf.Max(max.Y, screenPos.Y);
        }

        return new Rect2(min, max - min);
    }

    /// <summary>
    /// Draw a line in 3d space that always faces the camera
    /// </summary>
    /// <param name="start">The global position of the line's origin</param>
    /// <param name="end">The global position of the line's end</param>
    /// <param name="lineWidth"></param>
    /// <param name="mesh"></param>
    /// <param name="camera"></param>
    public static void DrawLine3D(Vector3 start, Vector3 end, float lineWidth, ImmediateMesh mesh, Camera3D camera)
    {
        Vector3 lineDir = end.Normalized();
        Vector3 midPoint = end / 2.0f;
        Vector3 toCamera;

        toCamera = (camera.GlobalPosition - (start + midPoint)).Normalized();

        Vector3 side = lineDir.Cross(toCamera).Normalized();

        if (side.IsZeroApprox())
        {
            side = lineDir.Cross(Vector3.Up).Normalized();
        }

        side *= lineWidth / 2.0f;
        Vector3 p1 = -side;
        Vector3 p2 = side;
        Vector3 p3 = end - side;
        Vector3 p4 = end + side;

        mesh.SurfaceSetUV(new Vector2(0, 0));
        mesh.SurfaceAddVertex(p1);
        mesh.SurfaceSetUV(new Vector2(0, 1));
        mesh.SurfaceAddVertex(p3);
        mesh.SurfaceSetUV(new Vector2(1, 1));
        mesh.SurfaceAddVertex(p4);

        mesh.SurfaceSetUV(new Vector2(1, 1));
        mesh.SurfaceAddVertex(p4);
        mesh.SurfaceSetUV(new Vector2(1, 0));
        mesh.SurfaceAddVertex(p2);
        mesh.SurfaceSetUV(new Vector2(0, 0));
        mesh.SurfaceAddVertex(p1);
    }
}
