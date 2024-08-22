using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PotionInventory : MonoBehaviour
{
    public List<PotionData> allPotions = new List<PotionData>();

    [Header("Do not edit the Potions. It is updated by scripts.")]
    [Header("You can only update the Capacity.")]
    public PotionBagSerializable potionBag = new PotionBagSerializable();
    public PotionData equippedPotion;

    public int maxCapacity = 30;  // Maximum number of potions in the inventory
    public float potionUseCooldown = 60f;
    private float lastPotionUseTime = -60f;

    private SaveLoadManager saveLoadManager;
    private EquippedPotionDisplay equippedPotionDisplay; // Reference to the display script

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        equippedPotionDisplay = FindObjectOfType<EquippedPotionDisplay>(); // Find the display script
        
        if (saveLoadManager == null)
        {
            Debug.LogError("SaveLoadManager not found!");
        }

        if (equippedPotionDisplay == null)
        {
            Debug.LogError("EquippedPotionDisplay not found!");
        }

        LoadInventory();
    }

    public void UpdateEquippedPotionDisplay()
    {
        equippedPotionDisplay?.UpdatePotionIcon();
    }

    private void Update()
    {
        if (equippedPotion != null && Input.GetKeyDown(KeyCode.LeftControl) && Time.time - lastPotionUseTime >= potionUseCooldown)
        {
            UsePotion();
        }

        // Update the display with the remaining cooldown
        equippedPotionDisplay?.UpdateCooldownDisplay();
    }

    public float GetLastPotionUseTime()
    {
        return lastPotionUseTime;
    }

    public bool AddPotion(PotionData potion)
    {
        if (potionBag.potions.Count < maxCapacity)
        {
            // Add the potion's name to the list
            potionBag.potions.Add(potion.potionName);
            SaveInventory(); // Call the save method to persist the inventory
            return true;
        }
        Debug.Log("Inventory is full. Cannot add more potions.");
        return false;
    }

    public void EquipPotion(PotionData potion)
    {
        if (potionBag.potions.Contains(potion.potionName))
        {
            if (equippedPotion != null)
            {
                UnequipPotion(equippedPotion);
            }

            potionBag.potions.Remove(potion.potionName);
            equippedPotion = potion;
            SaveInventory();
            equippedPotionDisplay?.UpdatePotionIcon(); // Update the display
        }
    }

    public void UnequipPotion(PotionData potion)
    {
        if (equippedPotion == potion)
        {
            equippedPotion = null;
            potionBag.potions.Add(potion.potionName);
            SaveInventory();
            equippedPotionDisplay?.UpdatePotionIcon(); // Update the display
        }
    }

    public void UsePotion()
    {
        if (equippedPotion != null && Time.time - lastPotionUseTime >= potionUseCooldown)
        {
            equippedPotion.ApplyPotion(GetComponent<PlayerStats>());
            lastPotionUseTime = Time.time;
            equippedPotionDisplay?.StartPotionCooldown(potionUseCooldown); // Notify display of cooldown
            SaveInventory();
        }
    }

    private void SaveInventory()
    {
        saveLoadManager?.SavePotionInventory(this);
    }

    private void LoadInventory()
    {
        var loadedData = saveLoadManager?.LoadPotionInventory();
        if (loadedData != null)
        {
            // Map the saved potion names back to PotionData objects and update potionBag.potions with the names
            potionBag.potions = loadedData.potions
                .Select(potionName => allPotions.Find(p => p.potionName == potionName))
                .Where(potion => potion != null) // Ensure only valid potions are added
                .Select(potion => potion.potionName) // Convert back to a list of names (string)
                .ToList();

            equippedPotion = allPotions.Find(p => p.potionName == loadedData.equippedPotion);

            if (equippedPotion == null && !string.IsNullOrEmpty(loadedData.equippedPotion))
            {
                Debug.LogWarning($"Equipped potion '{loadedData.equippedPotion}' not found in allPotions.");
            }

            equippedPotionDisplay?.UpdatePotionIcon(); // Update the display if it's available
        }
        else
        {
            Debug.LogWarning("No potion data found to load.");
        }
    }
}
