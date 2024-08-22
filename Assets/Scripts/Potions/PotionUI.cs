using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotionUI : MonoBehaviour
{
    public GameObject potionSlotPrefab;
    public Transform equippedPotionTransform;  // Assign this in the Unity editor
    public TMP_Text potionNameText;
    public TMP_Text potionDescriptionText;
    public Button equipButton;
    public Button unequipButton;
    public Button destroyButton;
    public TMP_Text notificationText;

    private Transform potionInventoryTransform;
    private List<Image> potionSlotImages = new List<Image>();
    private List<Image> potionIconImages = new List<Image>();
    private int selectedPotionIndex = -1;

    public GameObject playerObject;
    private PotionInventory potionInventory;
    private SaveLoadManager saveLoadManager;
    public EquippedPotionDisplay equippedPotionDisplay;
    public EquippedPotionDisplay equippedPotionDisplay1;

    void Start()
    {
        potionInventory = playerObject.GetComponent<PotionInventory>();
        if (potionInventory == null)
        {
            Debug.LogError("PotionInventory component not found on the player object.");
            return;
        }

        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        potionInventoryTransform = transform.Find("Potion Inventory");
        equippedPotionTransform = transform.Find("Equipped Potion");

        CreatePotionSlots();
        CreateEquippedPotionSlot();
        UpdatePotionSlots();

        equipButton.onClick.AddListener(() => EquipSelectedPotion(selectedPotionIndex));
        unequipButton.onClick.AddListener(UnequipSelectedPotion);
        destroyButton.onClick.AddListener(() => DestroySelectedPotion(selectedPotionIndex));

        ClearSelectedPotion();
    }

    void OnEnable()
    {
        UpdatePotionSlots();
    }

    private void CreateEquippedPotionSlot()
    {
        GameObject slotObj = Instantiate(potionSlotPrefab, equippedPotionTransform);
        Image potionSlotImage = slotObj.GetComponent<Image>();
        Image potionIconImage = slotObj.transform.Find("Icon").GetComponent<Image>();
        potionSlotImages.Add(potionSlotImage);
        potionIconImages.Add(potionIconImage);

        Button slotButton = slotObj.GetComponent<Button>();
        if (slotButton == null)
        {
            slotButton = slotObj.AddComponent<Button>();
        }
        slotButton.interactable = true; // Ensure button is interactable
        slotButton.onClick.AddListener(OnEquippedPotionClicked);

        // Enable raycast target on the image
        potionSlotImage.raycastTarget = true;
        potionIconImage.raycastTarget = true;
    }

    private void CreatePotionSlots()
    {
        foreach (Transform child in potionInventoryTransform)
        {
            Destroy(child.gameObject);
        }
        potionSlotImages.Clear();
        potionIconImages.Clear();

        for (int i = 0; i < potionInventory.maxCapacity; i++)
        {
            GameObject slotObj = Instantiate(potionSlotPrefab, potionInventoryTransform);
            Image potionSlotImage = slotObj.GetComponent<Image>();
            Image potionIconImage = slotObj.transform.Find("Icon").GetComponent<Image>();

            potionSlotImages.Add(potionSlotImage);
            potionIconImages.Add(potionIconImage);

            int index = i;
            Button slotButton = slotObj.GetComponent<Button>();
            if (slotButton == null)
            {
                slotButton = slotObj.AddComponent<Button>();
            }
            slotButton.onClick.AddListener(() => OnPotionClicked(index));
        }
    }

    private void UpdatePotionSlots()
    {
        for (int i = 0; i < potionSlotImages.Count; i++)
        {
            if (i < potionInventory.potionBag.potions.Count)
            {
                // Find the PotionData corresponding to the potion name
                string potionName = potionInventory.potionBag.potions[i];
                PotionData potion = potionInventory.allPotions.Find(p => p.potionName == potionName);

                if (potion != null)
                {
                    potionIconImages[i].sprite = potion.icon;
                    potionIconImages[i].color = Color.white;
                }
                else
                {
                    Debug.LogWarning($"Potion '{potionName}' not found in allPotions.");
                    potionIconImages[i].sprite = null;
                    potionIconImages[i].color = new Color(0, 0, 0, 0);
                }
            }
            else
            {
                potionIconImages[i].sprite = null;
                potionIconImages[i].color = new Color(0, 0, 0, 0);
                potionSlotImages[i].color = new Color(0, 0, 0, 0.5f);
            }
        }

        UpdateEquippedPotionSlot();
    }

    private void OnPotionClicked(int index)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            EquipSelectedPotion(index);
        }
        else
        {
            selectedPotionIndex = index;
            string potionName = potionInventory.potionBag.potions[index];
            
            // Find the corresponding PotionData
            PotionData selectedPotion = potionInventory.allPotions.Find(p => p.potionName == potionName);
            
            if (selectedPotion != null)
            {
                UpdateDetails(selectedPotion, showEquipButton: true, showUnequipButton: false);
            }
            else
            {
                Debug.LogWarning($"Potion '{potionName}' not found in allPotions.");
            }
        }
    }

    private void OnEquippedPotionClicked()
    {
        if (potionInventory.equippedPotion != null)
        {
            Debug.Log("Equipped potion slot clicked."); // Debug to check if the method is triggered
            UpdateDetails(potionInventory.equippedPotion, showEquipButton: false, showUnequipButton: true);
        }
    }

    private void EquipSelectedPotion(int index)
    {
        if (index >= 0 && index < potionInventory.potionBag.potions.Count)
        {
            // Retrieve the potion name
            string potionName = potionInventory.potionBag.potions[index];
            
            // Find the corresponding PotionData using the potion name
            PotionData selectedPotion = potionInventory.allPotions.Find(p => p.potionName == potionName);
            
            if (selectedPotion != null)
            {
                // Equip the potion
                potionInventory.EquipPotion(selectedPotion);

                UpdateEquippedPotionSlot();
                UpdatePotionSlots();
                ClearSelectedPotion();

                // Notify the EquippedPotionDisplay to update
                equippedPotionDisplay?.UpdatePotionIcon();
                equippedPotionDisplay1?.UpdatePotionIcon();
            }
            else
            {
                Debug.LogWarning($"Potion '{potionName}' not found in allPotions.");
            }
        }
    }

    private void UnequipSelectedPotion()
    {
        if (potionInventory.equippedPotion != null)
        {
            potionInventory.UnequipPotion(potionInventory.equippedPotion);
            UpdateEquippedPotionSlot();
            UpdatePotionSlots();
            ClearSelectedPotion();
        }
    }

    private void UpdateEquippedPotionSlot()
    {
        Image potionIconImage = equippedPotionTransform.GetChild(0).Find("Icon").GetComponent<Image>();

        if (potionInventory.equippedPotion != null)
        {
            potionIconImage.sprite = potionInventory.equippedPotion.icon;
            potionIconImage.color = Color.white;
        }
        else
        {
            potionIconImage.sprite = null;
            potionIconImage.color = new Color(0, 0, 0, 0);  // Make the icon placeholder transparent
        }
    }

    private void DestroySelectedPotion(int index)
    {
        if (index < potionInventory.potionBag.potions.Count)
        {
            // Retrieve the potion name
            string potionName = potionInventory.potionBag.potions[index];
            
            // Find the corresponding PotionData using the potion name
            PotionData potionToDestroy = potionInventory.allPotions.Find(p => p.potionName == potionName);
            
            if (potionToDestroy != null)
            {
                if (potionToDestroy == potionInventory.equippedPotion)
                {
                    potionInventory.UnequipPotion(potionToDestroy);
                }

                // Remove the potion by its name from the list
                potionInventory.potionBag.potions.RemoveAt(index);

                UpdatePotionSlots();
                UpdateEquippedPotionSlot();
                ClearSelectedPotion();
            }
            else
            {
                Debug.LogWarning($"Potion '{potionName}' not found in allPotions.");
            }
        }
    }

    private void UpdateDetails(PotionData potion, bool showEquipButton, bool showUnequipButton)
    {
        potionNameText.text = potion.potionName;
        potionDescriptionText.text = potion.description;
        potionNameText.gameObject.SetActive(true);
        potionDescriptionText.gameObject.SetActive(true);
        equipButton.gameObject.SetActive(showEquipButton);
        unequipButton.gameObject.SetActive(showUnequipButton);
        destroyButton.gameObject.SetActive(true);  // Make sure the destroy button is visible
    }

    private void ClearSelectedPotion()
    {
        selectedPotionIndex = -1;
        potionNameText.gameObject.SetActive(false);
        potionDescriptionText.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        destroyButton.gameObject.SetActive(false);
    }
}
