using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StorageServiceManager : MonoBehaviour
{
    private GameData gameData;


    [Header("Debugging")]
    [SerializeField]
    bool initializeGameDataIfNull;
    [SerializeField]
    bool disableDataPersistance = false;
    [SerializeField]
    bool overrideSelectedProfileId = false;
    [SerializeField]
    string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField]
    bool useEncription;
    [SerializeField]
    string savePath;

    [Header("Auto Saving Configuration")]
    [SerializeField] float autosaveTimeSeconds = 60;
    [SerializeField] bool autoSave = true;
    private string selectedProfileId = "";

    private Coroutine autoSaveCoroutine;

    public static StorageServiceManager Instance { get; private set; }
    private List<IDataPersistance> dataPersistances = new List<IDataPersistance>();
    IStorageService storageService;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistance)
        {
            Debug.LogWarning("Data Persistance is currently Disabled");
        }

        storageService = new JsonToFileStorageService(savePath, useEncription);

        selectedProfileId = storageService.GetMostRecentlyUpdatedProfileId();

        if (overrideSelectedProfileId)
        {
            selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    private List<IDataPersistance> FindAllDataPersistances()
    {
        IEnumerable<IDataPersistance> dataPersistances = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistances);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistances = FindAllDataPersistances();
        LoadGame();
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        if (autoSave)
        {
            autoSaveCoroutine = StartCoroutine(AutoSave());

        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void ChangeSelectedProfileId(string profileId)
    {
        selectedProfileId = profileId;
        LoadGame();
    }

    public void LoadGame()
    {
        if (disableDataPersistance)
        {
            return;
        }
        storageService.Load(selectedProfileId, data => gameData = data);
        if (gameData == null)
        {
            if (initializeGameDataIfNull)
            {
                NewGame();
            }
            else
            {
                return;

            }
        }
        else
        {
            foreach (IDataPersistance data in dataPersistances)
            {
                data.LoadData(gameData);
            }
        }
    }
    public void NewGame()
    {
        gameData = new GameData();
    }
    public void SaveGame()
    {
        if (disableDataPersistance)
        {
            return;
        }
        if (gameData == null)
        {
            return;
        }
        else
        {
            foreach (IDataPersistance data in dataPersistances)
            {
                data.SaveData(gameData);
            }
            gameData.lastUpdated = System.DateTime.Now.ToBinary();
            storageService.Save(gameData, selectedProfileId);
        }
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return storageService.LoadAllProfiles();
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autosaveTimeSeconds);
            SaveGame();
            Debug.Log("Game AutoSaved");
        }
    }
    public bool HasGameData()
    {
        return gameData != null;
    }
}
