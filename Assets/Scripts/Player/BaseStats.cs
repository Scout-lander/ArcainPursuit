using System;
using UnityEngine;

[Serializable]
public class BaseStats
{
    public float strength = 5f;
    public float intelligence = 5f;
    public float dexterity = 5f;
    public float vitality = 5f;
    public float willpower = 5f;

    // Method to apply base stats to player's actual stats
    public void ApplyToStats(ref PlayerStats.Stats stats)
    {
        stats.maxHealth += vitality * 10f;    // Vitality adds to max health
        stats.armor += strength * 0.5f;       // Strength adds to armor
        stats.might += strength * 0.3f;       // Strength adds to might
        stats.recovery += vitality * 0.1f;    // Vitality adds to recovery
        stats.speed += dexterity * 0.2f;      // Dexterity adds to speed
        stats.luck += intelligence * 0.1f;    // Intelligence adds to luck
        // Modify the effects according to your game balance needs
    }

    // Optional: Operator overloads for combining base stats
    public static BaseStats operator +(BaseStats s1, BaseStats s2)
    {
        return new BaseStats
        {
            strength = s1.strength + s2.strength,
            intelligence = s1.intelligence + s2.intelligence,
            dexterity = s1.dexterity + s2.dexterity,
            vitality = s1.vitality + s2.vitality,
            willpower = s1.willpower + s2.willpower
        };
    }

    public static BaseStats operator -(BaseStats s1, BaseStats s2)
    {
        return new BaseStats
        {
            strength = s1.strength - s2.strength,
            intelligence = s1.intelligence - s2.intelligence,
            dexterity = s1.dexterity - s2.dexterity,
            vitality = s1.vitality - s2.vitality,
            willpower = s1.willpower - s2.willpower
        };
    }
}
