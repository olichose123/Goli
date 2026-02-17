namespace Goli.Resources;

using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using Goli.Utils;

public partial class SubResource
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
}
