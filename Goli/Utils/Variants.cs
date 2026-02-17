namespace Goli.Utils;

using System;
using System.Collections.Generic;
using Godot;
using Goli.Resources;

public static class Variants
{
    static Dictionary<Type, Variant.Type> typeMap = new Dictionary<Type, Variant.Type>
    {
        { typeof(int), Variant.Type.Int },
        { typeof(uint), Variant.Type.Int },
        { typeof(float), Variant.Type.Float },
        { typeof(double), Variant.Type.Float },
        { typeof(string), Variant.Type.String },
        { typeof(Vector2), Variant.Type.Vector2 },
        { typeof(Vector2I), Variant.Type.Vector2I },
        { typeof(Rect2), Variant.Type.Rect2 },
        { typeof(Rect2I), Variant.Type.Rect2I },
        { typeof(Vector3), Variant.Type.Vector3 },
        { typeof(Vector3I), Variant.Type.Vector3I },
        { typeof(Transform2D), Variant.Type.Transform2D },
        { typeof(Vector4), Variant.Type.Vector4 },
        { typeof(Vector4I), Variant.Type.Vector4I },
        { typeof(Plane), Variant.Type.Plane },
        { typeof(Quaternion), Variant.Type.Quaternion },
        { typeof(Aabb), Variant.Type.Aabb },
        { typeof(Basis), Variant.Type.Basis },
        { typeof(Transform3D), Variant.Type.Transform3D },
        { typeof(Projection), Variant.Type.Projection },
        { typeof(Color), Variant.Type.Color },
        { typeof(StringName), Variant.Type.StringName },
        { typeof(NodePath), Variant.Type.NodePath },
        { typeof(Rid), Variant.Type.Rid },
        { typeof(GodotObject), Variant.Type.Object },
        { typeof(Callable), Variant.Type.Callable },
        { typeof(Signal), Variant.Type.Signal },
        { typeof(Godot.Collections.Dictionary), Variant.Type.Dictionary },
        { typeof(Array), Variant.Type.Array },
        { typeof(byte[]), Variant.Type.PackedByteArray },
        { typeof(int[]), Variant.Type.PackedInt32Array },
        { typeof(long[]), Variant.Type.PackedInt64Array },
        { typeof(float[]), Variant.Type.PackedFloat32Array },
        { typeof(double[]), Variant.Type.PackedFloat64Array },
        { typeof(string[]), Variant.Type.PackedStringArray },
        { typeof(Vector2[]), Variant.Type.PackedVector2Array },
        { typeof(Vector3[]), Variant.Type.PackedVector3Array },
        { typeof(Color[]), Variant.Type.PackedColorArray },
        { typeof(Vector4[]), Variant.Type.PackedVector4Array },
    };

    /// <summary>
    /// Return the Variant Type corresponding to a C# type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Variant.Type GetVariantType(Type type)
    {
        if (typeMap.ContainsKey(type))
        {
            return typeMap[type];
        }
        else if (type.IsEnum)
        {
            return Variant.Type.Int;
        }
        else
        {
            if (type.IsSubclassOf(typeof(GodotObject)))
            {
                return Variant.Type.Object;
            }
            GD.PrintErr($"Type {type} is not supported for export");
            throw new NotSupportedException($"Type {type} is not supported for export");
        }
    }

    /// <summary>
    /// Convert a C# object to a Variant, using the type map to determine the correct Variant type
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Variant ConvertToVariant(object value)
    {
        var type = value.GetType();
        switch (GetVariantType(type))
        {
            case Variant.Type.Int:
                return Variant.From((int)value);
            case Variant.Type.Float:
                return Variant.From((float)value);
            case Variant.Type.String:
                return Variant.From((string)value);
            case Variant.Type.Vector2:
                return Variant.From((Vector2)value);
            case Variant.Type.Vector2I:
                return Variant.From((Vector2I)value);
            case Variant.Type.Rect2:
                return Variant.From((Rect2)value);
            case Variant.Type.Rect2I:
                return Variant.From((Rect2I)value);
            case Variant.Type.Vector3:
                return Variant.From((Vector3)value);
            case Variant.Type.Vector3I:
                return Variant.From((Vector3I)value);
            case Variant.Type.Transform2D:
                return Variant.From((Transform2D)value);
            case Variant.Type.Vector4:
                return Variant.From((Vector4)value);
            case Variant.Type.Vector4I:
                return Variant.From((Vector4I)value);
            case Variant.Type.Plane:
                return Variant.From((Plane)value);
            case Variant.Type.Quaternion:
                return Variant.From((Quaternion)value);
            case Variant.Type.Aabb:
                return Variant.From((Aabb)value);
            case Variant.Type.Basis:
                return Variant.From((Basis)value);
            case Variant.Type.Transform3D:
                return Variant.From((Transform3D)value);
            case Variant.Type.Projection:
                return Variant.From((Projection)value);
            case Variant.Type.Color:
                return Variant.From((Color)value);
            case Variant.Type.StringName:
                return Variant.From((StringName)value);
            case Variant.Type.NodePath:
                return Variant.From((NodePath)value);
            case Variant.Type.Rid:
                return Variant.From((Rid)value);
            case Variant.Type.Object:
                return Variant.From((GodotObject)value);
            case Variant.Type.Callable:
                return Variant.From((Callable)value);
            case Variant.Type.Signal:
                return Variant.From((Signal)value);
            case Variant.Type.Dictionary:
                return Variant.From((Godot.Collections.Dictionary)value);
            case Variant.Type.Array:
                return Variant.From((Array)value);
            case Variant.Type.PackedByteArray:
                return Variant.From((byte[])value);
            case Variant.Type.PackedInt32Array:
                return Variant.From((int[])value);
            case Variant.Type.PackedInt64Array:
                return Variant.From((long[])value);
            case Variant.Type.PackedFloat32Array:
                return Variant.From((float[])value);
            case Variant.Type.PackedFloat64Array:
                return Variant.From((double[])value);
            case Variant.Type.PackedStringArray:
                return Variant.From((string[])value);
            case Variant.Type.PackedVector2Array:
                return Variant.From((Vector2[])value);
            case Variant.Type.PackedVector3Array:
                return Variant.From((Vector3[])value);
            case Variant.Type.PackedColorArray:
                return Variant.From((Color[])value);
            case Variant.Type.PackedVector4Array:
                return Variant.From((Vector4[])value);
            default:
                GD.PrintErr($"Type {type} is not supported for export");
                throw new NotSupportedException($"Type {type} is not supported for export");
        }
    }
}
