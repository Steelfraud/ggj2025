using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;


[System.Serializable]
public class ModifierData
{
    public NumberModifierDictionary CombatModifierValues;
    public int ModifierPriority = 0;
    public float TimeToStay = 0f;

    [Header("Value modifiers")]
    public bool AllowNegativeValues = true;
        
    public float GetModifierValue(ModifiedValueNumber valueToGet)
    {
        if (this.CombatModifierValues.ContainsKey(valueToGet) == false)
        {
            return 0f;
        }

        float valueToSend = this.CombatModifierValues[valueToGet];

        if (this.AllowNegativeValues == false && valueToSend < 0)
        {
            valueToSend = 0;
        }

        return valueToSend;
    }
        
 }

public class BasicModifierSource
{

    public Action ModifierDataChanged;
    public float TimeStayed;

    public bool TimedOut => TimeStayed >= baseData.TimeToStay;
    public bool IsTimedModifier => baseData.TimeToStay > 0f;
    

    protected ModifierData baseData;
    protected float defaultMultiplier = 1f;

    public BasicModifierSource(ModifierData data)
    {
        this.baseData = data;
    }

    public void SetDefaultValueMultiplier(float setTo)
    {
        this.defaultMultiplier = setTo;
    }

    public virtual int ModifierPriority()
    {
        return this.baseData.ModifierPriority;
    }

    public float GetModifierValue(ModifiedValueNumber valueToGet)
    {
        return this.baseData.GetModifierValue(valueToGet) * GetDefaultMultiplier();
    }

    protected virtual float GetDefaultMultiplier()
    {
        return this.defaultMultiplier;
    }

}

[System.Serializable]
public class NumberModifierDictionary : SerializableDictionaryBase<ModifiedValueNumber, float> { }

public enum StatModificationType
{
    None,
    Addition,
    Multiplier
}

public enum ModifiedValueNumber
{
    MoveForce,
    MoveTorque,
    DashForce
}