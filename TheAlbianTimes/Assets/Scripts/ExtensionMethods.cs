using System;
using System.Reflection;
using UnityEngine;

public static class ExtensionMethods
{
    public static TComponent CopyComponent<TComponent>(this TComponent destination, TComponent originalComponent)
        where TComponent : Component
    {
        Type originalComponentType = originalComponent.GetType();

        FieldInfo[] fieldInfos = originalComponentType.GetFields();

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            fieldInfo.SetValue(destination, fieldInfo.GetValue(originalComponent));
        }

        return destination;
    }
}
