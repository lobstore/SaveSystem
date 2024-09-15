using System;
using System.Collections.Generic;

public interface IStorageService
{
    void Save(GameData data, string profileId, Action<bool> callback = null);
    void Load(string profileId, Action<GameData> callback, bool allowRestoreFromBackUp = true);
    Dictionary<string, GameData> LoadAllProfiles();
    string GetMostRecentlyUpdatedProfileId();
}
