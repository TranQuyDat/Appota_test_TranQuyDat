using UnityEngine;

public class LocalSave : ISave
{
    public string Load(string key) => PlayerPrefs.GetString(key);

    public void Save(string key, string data)
    {
        PlayerPrefs.SetString(key, data);
    }
}