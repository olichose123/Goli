namespace Goli.Utils;

using Godot;
using Godot.Collections;

public class Geometry
{
    public static Rect2 GetTargetScreenBounds(VisualInstance3D target, Camera3D camera)
    {
        // get aabb and set global position
        Aabb bounds = target.GetAabb();
        bounds.Position += target.GlobalPosition + bounds.Size / 2;

        // build a list of planes representing a box based on aabb
        Array<Plane> planes = Geometry3D.BuildBoxPlanes(bounds.Size);

        // fetch each corner of the box
        var corners = new Array<Vector3>();
        foreach (var plane in planes)
        {
            var corner = bounds.Position + plane.Normal * bounds.Size;
            corners.Add(corner);
        }

        // determine top-left-most and bottom-right-most screen positions
        Vector2 minScreen = Vector2.Zero;
        Vector2 maxScreen = Vector2.Zero;

        foreach (var corner in corners)
        {
            var screenPos = camera.UnprojectPosition(corner);
            if (minScreen == Vector2.Zero)
                minScreen = screenPos;
            if (maxScreen == Vector2.Zero)
                maxScreen = screenPos;
            minScreen = new Vector2(Mathf.Min(minScreen.X, screenPos.X), Mathf.Min(minScreen.Y, screenPos.Y));
            maxScreen = new Vector2(Mathf.Max(maxScreen.X, screenPos.X), Mathf.Max(maxScreen.Y, screenPos.Y));
        }

        return new Rect2(minScreen, maxScreen - minScreen);
    }
}
