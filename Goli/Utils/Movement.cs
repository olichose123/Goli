namespace Goli.Utils;

using Godot;

public class Movement
{
    /// <summary>
    /// Calculates velocity based on input direction and camera orientation for left/right and forward/backward movement. If no input, it decelerates towards zero.
    /// </summary>
    /// <param name="initialVelocity"></param>
    /// <param name="MovementSpeed"></param>
    /// <param name="inputDirection"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Vector3 GetBasicVelocityLeftRightForwardBackward(Vector3 initialVelocity, float MovementSpeed, Vector2 inputDirection, Camera3D camera)
    {
        Vector3 velocity = initialVelocity;
        Vector3 direction = (camera.Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * MovementSpeed;
            velocity.Z = direction.Z * MovementSpeed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(initialVelocity.X, 0, MovementSpeed);
            velocity.Z = Mathf.MoveToward(initialVelocity.Z, 0, MovementSpeed);
        }
        return velocity;
    }

    /// <summary>
    /// Calculates velocity based on input direction for up/down movement, without taking into account camera angle. If no input, it decelerates towards zero.
    /// </summary>
    /// <param name="initialVelocity"></param>
    /// <param name="MovementSpeed"></param>
    /// <param name="inputDirection"></param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Vector3 GetBasicVelocityUpDown(Vector3 initialVelocity, float MovementSpeed, Vector2 inputDirection, Transform3D transform)
    {
        Vector3 velocity = initialVelocity;
        Vector3 direction = (transform.Basis * new Vector3(0, inputDirection.X, 0)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.Y = direction.Y * MovementSpeed;
        }
        else
        {
            velocity.Y = Mathf.MoveToward(initialVelocity.Y, 0, MovementSpeed);
        }
        return velocity;
    }
}
