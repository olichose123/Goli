namespace Goli.Resources;

using System;
using System.Reflection;
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

    public Dictionary ToDictionary(bool anonymous = false)
    {
        var dict = new Dictionary();
        if (!anonymous)
        {
            dict["type"] = ResourceType;
            dict["name"] = this.ResourceName;
        }
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
                if (resValue.GetType().IsSubclassOf(typeof(SerializedResource)))
                {
                    var field = GetType().GetField(propName);
                    var p = GetType().GetProperty(propName);
                    if ((field != null && field.GetCustomAttribute<LocalAttribute>() != null) ||
                        (p != null && p.GetCustomAttribute<LocalAttribute>() != null))
                    {
                        dict[propName] = ((SerializedResource)resValue).ToDictionary(true);
                        continue;
                    }
                }
                dict[propName] = resValue.ResourcePath;
            }
            else
            {
                dict[propName] = value;
            }
        }

        return dict;
    }

    public void FromDictionary(Dictionary dict, bool anonymous = false)
    {
        if (!anonymous && dict.ContainsKey("type"))
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
                var type = GetType();
                var field = GetType().GetField(propName);
                var p = GetType().GetProperty(propName);
                if ((field != null && field.GetCustomAttribute<LocalAttribute>() != null) ||
                        (p != null && p.GetCustomAttribute<LocalAttribute>() != null))
                {
                    Type? t = null;
                    if (field != null)
                    {
                        t = field.FieldType;
                    }
                    else if (p != null)
                    {
                        t = p.PropertyType;
                    }
                    if (t != null)
                    {
                        if (t.IsSubclassOf(typeof(SerializedResource)))
                        {
                            var obj = Activator.CreateInstance(t) as SerializedResource;
                            obj.FromDictionary(dict[propName].As<Dictionary>(), true);
                            Set(propName, obj);
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    var path = dict[propName].AsString();
                    if (path != null && path != "")
                        Set(propName, ResourceLoader.Load(path));
                }
            }
            else
            {
                Set(propName, dict[propName]);
            }

            if (!anonymous)
                this.ResourceName = dict["name"].AsString();
        }
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
