using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SaveData
{
    public int _levelId;
    public SaveData() { }
    public SaveData(int levelId)
    {
        _levelId = levelId;
    }
    public int LevelId => _levelId;

    public string ToJson() => JsonUtility.ToJson(this);
    public void FromJson(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        _levelId = data._levelId;
    }

}
