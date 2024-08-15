using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    private string runeBagsSavePath;
    private string equippedWeaponSavePath;
    private string toggleStateSavePath;
    private string passiveUpgradesSavePath;

    private const string UpgradesKey = "Upgrades";
    private const string OneOffItemsKey = "OneOffItems";

    private void Awake()
    {
        // Combine the paths to ensure they are within the Save folder
        string saveFolderPath = Path.Combine(Application.persistentDataPath, "Save");

        // Ensure the Save directory exists
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        // Set the paths for the save files within the Save folder
        runeBagsSavePath = Path.Combine(saveFolderPath, "runeBags.json");
        equippedWeaponSavePath = Path.Combine(saveFolderPath, "equippedWeapon.json");
        toggleStateSavePath = Path.Combine(saveFolderPath, "toggleState.json");
        passiveUpgradesSavePath = Path.Combine(saveFolderPath, "passiveUpgrades.json");

        // Register the scene loaded and unloaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void SaveRuneBags(RuneBagSerializable runeBag, RuneBagSerializable equippedRuneBag)
    {
        RuneBagsData data = new RuneBagsData
        {
            runeBag = runeBag,
            equippedRuneBag = equippedRuneBag
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(runeBagsSavePath, json);
        //Debug.Log("Rune bags saved to " + runeBagsSavePath);
    }

    public void SaveUpgrades(List<PassiveUpgrades.Upgrade> upgrades, List<PassiveUpgrades.OneOffItem> oneOffItems, bool gameSceneLoaded)
    {
        PassiveUpgradesData data = new PassiveUpgradesData
        {
            upgrades = upgrades,
            oneOffItems = oneOffItems,
            gameSceneLoaded = gameSceneLoaded // Save the value
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(passiveUpgradesSavePath, json);
        //Debug.Log("Passive upgrades saved to " + passiveUpgradesSavePath);
    }

    public (List<PassiveUpgrades.Upgrade>, List<PassiveUpgrades.OneOffItem>, bool) LoadUpgrades()
    {
        if (File.Exists(passiveUpgradesSavePath))
        {
            string json = File.ReadAllText(passiveUpgradesSavePath);
            PassiveUpgradesData data = JsonUtility.FromJson<PassiveUpgradesData>(json);
            //Debug.Log("Passive upgrades loaded from " + passiveUpgradesSavePath);
            return (data.upgrades, data.oneOffItems, data.gameSceneLoaded); // Return the value
        }
        else
        {
            Debug.LogWarning("Passive upgrades save file not found at " + passiveUpgradesSavePath);
            return (new List<PassiveUpgrades.Upgrade>(), new List<PassiveUpgrades.OneOffItem>(), false);
        }
    }

    public void LoadRuneBags(RuneInventory runeInventory)
    {
        if (File.Exists(runeBagsSavePath))
        {
            string json = File.ReadAllText(runeBagsSavePath);
            RuneBagsData data = JsonUtility.FromJson<RuneBagsData>(json);

            runeInventory.runeBag = data.runeBag;
            runeInventory.equippedRuneBag = data.equippedRuneBag;
            //Debug.Log("Rune bags loaded from " + runeBagsSavePath);
        }
        else
        {
            Debug.LogWarning("Save file not found at " + runeBagsSavePath);
        }
    }

    public void SaveEquippedWeapon(WeaponData equippedWeapon)
    {
        EquippedWeaponData data = new EquippedWeaponData
        {
            equippedWeapon = equippedWeapon
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(equippedWeaponSavePath, json);
        //Debug.Log("Equipped weapon saved to " + equippedWeaponSavePath);
    }

    public WeaponData LoadEquippedWeapon()
    {
        if (File.Exists(equippedWeaponSavePath))
        {
            string json = File.ReadAllText(equippedWeaponSavePath);
            EquippedWeaponData data = JsonUtility.FromJson<EquippedWeaponData>(json);
            Debug.Log("Equipped weapon loaded from " + equippedWeaponSavePath);
            return data.equippedWeapon;
        }
        else
        {
            Debug.LogWarning("Equipped weapon save file not found at " + equippedWeaponSavePath);
            return null;
        }
    }

    public void SaveToggleState(bool isOn)
    {
        ToggleStateData data = new ToggleStateData
        {
            isOn = isOn
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(toggleStateSavePath, json);
        //Debug.Log("Toggle state saved to " + toggleStateSavePath);
    }

    public bool LoadToggleState()
    {
        if (File.Exists(toggleStateSavePath))
        {
            string json = File.ReadAllText(toggleStateSavePath);
            ToggleStateData data = JsonUtility.FromJson<ToggleStateData>(json);
            //Debug.Log("Toggle state loaded from " + toggleStateSavePath);
            return data.isOn;
        }
        else
        {
            Debug.LogWarning("Toggle state save file not found at " + toggleStateSavePath);
            return false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RuneInventory runeInventory = FindObjectOfType<RuneInventory>();
        if (runeInventory != null)
        {
            LoadRuneBags(runeInventory);
        }

        ToggleGameObjects toggleGameObjects = FindObjectOfType<ToggleGameObjects>();
        if (toggleGameObjects != null)
        {
            bool toggleState = LoadToggleState();
            toggleGameObjects.SetToggleState(toggleState);
        }

        PassiveUpgrades passiveUpgrades = FindObjectOfType<PassiveUpgrades>();
        if (passiveUpgrades != null)
        {
            var (upgrades, oneOffItems, gameSceneLoaded) = LoadUpgrades();  // Assign to local variables
            passiveUpgrades.purchasedUpgrades = upgrades;
            passiveUpgrades.purchasedOneOffItems = oneOffItems;
            passiveUpgrades.SetGameSceneLoaded(gameSceneLoaded); // Assuming you have a method to set this value
        }
    }
    
    private void OnSceneUnloaded(Scene scene)
    {
        RuneInventory runeInventory = FindObjectOfType<RuneInventory>();
        if (runeInventory != null)
        {
            SaveRuneBags(runeInventory.runeBag, runeInventory.equippedRuneBag);
        }

        PassiveUpgrades passiveUpgrades = FindObjectOfType<PassiveUpgrades>();
        if (passiveUpgrades != null)
        {
            SaveUpgrades(passiveUpgrades.purchasedUpgrades, passiveUpgrades.purchasedOneOffItems, passiveUpgrades.IsGameSceneLoaded()); // Pass the gameSceneLoaded value
        }
    }

    public void DeleteRuneSaveFiles()
    {
        if (File.Exists(runeBagsSavePath))
        {
            File.Delete(runeBagsSavePath);
            Debug.Log("Rune bags save file deleted.");
        }

        if (File.Exists(equippedWeaponSavePath))
        {
            File.Delete(equippedWeaponSavePath);
            Debug.Log("Equipped weapon save file deleted.");
        }

        if (File.Exists(toggleStateSavePath))
        {
            File.Delete(toggleStateSavePath);
            Debug.Log("Toggle state save file deleted.");
        }

        if (File.Exists(passiveUpgradesSavePath))
        {
            File.Delete(passiveUpgradesSavePath);
            Debug.Log("Passive upgrades save file deleted.");
        }
    }

    [System.Serializable]
    private class RuneBagsData
    {
        public RuneBagSerializable runeBag;
        public RuneBagSerializable equippedRuneBag;
    }

    [System.Serializable]
    private class EquippedWeaponData
    {
        public WeaponData equippedWeapon;
    }

    [System.Serializable]
    private class ToggleStateData
    {
        public bool isOn;
    }

    [System.Serializable]
    private class PassiveUpgradesData
    {
        public List<PassiveUpgrades.Upgrade> upgrades;
        public List<PassiveUpgrades.OneOffItem> oneOffItems;
        public bool gameSceneLoaded;
    }
}
