using Newtonsoft.Json;
using System;
using System.Collections.Generic;
/// <summary>
/// Game data to storage for scene object
/// </summary>
[Serializable]
public class GameData
{
    public long lastUpdated;
    [JsonProperty(propertyName: "data")]
    public Dictionary<string, Dictionary<string, string>> data = new();
}
