using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject weaponsScreen;
    public GameObject runesScreen;
    public GameObject settingsScreen;
    public GameObject potionScreen;
    public GameObject shopScreen;
    public GameObject currentScreen;

    void Start()
    {
        // Set all screens to inactive when the script first loads
        if (weaponsScreen != null) weaponsScreen.SetActive(false);
        if (runesScreen != null) runesScreen.SetActive(false);
        if (settingsScreen != null) settingsScreen.SetActive(false);
        if (potionScreen != null) potionScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            OpenPotionScreen();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenShopScreen();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenRunesScreen();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            OpenWeaponsScreen();
        }
    }

    public void OpenScreen(GameObject screen)
    {
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
        }

        screen.SetActive(true);
        currentScreen = screen;
    }

    public void CloseCurrentScreen()
    {
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
            currentScreen = null;
            Time.timeScale = 1;
        }
    }

    private void HandleEscapeKey()
    {
        if (currentScreen != null)
        {
            CloseCurrentScreen();
        }
        else
        {
            OpenScreen(settingsScreen);
            Time.timeScale = 0;
        }
    }

    public void OpenWeaponsScreen()
    {
        OpenScreen(weaponsScreen);
        Time.timeScale = 0;
    }

    public void OpenRunesScreen()
    {
        OpenScreen(runesScreen);
        Time.timeScale = 0;
    }

    public void OpenSettingsScreen()
    {
        OpenScreen(settingsScreen);
        Time.timeScale = 0;
    }

    public void OpenPotionScreen()
    {
        OpenScreen(potionScreen);
        Time.timeScale = 0;
    }

    public void OpenShopScreen()
    {
        OpenScreen(shopScreen);
        Time.timeScale = 0;
    }
}
