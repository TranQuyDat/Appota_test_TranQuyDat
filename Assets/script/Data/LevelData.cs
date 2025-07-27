using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{ 
    public int levelId;
    public int Width;
    public int Height;
    public BlockData[] blocks;
    public SawBladeData[] sawBlades;
    public int moveCount;

    public string ToJson()
    {
        string json = JsonUtility.ToJson(this, true);
        Debug.Log(json);
        return json;
    }
    public void FromJson(string json)
    {
        LevelData data = JsonUtility.FromJson<LevelData>(json);
        levelId = data.levelId;
        blocks = data.blocks;
        sawBlades = data.sawBlades;
        Width = data.Width;
        Height = data.Height;
        moveCount = data.moveCount;
    }
}
public enum BlockDir { up = 0, right = 1, left = 2, down = 3 }
[Serializable]
public class BlockData
{
    public Vector2 grid;
    public int UnlockCount = 0;
    public BlockDir dir;
}

[Serializable]
public class SawBladeData
{
    public Vector2 grid;
}