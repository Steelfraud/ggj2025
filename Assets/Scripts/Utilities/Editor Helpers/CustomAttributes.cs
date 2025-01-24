using System;
using System.Collections.Generic;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field)]
public class ClassTypeAttribute : PropertyAttribute
{
    public Type TargetType;
    public bool IncludeTargetType = false;
    public string[] ExcludedTypes;

}

[AttributeUsage(AttributeTargets.Field)]
public class ActionEditorAttribute : PropertyAttribute
{
    
}

[AttributeUsage(AttributeTargets.Field)]
public class MapAttribute : PropertyAttribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class KeybindAttribute : PropertyAttribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class MapKeybindAttribute : KeybindAttribute
{

    public int MapToUse = 0;

}