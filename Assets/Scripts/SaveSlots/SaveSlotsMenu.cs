using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Navigation")]
    [SerializeField] private MainMenu mainMenu;
    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;
    [Header("Confirmation Popup")]
    [SerializeField] ConfirmationPopupMenu confirmationPopupMenu;
    bool isLoadingGame;
   private SaveSlot[] saveSlots;

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        if (isLoadingGame)
        {
            StorageServiceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            SaveGameAndLoadScene();
        }
        else if (saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a New Game with this slot will overrride the currently saved data. Are you sure?",
                () => {
                    StorageServiceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    StorageServiceManager.Instance.NewGame();
                    SaveGameAndLoadScene();
                },
                () => {
                    ActivateMenu(isLoadingGame);
                }
                );

        }
        else
        {
            StorageServiceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            StorageServiceManager.Instance.NewGame();
            SaveGameAndLoadScene();
        }
    }

    private void SaveGameAndLoadScene()
    {
        StorageServiceManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync("SampleScene");
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        gameObject.SetActive(true);
        this.isLoadingGame = isLoadingGame;
        Dictionary<string, GameData> profilesGameData = StorageServiceManager.Instance.GetAllProfilesGameData();

        backButton.interactable = true;

        GameObject firstSelected = backButton.gameObject;
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
            Button firstSelectedButton = firstSelected.GetComponent<Button>();
            SetFirstSelected(firstSelectedButton);
        }
    }
    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }

}
