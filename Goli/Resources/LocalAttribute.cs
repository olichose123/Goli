namespace Goli.Resources;

using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LocalAttribute : Attribute
{
    public LocalAttribute() : base()
    {
    }
}
