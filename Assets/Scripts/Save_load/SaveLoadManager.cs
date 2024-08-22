using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public class SaveLoadManager : MonoBehaviour
{
    private string runeBagsSavePath;
    private string equippedWeaponSavePath;
    private string toggleStateSavePath;
    private string passiveUpgradesSavePath;
    private string potionBagSavePath;
    private string equippedPotionSavePath;

    private const string UpgradesKey = "Upgrades";
    private const string OneOffItemsKey = "OneOffItems";

    private void Awake()
    {
        string saveFolderPath = Path.Combine(Application.persistentDataPath, "Save");

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        runeBagsSavePath = Path.Combine(saveFolderPath, "runeBags.json");
        equippedWeaponSavePath = Path.Combine(saveFolderPath, "equippedWeapon.json");
        toggleStateSavePath = Path.Combine(saveFolderPath, "toggleState.json");
        passiveUpgradesSavePath = Path.Combine(saveFolderPath, "passiveUpgrades.json");
        potionBagSavePath = Path.Combine(saveFolderPath, "potionBag.json");
        equippedPotionSavePath = Path.Combine(saveFolderPath, "equippedPotion.json");

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
        string encryptedJson = EncryptionUtility.Encrypt(json);
        File.WriteAllText(runeBagsSavePath, encryptedJson);
    }

   public void SaveUpgrades(List<PassiveUpgrades.Upgrade> upgrades, List<PassiveUpgrades.OneOffItem> oneOffItems, bool gameSceneLoaded)
    {
        PassiveUpgradesData data = new PassiveUpgradesData
        {
            upgrades = upgrades,
            oneOffItems = oneOffItems,
            gameSceneLoaded = gameSceneLoaded
        };

        string json = JsonUtility.ToJson(data, true);
        string encryptedJson = EncryptionUtility.Encrypt(json);
        
        // Save the encrypted JSON to the file
        File.WriteAllText(passiveUpgradesSavePath, encryptedJson);
    }

    public (List<PassiveUpgrades.Upgrade>, List<PassiveUpgrades.OneOffItem>, bool) LoadUpgrades()
    {
        if (File.Exists(passiveUpgradesSavePath))
        {
            string encryptedJson = File.ReadAllText(passiveUpgradesSavePath);
            
            // Decrypt the JSON
            string json = EncryptionUtility.Decrypt(encryptedJson);
            
            PassiveUpgradesData data = JsonUtility.FromJson<PassiveUpgradesData>(json);
            return (data.upgrades, data.oneOffItems, data.gameSceneLoaded);
        }
        else
        {
            Debug.LogWarning("Passive upgrades save file not found.");
            return (new List<PassiveUpgrades.Upgrade>(), new List<PassiveUpgrades.OneOffItem>(), false);
        }
    }

    public void LoadRuneBags(RuneInventory runeInventory)
    {
        if (File.Exists(runeBagsSavePath))
        {
            string encryptedJson = File.ReadAllText(runeBagsSavePath);
            string json = EncryptionUtility.Decrypt(encryptedJson);
            RuneBagsData data = JsonUtility.FromJson<RuneBagsData>(json);
            runeInventory.runeBag = data.runeBag;
            runeInventory.equippedRuneBag = data.equippedRuneBag;
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
    }

    public WeaponData LoadEquippedWeapon()
    {
        if (File.Exists(equippedWeaponSavePath))
        {
            string json = File.ReadAllText(equippedWeaponSavePath);
            EquippedWeaponData data = JsonUtility.FromJson<EquippedWeaponData>(json);
            return data.equippedWeapon;
        }
        else
        {
            Debug.LogWarning("Equipped weapon save file not found.");
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
    }

    public bool LoadToggleState()
    {
        if (File.Exists(toggleStateSavePath))
        {
            string json = File.ReadAllText(toggleStateSavePath);
            ToggleStateData data = JsonUtility.FromJson<ToggleStateData>(json);
            return data.isOn;
        }
        else
        {
            Debug.LogWarning("Toggle state save file not found.");
            return false;
        }
    }

    public void SavePotionInventory(PotionInventory potionInventory)
    {
        PotionBagSerializable data = new PotionBagSerializable
        {
            potions = potionInventory.potionBag.potions, // Already storing potion names
            equippedPotion = potionInventory.equippedPotion?.potionName
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(potionBagSavePath, json);

        Debug.Log("Potion inventory saved: " + json);
    }

    public PotionBagSerializable LoadPotionInventory()
    {
        if (File.Exists(potionBagSavePath))
        {
            string json = File.ReadAllText(potionBagSavePath);
            PotionBagSerializable data = JsonUtility.FromJson<PotionBagSerializable>(json);

            Debug.Log("Potion inventory loaded: " + json);
            return data;
        }
        else
        {
            Debug.LogWarning("Potion inventory save file not found.");
            return null;
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
            var (upgrades, oneOffItems, gameSceneLoaded) = LoadUpgrades();
            passiveUpgrades.purchasedUpgrades = upgrades;
            passiveUpgrades.purchasedOneOffItems = oneOffItems;
            passiveUpgrades.SetGameSceneLoaded(gameSceneLoaded);
        }

        PotionInventory potionInventory = FindObjectOfType<PotionInventory>();
        if (potionInventory != null)
        {
            PotionBagSerializable loadedData = LoadPotionInventory(); // Load the serialized data

            // Convert potion names back to PotionData objects
            potionInventory.potionBag.potions = loadedData.potions
                .Select(potionName => potionInventory.allPotions.Find(p => p.potionName == potionName))
                .Where(potion => potion != null) // Ensure only valid potions are added
                .Select(potion => potion.potionName) // Convert back to a list of names (string)
                .ToList();

            // Convert the equipped potion name back to a PotionData object
            potionInventory.equippedPotion = potionInventory.allPotions
                .Find(p => p.potionName == loadedData.equippedPotion);

            if (potionInventory.equippedPotion == null && !string.IsNullOrEmpty(loadedData.equippedPotion))
            {
                Debug.LogWarning($"Equipped potion '{loadedData.equippedPotion}' not found in allPotions.");
            }

            potionInventory.UpdateEquippedPotionDisplay();
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
            SaveUpgrades(passiveUpgrades.purchasedUpgrades, passiveUpgrades.purchasedOneOffItems, passiveUpgrades.IsGameSceneLoaded());
        }

        PotionInventory potionInventory = FindObjectOfType<PotionInventory>();
        if (potionInventory != null)
        {
            SavePotionInventory(potionInventory);
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

        if (File.Exists(potionBagSavePath))
        {
            File.Delete(potionBagSavePath);
            Debug.Log("Potion inventory save file deleted.");
        }

        if (File.Exists(equippedPotionSavePath))
        {
            File.Delete(equippedPotionSavePath);
            Debug.Log("Equipped potion save file deleted.");
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

    [System.Serializable]
    public class PotionBagData
    {
        public List<PotionData> potions = new List<PotionData>();
        public PotionData equippedPotion; // Changed to single PotionData
    }
}
