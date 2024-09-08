using UnityEngine;
using TMPro; // Assuming you're using TextMeshPro for text elements

public class ChestInteraction : MonoBehaviour
{
    public GameObject characterInventoryUI; // Reference to the Character Inventory UI
    public GameObject WeaponSelection; // Reference to the Weapon Selection UI
    public SpriteRenderer chestSpriteRenderer; // Reference to the chest's sprite renderer
    public Sprite closedChestSprite; // Sprite for the closed chest
    public Sprite openChestSprite; // Sprite for the open chest
    public GameObject interactPrompt; // Reference to the "Press F to open" UI prompt

    private bool isPlayerNearby = false; // Track if player is nearby
    private bool isChestOpen = false; // Track if the chest is open
    private ScreenManager screenManager; // Reference to the ScreenManager

    private void Start()
    {
        WeaponSelection.SetActive(false); // Initialize Weapon Selection UI as inactive
        chestSpriteRenderer.sprite = closedChestSprite; // Set the chest to closed sprite initially
        screenManager = FindObjectOfType<ScreenManager>(); // Find the ScreenManager in the scene
        interactPrompt.SetActive(false); // Ensure the interact prompt is initially hidden
    }

    private void Update()
    {
        if (isPlayerNearby && !isChestOpen && Input.GetKeyDown(KeyCode.F))
        {
            OpenChest(); // Open the chest when 'F' is pressed
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // Player is now nearby
            interactPrompt.SetActive(true); // Show the "Press F to open" prompt
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player is no longer nearby
            CloseChest(); // Close the chest and UI elements
            interactPrompt.SetActive(false); // Hide the interact prompt
        }
    }

    private void OpenChest()
    {
        isChestOpen = true;
        OpenInventoryUI();
        screenManager.OpenWeaponsScreen();
        chestSpriteRenderer.sprite = openChestSprite; // Change to open chest sprite
        interactPrompt.SetActive(false); // Hide the prompt when chest is opened
    }

    private void CloseChest()
    {
        isChestOpen = false;
        screenManager.CloseCurrentScreen();
        chestSpriteRenderer.sprite = closedChestSprite; // Change back to closed chest sprite
        CloseInventoryUI();
    }

    private void OpenInventoryUI()
    {
        characterInventoryUI.SetActive(true);
        WeaponSelection.SetActive(true);
        characterInventoryUI.GetComponent<CharacterInventoryUI>().UpdateUI();
    }

    private void CloseInventoryUI()
    {
        characterInventoryUI.SetActive(false);
        WeaponSelection.SetActive(false);
    }
}
