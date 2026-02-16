namespace Goli.Resources;

using System;
using Godot;
using Godot.Collections;
using Goli.Exceptions;

[GlobalClass, Tool]
public partial class SerializedLoader : ResourceFormatLoader
{
    public SerializedLoader() : base()
    {
        GD.Print("SerializedLoader initialized");
    }

    public override string[] _GetRecognizedExtensions()
    {
        return new string[] { "jsonr" };
    }

    public override string _GetResourceType(string path)
    {
        return "Resource";
    }

    public override bool _HandlesType(StringName type)
    {
        return ClassDB.IsParentClass(type, "Resource");
    }

    public override Variant _Load(string path, string originalPath, bool useSubThreads, int cacheMode)
    {
        GD.Print($"Loading resource from {path}");
        try
        {
            FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var data = Json.ParseString(file.GetAsText());
            file.Close();

            string resourceType = data.As<Dictionary>()["type"].AsString();
            var t = Type.GetType(resourceType, true, true);

            var res = Activator.CreateInstance(t) as SerializedResource;

            res.FromDictionary(data.As<Dictionary>());

            return res;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load resource from {path}: {e}");
            throw new CannotLoadResourceException($"Failed to load resource from {path}: {e}");
        }

    }
}
