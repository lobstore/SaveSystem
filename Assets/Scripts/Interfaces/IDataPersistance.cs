
/// <summary>
/// The interface for saving data must be implemented by all persistentable objects in the scene
/// </summary>
public interface IDataPersistance
{
    void LoadData(GameData data);

    void SaveData(GameData data);
}