using UnityEngine;

public class ShopKeeperInteraction : MonoBehaviour
{
    public GameObject shopUI; // Reference to the Shop UI
    public GameObject interactPrompt; // Reference to the "Press F to interact" UI prompt

    private bool isPlayerNearby = false; // Track if player is nearby
    private bool isShopOpen = false; // Track if the shop is open
    private ScreenManager screenManager; // Reference to the ScreenManager

    private void Start()
    {
        shopUI.SetActive(false); // Initialize Shop UI as inactive
        screenManager = FindObjectOfType<ScreenManager>(); // Find the ScreenManager in the scene
        interactPrompt.SetActive(false); // Ensure the interact prompt is initially hidden
    }

    private void Update()
    {
        if (isPlayerNearby && !isShopOpen && Input.GetKeyDown(KeyCode.F))
        {
            OpenShop(); // Open the shop when 'F' is pressed
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // Player is now nearby
            interactPrompt.SetActive(true); // Show the "Press F to interact" prompt
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player is no longer nearby
            CloseShop(); // Close the shop and UI elements
            interactPrompt.SetActive(false); // Hide the interact prompt
        }
    }

    private void OpenShop()
    {
        isShopOpen = true;
        shopUI.SetActive(true); // Show the Shop UI
        screenManager.OpenShopScreen(); // Open the shop screen through the screen manager
        interactPrompt.SetActive(false); // Hide the prompt when the shop is opened
    }

    private void CloseShop()
    {
        isShopOpen = false;
        screenManager.CloseCurrentScreen(); // Close the current screen through the screen manager
        shopUI.SetActive(false); // Hide the Shop UI
    }
}
