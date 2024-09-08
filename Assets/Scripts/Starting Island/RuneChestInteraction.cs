using UnityEngine;

public class RuneChestInteraction : MonoBehaviour
{
    public GameObject RuneUI; // Reference to the Rune UI
    public SpriteRenderer chestSpriteRenderer; // Reference to the chest's sprite renderer
    public Sprite closedChestSprite; // Sprite for the closed chest
    public Sprite openChestSprite; // Sprite for the open chest
    public GameObject interactPrompt; // Reference to the "Press F to open" UI prompt

    private bool isPlayerNearby = false; // Track if player is nearby
    private bool isChestOpen = false; // Track if the chest is open
    private ScreenManager screenManager; // Reference to the ScreenManager

    private void Start()
    {
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
        RuneUI.SetActive(true); // Show the Rune UI
        screenManager.OpenRunesScreen(); // Open the runes screen through the screen manager
        chestSpriteRenderer.sprite = openChestSprite; // Change to open chest sprite
        interactPrompt.SetActive(false); // Hide the prompt when chest is opened
    }

    private void CloseChest()
    {
        isChestOpen = false;
        screenManager.CloseCurrentScreen(); // Close the current screen through the screen manager
        chestSpriteRenderer.sprite = closedChestSprite; // Change back to closed chest sprite
        RuneUI.SetActive(false); // Hide the Rune UI
    }
}
