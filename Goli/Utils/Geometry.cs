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
}
