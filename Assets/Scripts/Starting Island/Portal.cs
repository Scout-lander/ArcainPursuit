using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string targetSceneName = "GameScene"; // Name of the scene to transition to
    public WeaponInventory weaponInventory; // Reference to the player's weapon inventory
    private SceneController sceneController; // Reference to the SceneController

    private void Start()
    {
        // Find the SceneController in the scene
        sceneController = FindObjectOfType<SceneController>();

        // Check if the SceneController was found
        if (sceneController == null)
        {
            Debug.LogError("SceneController not found in the scene.");
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pass the equipped weapon to the CharacterSelector
            if (weaponInventory.equippedWeapon != null)
            {
                CharacterSelector.instance.SetEquippedWeapon(weaponInventory.equippedWeapon);
            }

            // Use the SceneController to change the scene
            if (sceneController != null)
            {
                sceneController.SceneChange(targetSceneName);
            }
            else
            {
                Debug.LogWarning("SceneController is not assigned. Cannot change scene.");
            }
        }
    }
}
