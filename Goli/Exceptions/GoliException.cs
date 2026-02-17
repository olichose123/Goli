namespace Goli.Exceptions;

using System;
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

public class ResourceDoesNotExistException : GoliException
{
    public ResourceDoesNotExistException(string message) : base(message)
    {
    }
}

public class ResourceAlreadyExistsException : GoliException
{
    public ResourceAlreadyExistsException(string message) : base(message)
    {
    }
}

public class CannotLoadResourceException : GoliException
{
    public CannotLoadResourceException(string message) : base(message)
    {
    }
}

public class CannotSaveResourceException : GoliException
{
    public CannotSaveResourceException(string message) : base(message)
    {
    }
}

public class UnnamedResourceException : GoliException
{
    public UnnamedResourceException(string message) : base(message)
    {
    }
}
