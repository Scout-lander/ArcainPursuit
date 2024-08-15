using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public static PlayerGold instance;

    public int totalGold;

    public delegate void OnGoldChanged(int newGoldAmount);
    public event OnGoldChanged onGoldChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Load the saved gold amount
            LoadGold();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to add gold to the player's total
    public void AddGold(int amount)
    {
        totalGold += amount;
        SaveGold();
        onGoldChanged?.Invoke(totalGold);
       // Debug.Log("Gold added: " + amount + ". Total gold: " + totalGold);
    }

    // Method to spend gold
    public bool SpendGold(int amount)
    {
        if (totalGold >= amount)
        {
            totalGold -= amount;
            SaveGold();
            onGoldChanged?.Invoke(totalGold);
            Debug.Log("Gold spent: " + amount + ". Total gold: " + totalGold);
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough gold to spend. Required: " + amount + ", but have: " + totalGold);
            return false;
        }
    }

    // Method to check if the player has enough gold
    public bool HasEnoughGold(int amount)
    {
        return totalGold >= amount;
    }

    // Method to save the current gold amount
    private void SaveGold()
    {
        PlayerPrefs.SetInt("PlayerGold", totalGold);
    }

    // Method to load the saved gold amount
    private void LoadGold()
    {
        totalGold = PlayerPrefs.GetInt("PlayerGold", 0); // Load gold, default to 0 if no saved data
        onGoldChanged?.Invoke(totalGold);
        //Debug.Log("Gold loaded: " + totalGold);
    }

    private void OnApplicationQuit()
    {
        SaveGold(); // Save gold when the application is closed
    }
}
