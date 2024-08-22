using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionBagSerializable
{
    public List<string> potions = new List<string>(); // Store potion names instead of PotionData
    public string equippedPotion; // Store the name of the equipped potion
}
