using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Buff", menuName = "2D Top-down Rogue-like/Weapon Buff")]
public class WeaponBuff : ScriptableObject
{
    public string buffName;
    public string description;
    public bool firesForwardAndBackward; // Example property for the knife buff
    // Add other buff properties as needed
}
