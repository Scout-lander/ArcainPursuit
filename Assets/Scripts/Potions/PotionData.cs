using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion-stats")]
public class PotionData : ScriptableObject
{
    public string potionName;
    public string description;
    public Sprite icon;
    public string iconName;
    public float duration;

    public float healthRestoration;
    public PlayerStats.Stats statBuffs;

    public GameObject effectPrefab;

    public void SetIcon(Sprite icon)
    {
        iconName = icon.name; // Save the name of the sprite
    }

    public Sprite GetIcon()
    {
        return Resources.Load<Sprite>("PotionIcon/" + iconName); // Load the sprite from Resources
    }
    
    public void ApplyPotion(PlayerStats playerStats)
    {
        // Heal the player first
        playerStats.RestoreHealth(healthRestoration);
        
        // Apply buffs
        playerStats.ApplyPotionEffect(this);
   }
}

