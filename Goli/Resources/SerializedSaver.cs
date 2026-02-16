namespace Goli.Resources;

using System;
using Godot;
using Goli.Exceptions;

[GlobalClass, Tool]
public partial class SerializedSaver : ResourceFormatSaver
{
    public SerializedSaver() : base()
    {
        GD.Print("SerializedSaver initialized");
    }


    public override string[] _GetRecognizedExtensions(Resource resource)
    {
        return new string[] { "jsonr" };
    }

    public override bool _Recognize(Resource resource)
    {
        var res = resource as SerializedResource;
        if (res == null)
        {
            GD.PrintErr($"Resource {resource.ResourceName} is not of type SerializedResource and cannot be saved as JSON");
            return false;
        }
        return true;
    }

    public override Error _Save(Resource resource, string path, uint flags)
    {
        try
        {
            FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            file.StoreLine(Json.Stringify(((SerializedResource)resource).ToDictionary(), "  "));
            file.Close();
            return Error.Ok;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to save resource to {path}: {e}");
            return Error.FileCantWrite;
        }
    }
}
