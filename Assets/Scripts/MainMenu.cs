using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu :Menu
{

    [Header("Naviganion")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueGameButton;
    [SerializeField] Button loadGameButton;

    private void Start()
    {
        if (!StorageServiceManager.Instance.HasGameData())
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }

    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }
    public void ActivateMenu()
    {
        gameObject.SetActive(true);
    }
    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }

    public void OnContinueGameClicked()
    {
        DisableMenuButtons();

        StorageServiceManager.Instance.SaveGame();

        SceneManager.LoadSceneAsync("SampleScene");
    }

    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }


}