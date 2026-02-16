namespace Goli.Resources;

using System;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class SerializedResource : Resource
{
    static Godot.Collections.Array propertiesToSkip = new Godot.Collections.Array { "RefCounted", "metadata/_custom_type_script", "script", "SerializedResource" };
    public string ResourceType { get; private set; }

    public SerializedResource() : base()
    {
        ResourceType = GetType().FullName;
    }

    public Dictionary ToDictionary()
    {
        var dict = new Dictionary();
        dict["type"] = ResourceType;
        dict["name"] = this.ResourceName;
        foreach (var prop in GetPropertyList())
        {
            string propName = prop["name"].AsString();
            if (isClassProperty(propName))
            {
                continue;
            }

            var value = Get(propName);
            var resValue = value.As<Resource>();
            if (resValue != null)
            {
                dict[propName] = resValue.ResourcePath;
                //dict[propName] = value;
            }
            else
            {
                dict[propName] = value;
            }

        }

        return dict;
    }

    public void FromDictionary(Dictionary dict)
    {
        if (dict.ContainsKey("type"))
        {
            if (dict["type"].AsString() != ResourceType)
            {
                GD.PrintErr($"Resource type mismatch: expected {ResourceType}, got {dict["ResourceType"]}");
                return;
            }
        }
        foreach (var prop in GetPropertyList())
        {
            string propName = prop["name"].AsString();
            if (isClassProperty(propName))
            {
                continue;
            }
            if (prop["class_name"].AsString() != "")
            {
                var path = dict[propName].AsString();
                Set(propName, ResourceLoader.Load(path));
            }
            else
            {
                Set(propName, dict[propName]);
            }
        }
        this.ResourceName = dict["name"].AsString();
    }

    bool isClassProperty(string property)
    {
        var propList = ClassDB.ClassGetPropertyList("Resource");
        foreach (var prop in propList)
        {
            if (prop["name"].AsString() == property)
            {
                return true;
            }
        }
        if (propertiesToSkip.Contains(property))
        {
            return true;
        }
        if (property == this.GetType().Name)
        {
            return true;
        }
        return false;
    }
}
