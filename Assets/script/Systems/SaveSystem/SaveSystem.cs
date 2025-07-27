using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem 
{
    static ISave _localSave = new LocalSave();
    public static void Save(string key, string data)
    {
        _localSave.Save(key, data);
        PlayerPrefs.Save();
    }
    public static string Load(string key)
    {
        if (!PlayerPrefs.HasKey(key)) return "";
        return _localSave.Load(key);
    }
}