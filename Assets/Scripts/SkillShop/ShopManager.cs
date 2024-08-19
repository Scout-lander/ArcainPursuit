using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public UpgradeData[] upgrades;
    public OneOffItemData[] oneOffItems; // Array of one-off items
    public Transform upgradesContent; // Container for upgrades
    public Transform oneOffItemsContent; // Container for one-off items
    public GameObject upgradeButtonPrefab; // Use the same prefab for both upgrades and one-off items
    public PlayerStats playerStats;
    public TMP_Text goldText; // For shop UI
    public TMP_Text nonShopGoldText; // For non-shop UI
    public PassiveUpgrades passiveUpgrades;
    public GameObject shopUI;

    [Header("Tab Buttons")]
    public Button upgradesTabButton; // Button to switch to upgrades
    public Button oneOffItemsTabButton; // Button to switch to one-off items
    public Button removeAllButton; // Reference to the remove all button

    [Header("Cannot Buy")]
    public Color cannotBuyFlashColor = Color.red; // Color to flash when unable to purchase
    public float cannotBuyShakeDuration = 0.5f; // Duration of the shake effect
    public float cannotBuyShakeMagnitude = 10f; // Magnitude of the shake effect

    [Header("Can Buy")]
    public Color canBuyFlashColor = Color.green; // Color to flash when purchase is successful
    public float canBuyFlashDuration = 0.5f; // Duration of the flash effect

    private bool isShopOpen = false; // Track if the shop is open

    private void Start()
    {
        InitializeShop();
        UpdateGoldUI(PlayerGold.instance.totalGold); // Pass the current gold amount here
        ToggleShop(false); // Ensure the shop starts closed
        removeAllButton.onClick.AddListener(RemoveAllBoughtItems); // Add listener to the remove all button
        PlayerGold.instance.onGoldChanged += UpdateGoldUI; // Subscribe to the gold change event

         // Add listeners to the tab buttons
        upgradesTabButton.onClick.AddListener(() => SwitchTab(true));
        oneOffItemsTabButton.onClick.AddListener(() => SwitchTab(false));

        // Start with the upgrades tab open
        SwitchTab(true);
    }

    private void OnDestroy()
    {
        PlayerGold.instance.onGoldChanged -= UpdateGoldUI; // Unsubscribe to avoid memory leaks
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleShop(!isShopOpen);
        }
    }

    private void InitializeShop()
    {
        // Clear existing buttons first
        foreach (Transform child in upgradesContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in oneOffItemsContent)
        {
            Destroy(child.gameObject);
        }

        // Create buttons for upgrades
        foreach (UpgradeData upgrade in upgrades)
        {
            CreateUpgradeButton(upgrade);
        }

        // Create buttons for one-off items
        foreach (OneOffItemData item in oneOffItems)
        {
            CreateOneOffItemButton(item);
        }
    }

    private void CreateUpgradeButton(UpgradeData upgrade)
    {
        GameObject button = Instantiate(upgradeButtonPrefab, upgradesContent);

        button.transform.Find("Icon").GetComponent<Image>().sprite = upgrade.icon;
        button.transform.Find("Name").GetComponent<TMP_Text>().text = upgrade.upgradeName;

        int currentLevel = GetUpgradeLevel(upgrade.upgradeName);
        if (currentLevel < upgrade.levels.Length)
        {
            button.transform.Find("Cost").GetComponent<TMP_Text>().text = upgrade.levels[currentLevel].cost.ToString();
            button.transform.Find("Level").GetComponent<TMP_Text>().text = $"Level {currentLevel} -> {currentLevel + 1}";
            button.transform.Find("Description").GetComponent<TMP_Text>().text = upgrade.levels[currentLevel].description;
            Button purchaseButton = button.transform.Find("PurchaseButton").GetComponent<Button>();
            purchaseButton.onClick.AddListener(() => PurchaseUpgrade(upgrade, button));
        }
        else
        {
           // Set the UI elements for a maxed-out upgrade
            button.transform.Find("Cost").GetComponent<TMP_Text>().text = "";
            button.transform.Find("Level").GetComponent<TMP_Text>().text = "Maxed Level";
            button.transform.Find("Description").GetComponent<TMP_Text>().text = "";

            // Disable the purchase button
            Button purchaseButton = button.transform.Find("PurchaseButton").GetComponent<Button>();
            purchaseButton.interactable = false;

            // Change the button and text color to grey to indicate that the upgrade is maxed out
            Color greyColor = Color.grey;
            button.GetComponent<Image>().color = greyColor; // Change the button background to grey
            button.transform.Find("Icon").GetComponent<Image>().color = greyColor; // Change the icon color to grey
            button.transform.Find("Name").GetComponent<TMP_Text>().color = greyColor; // Change the name text color to grey
            button.transform.Find("Level").GetComponent<TMP_Text>().color = greyColor; // Change the level text color to grey

            Debug.Log($"{upgrade.upgradeName} is maxed out");
        }
    }

    private void CreateOneOffItemButton(OneOffItemData item)
    {
        GameObject button = Instantiate(upgradeButtonPrefab, oneOffItemsContent);

        // Set the icon, name, cost, and description
        button.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
        button.transform.Find("Name").GetComponent<TMP_Text>().text = item.itemName;
        button.transform.Find("Cost").GetComponent<TMP_Text>().text = item.cost.ToString();
        button.transform.Find("Description").GetComponent<TMP_Text>().text = item.description;
        button.transform.Find("Level").GetComponent<TMP_Text>().text = ""; // No level for one-off items

        Button purchaseButton = button.transform.Find("PurchaseButton").GetComponent<Button>();
        purchaseButton.onClick.AddListener(() => PurchaseOneOffItem(item, button));

        // Check if the item has already been purchased or owned
        if (passiveUpgrades.HasOneOffItem(item.itemName))
        {
            // Grey out the icon
            button.transform.Find("Icon").GetComponent<Image>().color = Color.grey;
            
            // Disable the purchase button
            purchaseButton.interactable = false;

            // Grey out the button and text to indicate the item is already owned
            button.GetComponent<Image>().color = Color.grey; // Grey out the button background
            button.transform.Find("Name").GetComponent<TMP_Text>().color = Color.grey; // Grey out the name text
            button.transform.Find("Cost").GetComponent<TMP_Text>().text = "Owned"; // Grey out the cost text
            button.transform.Find("Description").GetComponent<TMP_Text>().color = Color.grey; // Grey out the description text
        }
    }

    private int GetUpgradeLevel(string upgradeName)
    {
        foreach (var upgrade in passiveUpgrades.purchasedUpgrades)
        {
            if (upgrade.upgradeName == upgradeName)
            {
                return upgrade.level + 1; // Upgrade levels start from 0, so return the next level
            }
        }
        return 0; // If not found, assume it’s at level 0
    }

    private void PurchaseUpgrade(UpgradeData upgrade, GameObject button)
    {
        int currentLevel = GetUpgradeLevel(upgrade.upgradeName);
        Debug.Log($"Attempting to purchase {upgrade.upgradeName} at level {currentLevel}");

        if (currentLevel < upgrade.levels.Length && PlayerGold.instance.HasEnoughGold(upgrade.levels[currentLevel].cost))
        {
            PlayerGold.instance.SpendGold(upgrade.levels[currentLevel].cost);
            Debug.Log($"Purchased {upgrade.upgradeName} level {currentLevel} for {upgrade.levels[currentLevel].cost} gold.");
            ApplyUpgrade(upgrade, currentLevel);
            StartCoroutine(FlashButton(button, canBuyFlashColor, canBuyFlashDuration)); // Flash green on successful purchase
            InitializeShop(); // Reinitialize the shop to refresh the buttons
        }
        else
        {
            StartCoroutine(FlashRedAndShake(button));
            Debug.Log("Not enough gold or upgrade maxed out!");
        }
    }

    private void PurchaseOneOffItem(OneOffItemData item, GameObject button)
    {
        Debug.Log($"Attempting to purchase one-off item {item.itemName}");

        if (PlayerGold.instance.HasEnoughGold(item.cost))
        {
            PlayerGold.instance.SpendGold(item.cost);
            Debug.Log($"Purchased one-off item {item.itemName} for {item.cost} gold.");
            StartCoroutine(FlashButton(button, canBuyFlashColor, canBuyFlashDuration)); // Flash green on successful purchase
            passiveUpgrades.AddOneOffItem(item);
            UseOneOffItem(item);
            InitializeShop(); // Reinitialize the shop to refresh the buttons
        }
        else
        {
            StartCoroutine(FlashRedAndShake(button));
            Debug.Log("Not enough gold!");
        }
    }

    private void ApplyUpgrade(UpgradeData upgrade, int level)
    {
        Debug.Log($"Applying upgrade: {upgrade.upgradeName} at level {level}");
        passiveUpgrades.AddUpgrade(upgrade, level);
        passiveUpgrades.ApplyUpgrades(playerStats);
    }

   private void UseOneOffItem(OneOffItemData item)
    {
        Debug.Log($"Using one-off item {item.itemName}");
        // Implement the effect of the one-off item here
        // For example, if it's a Double XP item, you can double the XP gain for a certain period

        // Do not remove the item immediately; PassiveUpgrades will handle removal after scene change
    }

    private IEnumerator RemoveOneOffItemAfterUse(OneOffItemData item)
    {
        yield return new WaitForSeconds(5f); // Simulate some delay before removing the item
        passiveUpgrades.RemoveOneOffItem(item);
        Debug.Log($"One-off item {item.itemName} used and removed from inventory.");
    }

    private void UpdateGoldUI(int newGoldAmount)
    {
        // Update both the shop UI and the non-shop UI
        goldText.text = $"Gold: {newGoldAmount}";
        nonShopGoldText.text = $"Gold: {newGoldAmount}";
        //Debug.Log($"Updated gold: {newGoldAmount}");
    }

    public void ToggleShop(bool open)
    {
        isShopOpen = open;
        shopUI.SetActive(isShopOpen);
        if (!isShopOpen)
        {
            InitializeShop();
        }
    }

    private void SwitchTab(bool showUpgrades)
    {
        // Toggle the content visibility
        upgradesContent.gameObject.SetActive(showUpgrades);
        oneOffItemsContent.gameObject.SetActive(!showUpgrades);

        // Update button colors based on the active content
        if (showUpgrades)
        {
            // Show the upgrades tab, set the upgrades button to normal and others to gray
            upgradesTabButton.image.color = Color.white;
            oneOffItemsTabButton.image.color = Color.grey;
        }
        else
        {
            // Show the one-off items tab, set the one-off items button to normal and others to gray
            upgradesTabButton.image.color = Color.grey;
            oneOffItemsTabButton.image.color = Color.white;
        }
    }

    private void RemoveAllBoughtItems()
    {
        Debug.Log("Removing all bought items");
        // Clear the shop buttons first
        foreach (Transform child in upgradesContent)
        {
            Destroy(child.gameObject);
        }
        passiveUpgrades.purchasedUpgrades.Clear();
        passiveUpgrades.purchasedOneOffItems.Clear();
        PlayerPrefs.DeleteAll(); // This will reset all upgrades to level 0
        passiveUpgrades.RemovedSaved();
        InitializeShop(); // Reinitialize the shop to reflect the changes
        passiveUpgrades.ApplyUpgrades(playerStats); // Reapply upgrades to ensure no stats are carried over
        Debug.Log("All bought items removed");
    }

    private IEnumerator FlashRedAndShake(GameObject button)
    {
        Image[] images = button.GetComponentsInChildren<Image>();
        Color[] originalColors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            originalColors[i] = images[i].color;
            images[i].color = cannotBuyFlashColor;
        }

        Vector3 originalPosition = button.transform.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < cannotBuyShakeDuration)
        {
            float xOffset = Random.Range(-cannotBuyShakeMagnitude, cannotBuyShakeMagnitude);
            float yOffset = Random.Range(-cannotBuyShakeMagnitude, cannotBuyShakeMagnitude);

            button.transform.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = originalColors[i];
        }

        button.transform.localPosition = originalPosition;
    }

    private IEnumerator FlashButton(GameObject button, Color flashColor, float flashDuration)
    {
        Image[] images = button.GetComponentsInChildren<Image>();
        Color[] originalColors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            originalColors[i] = images[i].color;
            images[i].color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = originalColors[i];
        }
    }
}
