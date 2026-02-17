namespace Goli.Resources;

using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using Goli.Utils;

public partial class SubResource : GodotObject
{

    /// <summary>
    /// Return true if the given type is a subclass of SubResource
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsChildOfSubResource(Type type)
    {
        return typeof(SubResource).IsAssignableFrom(type);
    }

    /// <summary>
    /// Return a list of fields that have the Export attribute
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<FieldInfo> GetExportedFields(Type type)
    {
        List<FieldInfo> exportedFields = new List<FieldInfo>();
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<ExportAttribute>();
            if (attr != null)
            {
                exportedFields.Add(field);
            }
        }
        return exportedFields;
    }


    public SubResource() : base()
    {
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        var properties = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        var fields = SubResource.GetExportedFields(GetType());

        foreach (var field in fields)
        {
            var dict = new Godot.Collections.Dictionary();
            dict["name"] = field.Name;
            dict["type"] = (int)Variants.GetVariantType(field.FieldType);
            dict["hint"] = "";
            dict["hint_string"] = "";
            properties.Add(dict);
        }
        return properties;
    }

    public override Variant _Get(StringName property)
    {
        var field = GetType().GetField(property);
        if (field != null)
        {
            return Variants.ConvertToVariant(field.GetValue(this));
        }
        else
        {
            GD.PrintErr($"Property {property} not found in SubResource of type {GetType().Name}");
            return default;
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        var field = GetType().GetField(property);
        if (field != null)
        {

            field.SetValue(this, Convert.ChangeType(value.Obj, field.FieldType));
            return true;
        }
        else
        {
            GD.PrintErr($"Property {property} not found in SubResource of type {GetType().Name}");
        }
        return false;
    }

}
