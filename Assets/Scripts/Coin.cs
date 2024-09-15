using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Coin : MonoBehaviour, IDataPersistance
{
    [SerializeField]
    int X;
    [SerializeField]
    int Y;
    [SerializeField]
    int id;
    // Start is called before the first frame update
    public void LoadData(GameData data)
    {
        if (data.data.ContainsKey(id.ToString()))
        {
          Debug.Log(data.data.First().Key + "" + data.data.First().Value.First());
        }
    }

    public void SaveData(GameData data)
    {
        if (!data.data.ContainsKey(id.ToString()))
        {
            data.data.Add(id.ToString(), new() { { nameof(X), X.ToString() }, { nameof(Y), Y.ToString() } });
        }
    }
}
