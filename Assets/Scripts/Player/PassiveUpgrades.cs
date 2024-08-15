using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Xml.Serialization;

public class PassiveUpgrades : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;
    private bool gameSceneLoaded = false;

    [System.Serializable]
    public class Upgrade
    {
        public string upgradeName;
        public int level;
        public PlayerStats.Stats statBoost;
    }

    [System.Serializable]
    public class OneOffItem
    {
        public string itemName;
        public PlayerStats.Stats statBoost;
    }

    public List<Upgrade> purchasedUpgrades = new List<Upgrade>();
    public List<OneOffItem> purchasedOneOffItems = new List<OneOffItem>();

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        LoadUpgrades();

        // SceneManager.sceneLoaded += OnSceneLoaded;
        //    Debug.Log("OnSceneLoaded has been subscribed to sceneLoaded event.");

        // Manually trigger to test
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool IsGameSceneLoaded()
    {
        return gameSceneLoaded;
    }

    public void SetGameSceneLoaded(bool value)
    {
        gameSceneLoaded = value;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       //Debug.Log($"Scene Loaded: {scene.name}");

        if (scene.name != "Base") // Replace with your actual base scene name
        {
            //Debug.Log("Non-base scene loaded. Setting gameSceneLoaded to true.");
            gameSceneLoaded = true;
            SaveUpgrades();
        }

       //Debug.Log($"gameSceneLoaded: {gameSceneLoaded}, scene.name: {scene.name}");

        if (gameSceneLoaded == true && scene.name == "Base")
        {
            //Debug.Log("Base scene loaded after game scene. Removing all one-off items.");
            RemoveAllOneOffItems();
            gameSceneLoaded = false;
        }
    }

    private void RemoveAllOneOffItems()
    { 
        if(gameSceneLoaded == true)
        //Debug.Log("Current one-off items: " + purchasedOneOffItems.Count);
        purchasedOneOffItems.Clear();
        SaveUpgrades();
        //Debug.Log("All one-off items removed after game scene.");
    }

    public void AddUpgrade(UpgradeData upgradeData, int level)
    {
        Upgrade existingUpgrade = purchasedUpgrades.Find(upgrade => upgrade.upgradeName == upgradeData.upgradeName);
        if (existingUpgrade != null)
        {
            existingUpgrade.level = level;
            existingUpgrade.statBoost = upgradeData.levels[level].statBoost;
            Debug.Log($"Upgraded {upgradeData.upgradeName} to level {level}");
        }
        else
        {
            Upgrade newUpgrade = new Upgrade
            {
                upgradeName = upgradeData.upgradeName,
                level = level,
                statBoost = upgradeData.levels[level].statBoost
            };
            purchasedUpgrades.Add(newUpgrade);
            Debug.Log($"Added new upgrade: {upgradeData.upgradeName} at level {level}");
        }
        SaveUpgrades();
    }

    public void AddOneOffItem(OneOffItemData itemData)
    {
        OneOffItem newItem = new OneOffItem
        {
            itemName = itemData.itemName,
            statBoost = itemData.statBoost
        };
        purchasedOneOffItems.Add(newItem);
        Debug.Log($"Added new one-off item: {itemData.itemName}");
        SaveUpgrades();
    }

    public bool HasOneOffItem(string itemName)
    {
        return purchasedOneOffItems.Exists(item => item.itemName == itemName);
    }

    public void ApplyUpgrades(PlayerStats playerStats)
    {
        foreach (Upgrade upgrade in purchasedUpgrades)
        {
            playerStats.ActualStats += upgrade.statBoost;
        }
        foreach (OneOffItem item in purchasedOneOffItems)
        {
            playerStats.ActualStats += item.statBoost;
        }
        playerStats.RecalculateStats();
    }

    public void RemoveOneOffItem(OneOffItemData itemData)
    {
        purchasedOneOffItems.RemoveAll(item => item.itemName == itemData.itemName);
        purchasedOneOffItems.Clear();        
        Debug.Log($"Removed one-off item: {itemData.itemName}");
        SaveUpgrades();
    }
    public void RemovedSaved()
    {
        SaveUpgrades();
    }

    private void SaveUpgrades()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.SaveUpgrades(purchasedUpgrades, purchasedOneOffItems, gameSceneLoaded); // Include gameSceneLoaded
        }
    }

    private void LoadUpgrades()
    {
        if (saveLoadManager != null)
        {
            (purchasedUpgrades, purchasedOneOffItems, gameSceneLoaded) = saveLoadManager.LoadUpgrades(); // Load gameSceneLoaded
        }
    }
}
