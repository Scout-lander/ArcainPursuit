using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneController : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;
    private RuneInventory runeInventory;

    public GameObject loadingScreen, loadingIcon;
    public TMP_Text loadingText;

    private void Awake()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        runeInventory = FindObjectOfType<RuneInventory>();
    }

    public void SceneChange(string name)
    {
        if (saveLoadManager != null && runeInventory != null)
        {
            saveLoadManager.SaveRuneBags(runeInventory.runeBag, runeInventory.equippedRuneBag);
        }
        else
        {
            Debug.LogWarning("SaveLoadManager or RuneInventory not found. Cannot save rune bags.");
        }

        StartCoroutine(LoadMain(name));
    }

    private IEnumerator LoadMain(string name)
    {
        loadingScreen.SetActive(true);
        loadingText.text = "Loading...";

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // Update loading icon or text here to show progress
            if (asyncLoad.progress < 0.9f)
            {
                loadingText.text = $"Loading {asyncLoad.progress * 100}%";
            }
            else
            {
                loadingText.text = "Press any key to continue";
                
                // Wait for user input to activate the scene
                if (Input.anyKeyDown)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        // Reset time scale just in case it was modified in the previous scene
        Time.timeScale = 1f;
        loadingScreen.SetActive(false);
    }
}
