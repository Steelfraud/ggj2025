
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModifierHandler : MonoBehaviour
{
    private List<BasicModifierSource> activeModifiers = new List<BasicModifierSource>();

    public float GetValueModifier(ModifiedValueNumber valueToGet)
    {
        float value = 0f;

        //IEnumerable<BasicModifierSource> modifiers = activeModifiers.OrderBy(x => x.ModifierPriority);
        List<BasicModifierSource> modifiers = activeModifiers.OrderByDescending(x => x.ModifierPriority()).ToList();
        
        foreach (BasicModifierSource modifier in modifiers)
        {
            if (modifier.ModificationType == ModificationType.Multiplier)
            {
                value = modifier.GetMultiplierModifierValue(valueToGet, value);
            }
            else
            {
                value += modifier.GetModifierValue(valueToGet);
            }            
        }

        return value;
    }

    private void Update()
    {
        List<BasicModifierSource> modifiers = new List<BasicModifierSource>(activeModifiers);
        
        foreach (BasicModifierSource modifier in modifiers)
        {
            if (modifier.IsTimedModifier == false)
                continue;

            modifier.TimeStayed += Time.deltaTime;

            if (modifier.TimedOut)
            {
                RemoveModifier(modifier);
            }           
        }
    }

    public void AddModifier(BasicModifierSource modifier)
    {
        activeModifiers.Add(modifier);
        Debug.Log("Added new modifier!");
    }

    public void RemoveModifier(BasicModifierSource modifier)
    {
        activeModifiers.Remove(modifier);
        Debug.Log("Removed modifier!");
    }

}