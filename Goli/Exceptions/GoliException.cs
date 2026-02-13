namespace Goli.Exceptions;

using Godot;

public class GoliException : Exception
{
    public GoliException(string message) : base(message)
    {
        GD.PrintErr($"GoliException: {message}");
    }
}

public class MissingPropertyException : GoliException
{
    public MissingPropertyException(string message) : base(message)
    {
    }
}
