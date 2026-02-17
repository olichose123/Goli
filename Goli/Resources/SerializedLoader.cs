namespace Goli.Resources;

using System;
using Godot;
using Godot.Collections;
using Goli.Exceptions;

[GlobalClass, Tool]
public partial class SerializedLoader : ResourceFormatLoader
{
    static string extension = "jsonr";

    public SerializedLoader() : base()
    {
        GD.Print("SerializedLoader initialized");
    }

    public override string[] _GetRecognizedExtensions()
    {
        return new string[] { extension };
    }

    public override string _GetResourceType(string path)
    {
        if (path.GetExtension() != extension)
        {
            return "";
        }
        return "Resource";
    }

    public override bool _HandlesType(StringName type)
    {
        return type == "Resource";
    }

    public override Variant _Load(string path, string originalPath, bool useSubThreads, int cacheMode)
    {
        try
        {
            FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var data = Json.ParseString(file.GetAsText());
            file.Close();

            string resourceType = data.As<Dictionary>()["type"].AsString();
            var t = Type.GetType(resourceType, true, true);

            var res = Activator.CreateInstance(t) as SerializedResource;
            if (res == null)
            {
                GD.PrintErr($"Failed to create instance of type {resourceType}");
                throw new CannotLoadResourceException($"Failed to create instance of type {resourceType}");
            }
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
