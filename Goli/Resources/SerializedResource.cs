#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8601 // Possible null reference assignment.

namespace Goli.Resources;

using System;
using System.Linq;
using Godot;
using Godot.Collections;
using Goli.Utils;

[GlobalClass, Tool]
public partial class SerializedResource : Resource
{
    static Godot.Collections.Array propertiesToSkip = new Godot.Collections.Array { "Name", "RefCounted", "metadata/_custom_type_script", "script", "SerializedResource" };
    public string ResourceType { get; private set; }

    Dictionary subResources = new Dictionary();
    Dictionary subResourceTypes = new Dictionary();
    Dictionary subResourceClasses = new Dictionary();
    Dictionary subResourceHints = new Dictionary();
    Dictionary subResourceHintStrings = new Dictionary();

    [Export]
    public string Name
    {
        get => this.ResourceName;
        set => this.ResourceName = value;
    }

    public SerializedResource() : base()
    {
        ResourceType = GetType().FullName;
        HandleSubresources();
    }

    void HandleSubresources()
    {
        var type = GetType();
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (SubResource.IsChildOfSubResource(field.FieldType))
            {
                handleSubresource(field.Name, field.FieldType);
            }
        }
    }

    void handleSubresource(string name, Type subResType)
    {
        var dict = new Dictionary();
        var types = new Dictionary();
        var classes = new Dictionary();
        var hints = new Dictionary();
        var hintStrings = new Dictionary();
        subResources[name] = dict;
        subResourceTypes[name] = types;
        subResourceClasses[name] = classes;
        subResourceHints[name] = hints;
        subResourceHintStrings[name] = hintStrings;

        var fields = subResType.GetFields();
        foreach (var field in fields)
        {
            if (SubResource.IsChildOfSubResource(field.FieldType))
            {
                handleSubresource(name + "/" + field.Name, field.FieldType);
            }
            else
            {
                var subFields = SubResource.GetExportedFields(subResType);
                foreach (var subField in subFields)
                {
                    try
                    {
                        if (!field.FieldType.IsSubclassOf(typeof(GodotObject)))
                        {
                            var defaultValue = subField.GetValue(Activator.CreateInstance(subResType));
                            dict[subField.Name] = Variants.ConvertToVariant(defaultValue);
                        }
                        else
                        {
                            dict[subField.Name] = default;
                        }

                    }
                    catch (Exception e)
                    {
                        dict[subField.Name] = default;
                    }

                    types[subField.Name] = (int)Variants.GetVariantType(subField.FieldType);
                    if (subField.FieldType.IsSubclassOf(typeof(GodotObject)))
                    {
                        classes[subField.Name] = subField.FieldType.Name;
                        hints[subField.Name] = (int)PropertyHint.ResourceType;
                        hintStrings[subField.Name] = subField.FieldType.Name;

                    }
                    else
                    {
                        classes[subField.Name] = "";
                        hints[subField.Name] = "";
                        hintStrings[subField.Name] = "";
                    }
                }
            }
        }
    }


    public override Array<Dictionary> _GetPropertyList()
    {
        // handle subresources
        var properties = new Array<Dictionary>();
        foreach (var subRes in subResources)
        {
            foreach (var subResField in (Dictionary)subRes.Value)
            {
                var dict = new Dictionary();
                dict["name"] = subRes.Key + "/" + subResField.Key;
                dict["type"] = (int)((Dictionary)subResourceTypes[subRes.Key])[subResField.Key];
                dict["hint"] = ((Dictionary)subResourceHints[subRes.Key])[subResField.Key];
                dict["hint_string"] = ((Dictionary)subResourceHintStrings[subRes.Key])[subResField.Key];
                dict["class_name"] = ((Dictionary)subResourceClasses[subRes.Key])[subResField.Key];

                properties.Add(dict);
            }
        }
        return properties;
    }

    public override Variant _Get(StringName property)
    {
        if (isClassProperty(property))
        {
            return default;
        }

        // split property into subresource and field

        string[] split = property.ToString().Split("/");
        var subresName = string.Join("/", split.Take(split.Length - 1));
        var fieldName = split.Last();

        if (split.Length > 1)
        {
            var subres = (Dictionary)subResources[subresName];
            return subres[fieldName];
        }
        else
        {
            GD.PrintErr($"Property {property} not found in SerializedResource of type {GetType().Name}");
            return default;
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (isClassProperty(property))
        {
            return false;
        }

        string[] split = property.ToString().Split("/");
        var subresName = string.Join("/", split.Take(split.Length - 1));
        var fieldName = split.Last();
        if (split.Length > 1)
        {
            var subres = (Dictionary)subResources[subresName];
            subres[fieldName] = value;
            return true;
        }
        else
        {
            GD.PrintErr($"Property {property} not found in SerializedResource of type {GetType().Name}");
            return false;
        }
    }

    bool hasClass(Dictionary prop)
    {
        return prop["class_name"].AsString() != "";
    }

    bool isSerializedResourceClass(Dictionary prop)
    {
        if (hasClass(prop))
        {
            var className = prop["class_name"].AsString();
            var type = Type.GetType(className, false, true);
            if (type != null)
            {
                return type.IsSubclassOf(typeof(SerializedResource));
            }
        }
        return false;
    }

    public Dictionary ToDictionary()
    {
        if (ResourceName == "")
        {
            GD.PrintErr($"Cannot serialize resource of type {GetType().Name} with empty name");
            throw new Goli.Exceptions.UnnamedResourceException($"Cannot serialize resource of type {GetType().Name} with empty name");
        }

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

            var split = propName.Split("/");
            var value = Get(propName);
            if (split.Length > 1)
            {
                var currentDict = dict;
                for (int i = 0; i < split.Length - 1; i++)
                {
                    if (!currentDict.ContainsKey(split[i]))
                    {
                        currentDict[split[i]] = new Dictionary();
                    }
                    currentDict = (Dictionary)currentDict[split[i]];
                }

                var resValue = value.As<Resource>();
                if (resValue != null)
                {
                    currentDict[split[^1]] = resValue.ResourcePath;
                }
                else
                {
                    currentDict[split[^1]] = Get(propName);
                }
            }
            else
            {
                var resValue = value.As<Resource>();
                if (resValue != null)
                {
                    dict[propName] = resValue.ResourcePath;
                }
                else
                {
                    dict[propName] = Get(propName);
                }
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

            var split = propName.Split("/");
            if (split.Length > 1)
            {
                var currentDict = dict;
                for (int i = 0; i < split.Length - 1; i++)
                {
                    if (!currentDict.ContainsKey(split[i]))
                    {
                        continue;
                    }
                    currentDict = (Dictionary)currentDict[split[i]];
                }

                var value = currentDict[split[^1]];

                if (hasClass(prop))
                {
                    var path = currentDict[split[^1]].AsString();
                    if (path != null && path != "")
                    {
                        Set(propName, ResourceLoader.Load(path, "", ResourceLoader.CacheMode.Replace));
                        continue;
                    }
                }
                Set(propName, value);
            }
            else
            {
                if (hasClass(prop))
                {
                    var path = dict[propName].AsString();
                    if (path != null && path != "")
                    {
                        Set(propName, ResourceLoader.Load(path, "", ResourceLoader.CacheMode.Replace));
                        continue;
                    }
                }
                Set(propName, dict[propName]);
            }
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
