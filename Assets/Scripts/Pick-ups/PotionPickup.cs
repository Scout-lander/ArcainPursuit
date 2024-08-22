using UnityEngine;

public class PotionPickup : Pickup
{
    public PotionData potionData; // Reference to the PotionData
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    protected override void Start()
    {
        base.Start();

        // Get the SpriteRenderer component attached to the PotionPickup object
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && potionData != null)
        {
            // Set the sprite to the potion's icon
            spriteRenderer.sprite = potionData.GetIcon();
        }
        else
        {
            Debug.LogWarning("SpriteRenderer or PotionData is missing on PotionPickup.");
        }
    }

    public override bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (base.Collect(target, speed, lifespan))
        {
            // Check for inventory component and attempt to add potion
            PotionInventory potionInventory = target.GetComponent<PotionInventory>();
            if (potionInventory != null && potionData != null)
            {
                Debug.Log("Attempting to add potion: " + potionData.potionName);
                bool added = potionInventory.AddPotion(potionData);
                
                if (added)
                {
                    Debug.Log("Potion added successfully to the inventory: " + potionData.potionName);
                    return true; // Potion added successfully
                }
                else
                {
                    // Inventory was full, potion not added
                    Debug.LogWarning("Could not add potion: Inventory full or potion already in the inventory");
                    return false; // Return false to prevent destruction of the pickup
                }
            }
            else
            {
                Debug.LogWarning("PotionInventory component or PotionData is missing.");
            }
        }
        return false;
    }
}
