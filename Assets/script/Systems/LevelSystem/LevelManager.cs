using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData[] _LevelDatas;
    public LevelLoader _levelLoader;
    private int _currentLevelId;
    private void Awake()
    {
        LoadJsonLevels();

        //load save data
        string json = SaveSystem.Load("SaveData");
        if (json != "")
        {
            SaveData saveData = new SaveData();
            saveData.FromJson(json);
            _currentLevelId = saveData.LevelId;
            return;
        }
        _currentLevelId = 1;

    }

    public bool ChangeLevel(int levelid) 
    {
        if (levelid < 1 || levelid > _LevelDatas.Length) return false;
        _currentLevelId = levelid;
        return true;
    }

    private void LoadJsonLevels()
    {
        TextAsset[] jsonLevels = Resources.LoadAll<TextAsset>("Data/Levels");
        _LevelDatas = new LevelData[jsonLevels.Length];
        foreach (TextAsset jsonLevel in jsonLevels)
        {
            LevelData levelData = new LevelData();
            levelData.FromJson(jsonLevel.text);
            _LevelDatas[levelData.levelId - 1] = levelData;
        }

        _levelLoader.Init(_LevelDatas);
    }

    public int CurrentLevelId => _currentLevelId;
    public int LevelCount => _LevelDatas.Length;
}
