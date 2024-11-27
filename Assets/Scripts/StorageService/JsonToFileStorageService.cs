using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonToFileStorageService : IStorageService
{
    private string key;
    private bool useEncryption;
    private readonly string backupExtension = ".bak";

    public JsonToFileStorageService(string key, bool useEncryption = true)
    {
        this.key = key;
        this.useEncryption = useEncryption;
    }

    public void Load(string profileId , Action<GameData> callback, bool allowRestoreFromBackUp = true)
    {
        if (profileId == null)
        {
            callback(null);
            return;
        }

        GameData data = null;
        string path = BuildPath(profileId, key);
        try
        {
            if (File.Exists(path))
            {
                string jsonData;

                using (var filestream = new FileStream(path, FileMode.Open))
                using (var streamReader = new StreamReader(filestream))
                {
                    jsonData = streamReader.ReadToEnd();
                }

                if (useEncryption)
                {
                    jsonData =EncryptionTool.DecryptData(jsonData);
                }

                data = JsonConvert.DeserializeObject<GameData>(jsonData);
            }
        }
        catch (Exception e)
        {
            if (allowRestoreFromBackUp)
            {
                Debug.LogWarning("Failed to load data file. atempring to roll back." + "\n" + e);
                bool rollbackSuccess = AttemptRollback(path);
                if (rollbackSuccess)
                {
                    Load(profileId, callback, false);
                }
            }
            else
            {
                Debug.LogError("Error occured when trying to load file and backup did not work.");
            }


        }


        callback?.Invoke(data);
    }


    public void Save(GameData data, string profileId, Action<bool> callback = null)
    {

        if (profileId == null)
        {
            return;
        }

        string path = BuildPath(profileId, key);
        string backupFilePath = path + backupExtension;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string dataToStore = JsonConvert.SerializeObject(data);

            if (useEncryption)
            {
                dataToStore = EncryptionTool.EncryptData(dataToStore);
            }

            using (var filestream = new FileStream(path, FileMode.Create))
            using (var streamWriter = new StreamWriter(filestream))
            {
                streamWriter.Write(dataToStore);
            }
            GameData veryfiedGameData = null;
            Load(profileId, data => veryfiedGameData = data);
            if (veryfiedGameData != null)
            {
                File.Copy(path, backupFilePath, true);
            }

            else
            {
                throw new Exception("Save file could not be verified and backup could not be ceated");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error occured when trying to save data to file: " + path + "\n" + e);
        }

        callback?.Invoke(true);
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(Application.persistentDataPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            string fullPath = Path.Combine(Application.persistentDataPath, profileId, key);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory: " + profileId);
                continue;
            }
            GameData profileData = null;
            Load(profileId, data => profileData = data);

            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Load Profile Error: " + profileId);
            }
        }
        return profileDictionary;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ID of the profile that was last updated/saved</returns>
    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;
        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (var pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            if (gameData == null)
            {
                continue;
            }

            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);

                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }

        }
        return mostRecentProfileId;
    }

    private bool AttemptRollback(string fullpath)
    {
        bool success = false;

        string backupFilePath = fullpath + backupExtension;
        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullpath, true);
            }
            else
            {
                throw new Exception("Tried to roll back, bot no backup file exists");
            }
        }
        catch (Exception e)
        {

            Debug.LogError(e);
        }
        return success;
    }

    private string BuildPath(string profileId, string key)
    {
        return Path.Combine(Application.persistentDataPath, profileId, key);
    }
}
