using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedPotionDisplay : MonoBehaviour
{
    public Image potionIconImage; // The UI Image component to display the potion icon
    public TMP_Text potionNameText; // TMP Text component to display the potion name
    public TMP_Text potionDescriptionText; // TMP Text component to display the potion description
    public TMP_Text cooldownText; // TMP Text component to display the cooldown timer

    public PotionInventory potionInventory; // Reference to the PotionInventory script

    void Start()
    {


        if (potionInventory != null)
        {
            // Update the potion icon display when the game starts
            UpdatePotionIcon();
        }
        else
        {
                Debug.LogError("PotionInventory component not found on the player object.");
        }
    }

    // This method updates the UI with the currently equipped potion's details
    public void UpdatePotionIcon()
    {
        if (potionInventory.equippedPotion != null)
        {
            // Update the potion icon
            potionIconImage.sprite = potionInventory.equippedPotion.icon;
            potionIconImage.color = Color.white; // Make sure the icon is visible

            // Update the potion name and description
            potionNameText.text = potionInventory.equippedPotion.potionName;
            potionDescriptionText.text = potionInventory.equippedPotion.description;

            // Hide cooldown text initially
            cooldownText.gameObject.SetActive(false);
        }
        else
        {
            // Clear the icon, name, and description if no potion is equipped
            potionIconImage.sprite = null;
            potionIconImage.color = new Color(0, 0, 0, 0); // Make the icon transparent

            potionNameText.text = string.Empty;
            potionDescriptionText.text = string.Empty;
            cooldownText.gameObject.SetActive(false); // Hide cooldown text
        }
    }

    // This method starts the cooldown display
    public void StartPotionCooldown(float cooldownDuration)
    {
        cooldownText.gameObject.SetActive(true); // Show cooldown text
        UpdatePotionCooldown(cooldownDuration); // Start the countdown
    }

    // This method updates the potion icon display with the cooldown timer
    public void UpdatePotionCooldown(float remainingCooldown)
    {
        potionIconImage.color = Color.gray; // Gray out the icon during cooldown
        cooldownText.text = Mathf.Ceil(remainingCooldown).ToString(); // Update cooldown text
    }

    // This method resets the potion icon display after the cooldown
    public void ResetPotionDisplay()
    {
        potionIconImage.color = Color.white; // Restore the icon color
        cooldownText.gameObject.SetActive(false); // Hide the cooldown text
    }

    // You can call this method from PotionInventory during cooldown updates
    public void UpdateCooldownDisplay()
    {
        if (potionInventory != null && potionInventory.equippedPotion != null)
        {
            float remainingCooldown = Mathf.Max(0, potionInventory.potionUseCooldown - (Time.time - potionInventory.GetLastPotionUseTime()));
            if (remainingCooldown > 0)
            {
                StartPotionCooldown(remainingCooldown);
            }
            else
            {
                ResetPotionDisplay();
            }
        }
    }
}
